SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
Admission Drafts - schema + stored procedures

Purpose:
- Support public admission application drafts
- Resume via secure token link
- Resume via admission_no + OTP (email/mobile)

Conventions aligned with SchoolERP/DB/schoolfullerp.sql:
- snake_case
- stored procedures return @executionStatus/@message pattern
*/

/* =========================================================
   TABLES
   ========================================================= */

IF OBJECT_ID('dbo.admission_drafts', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.admission_drafts
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_admission_drafts_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        student_id INT NULL,
        admission_no NVARCHAR(50) NOT NULL,

        step_completed TINYINT NOT NULL CONSTRAINT DF_admission_drafts_step_completed DEFAULT(0),
        submitted BIT NOT NULL CONSTRAINT DF_admission_drafts_submitted DEFAULT(0),

        deleted BIT NOT NULL CONSTRAINT DF_admission_drafts_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_admission_drafts_status DEFAULT(1)
    );

    CREATE UNIQUE INDEX ux_admission_drafts_admission_no ON dbo.admission_drafts(admission_no);
    CREATE INDEX ix_admission_drafts_student_id ON dbo.admission_drafts(student_id);
END
GO

-- If table exists from older script versions, ensure Option A compatibility
IF OBJECT_ID('dbo.admission_drafts', 'U') IS NOT NULL
BEGIN
    IF EXISTS(
        SELECT 1
        FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.admission_drafts')
          AND name = 'student_id'
          AND is_nullable = 0
    )
    BEGIN
        ALTER TABLE dbo.admission_drafts ALTER COLUMN student_id INT NULL;
    END
END
GO

IF OBJECT_ID('dbo.admission_resume_tokens', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.admission_resume_tokens
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_admission_resume_tokens_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        draft_id INT NOT NULL,
        token_hash NVARCHAR(128) NOT NULL,
        expires_at DATETIME NOT NULL,
        is_active BIT NOT NULL CONSTRAINT DF_admission_resume_tokens_is_active DEFAULT(1),

        deleted BIT NOT NULL CONSTRAINT DF_admission_resume_tokens_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_admission_resume_tokens_status DEFAULT(1)
    );

    ALTER TABLE dbo.admission_resume_tokens WITH CHECK
    ADD CONSTRAINT fk_admission_resume_tokens_draft
        FOREIGN KEY (draft_id) REFERENCES dbo.admission_drafts(id)
        ON DELETE CASCADE;

    CREATE INDEX ix_admission_resume_tokens_hash ON dbo.admission_resume_tokens(token_hash);
    CREATE INDEX ix_admission_resume_tokens_draft_id ON dbo.admission_resume_tokens(draft_id);
END
GO

IF OBJECT_ID('dbo.admission_resume_otps', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.admission_resume_otps
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_admission_resume_otps_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        draft_id INT NOT NULL,
        channel NVARCHAR(20) NOT NULL, -- EMAIL | SMS
        destination NVARCHAR(255) NOT NULL,
        otp_hash NVARCHAR(128) NOT NULL,
        valid_till DATETIME NOT NULL,
        verified BIT NOT NULL CONSTRAINT DF_admission_resume_otps_verified DEFAULT(0),
        verified_at DATETIME NULL,
        try_count INT NOT NULL CONSTRAINT DF_admission_resume_otps_try_count DEFAULT(0),

        deleted BIT NOT NULL CONSTRAINT DF_admission_resume_otps_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_admission_resume_otps_status DEFAULT(1)
    );

    ALTER TABLE dbo.admission_resume_otps WITH CHECK
    ADD CONSTRAINT fk_admission_resume_otps_draft
        FOREIGN KEY (draft_id) REFERENCES dbo.admission_drafts(id)
        ON DELETE CASCADE;

    CREATE INDEX ix_admission_resume_otps_draft_id ON dbo.admission_resume_otps(draft_id);
    CREATE INDEX ix_admission_resume_otps_valid ON dbo.admission_resume_otps(draft_id, channel, destination, valid_till);
END
GO

/* =========================================================
   STORED PROCEDURES
   ========================================================= */

IF OBJECT_ID('dbo.createAdmissionDraft', 'P') IS NOT NULL
    DROP PROCEDURE dbo.createAdmissionDraft;
GO
CREATE PROCEDURE dbo.createAdmissionDraft
    @studentId INT = NULL,
    @admissionNo NVARCHAR(50),
    @createdById INT = NULL,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@admissionNo IS NULL OR LTRIM(RTRIM(@admissionNo)) = '')
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Admission number is required.';
            RETURN;
        END

        -- Option A: student may not exist yet
        IF (@studentId IS NOT NULL AND @studentId <= 0)
            SET @studentId = NULL;

        -- Idempotency: if a draft already exists for this admission_no, return it.
        SELECT TOP 1 @outputId = id
        FROM dbo.admission_drafts
        WHERE admission_no = @admissionNo
          AND deleted = 0
          AND status = 1
        ORDER BY id DESC;

        IF (@outputId IS NOT NULL AND @outputId > 0)
        BEGIN
            -- If a student is now known, attach it.
            IF (@studentId IS NOT NULL)
            BEGIN
                UPDATE dbo.admission_drafts
                SET student_id = COALESCE(student_id, @studentId)
                WHERE id = @outputId;
            END

            SET @executionStatus = 'TRUE';
            SET @message = 'Admission draft already exists.';
            RETURN;
        END

        INSERT INTO dbo.admission_drafts
        (
            creation_timestamp,
            created_by_id,
            student_id,
            admission_no,
            step_completed,
            submitted,
            deleted,
            status
        )
        VALUES
        (
            GETDATE(),
            @createdById,
            @studentId,
            @admissionNo,
            0,
            0,
            0,
            1
        );

        SET @outputId = SCOPE_IDENTITY();
        SET @executionStatus = 'TRUE';
        SET @message = 'Admission draft created.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

IF OBJECT_ID('dbo.attachStudentToAdmissionDraft', 'P') IS NOT NULL
    DROP PROCEDURE dbo.attachStudentToAdmissionDraft;
GO
CREATE PROCEDURE dbo.attachStudentToAdmissionDraft
    @draftId INT,
    @studentId INT,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@draftId IS NULL OR @draftId <= 0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Invalid draft id.';
            RETURN;
        END

        IF (@studentId IS NULL OR @studentId <= 0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Invalid student id.';
            RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM dbo.admission_drafts WHERE id=@draftId AND deleted=0 AND status=1)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Draft not found.';
            RETURN;
        END

        UPDATE dbo.admission_drafts
        SET student_id = @studentId
        WHERE id = @draftId;

        SET @outputId = @draftId;
        SET @executionStatus = 'TRUE';
        SET @message = 'Student attached to draft.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

IF OBJECT_ID('dbo.getAdmissionDraftByAdmissionNo', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getAdmissionDraftByAdmissionNo;
GO
CREATE PROCEDURE dbo.getAdmissionDraftByAdmissionNo
    @admissionNo NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 d.*
    FROM dbo.admission_drafts d
    WHERE d.deleted = 0
      AND d.status = 1
      AND d.admission_no = @admissionNo
    ORDER BY d.id DESC;
END
GO

IF OBJECT_ID('dbo.createAdmissionResumeToken', 'P') IS NOT NULL
    DROP PROCEDURE dbo.createAdmissionResumeToken;
GO
CREATE PROCEDURE dbo.createAdmissionResumeToken
    @draftId INT,
    @tokenHash NVARCHAR(128),
    @expiresAt DATETIME,
    @createdById INT = NULL,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS(SELECT 1 FROM dbo.admission_drafts WHERE id=@draftId AND deleted=0 AND status=1)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Draft not found.';
            RETURN;
        END

        IF (@tokenHash IS NULL OR LTRIM(RTRIM(@tokenHash)) = '')
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Token hash is required.';
            RETURN;
        END

        IF (@expiresAt IS NULL)
            SET @expiresAt = DATEADD(DAY, 30, GETDATE());

        -- deactivate older tokens for this draft
        UPDATE dbo.admission_resume_tokens
        SET is_active = 0
        WHERE draft_id = @draftId AND deleted = 0 AND status = 1;

        INSERT INTO dbo.admission_resume_tokens
        (
            creation_timestamp,
            created_by_id,
            draft_id,
            token_hash,
            expires_at,
            is_active,
            deleted,
            status
        )
        VALUES
        (
            GETDATE(),
            @createdById,
            @draftId,
            @tokenHash,
            @expiresAt,
            1,
            0,
            1
        );

        SET @outputId = SCOPE_IDENTITY();
        SET @executionStatus = 'TRUE';
        SET @message = 'Resume token created.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

IF OBJECT_ID('dbo.getAdmissionDraftByTokenHash', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getAdmissionDraftByTokenHash;
GO
CREATE PROCEDURE dbo.getAdmissionDraftByTokenHash
    @tokenHash NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 d.*
    FROM dbo.admission_resume_tokens t
    INNER JOIN dbo.admission_drafts d ON d.id = t.draft_id
    WHERE t.deleted = 0
      AND t.status = 1
      AND t.is_active = 1
      AND t.expires_at >= GETDATE()
      AND t.token_hash = @tokenHash
      AND d.deleted = 0
      AND d.status = 1
    ORDER BY t.id DESC;
END
GO

IF OBJECT_ID('dbo.createAdmissionResumeOtp', 'P') IS NOT NULL
    DROP PROCEDURE dbo.createAdmissionResumeOtp;
GO
CREATE PROCEDURE dbo.createAdmissionResumeOtp
    @draftId INT,
    @channel NVARCHAR(20),
    @destination NVARCHAR(255),
    @otpHash NVARCHAR(128),
    @validTill DATETIME,
    @createdById INT = NULL,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS(SELECT 1 FROM dbo.admission_drafts WHERE id=@draftId AND deleted=0 AND status=1)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Draft not found.';
            RETURN;
        END

        IF (@validTill IS NULL)
            SET @validTill = DATEADD(MINUTE, 10, GETDATE());

        INSERT INTO dbo.admission_resume_otps
        (
            creation_timestamp,
            created_by_id,
            draft_id,
            channel,
            destination,
            otp_hash,
            valid_till,
            verified,
            try_count,
            deleted,
            status
        )
        VALUES
        (
            GETDATE(),
            @createdById,
            @draftId,
            @channel,
            @destination,
            @otpHash,
            @validTill,
            0,
            0,
            0,
            1
        );

        SET @outputId = SCOPE_IDENTITY();
        SET @executionStatus = 'TRUE';
        SET @message = 'OTP created.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

IF OBJECT_ID('dbo.verifyAdmissionResumeOtp', 'P') IS NOT NULL
    DROP PROCEDURE dbo.verifyAdmissionResumeOtp;
GO
CREATE PROCEDURE dbo.verifyAdmissionResumeOtp
    @draftId INT,
    @channel NVARCHAR(20),
    @destination NVARCHAR(255),
    @otpHash NVARCHAR(128),
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @otpId INT;

        SELECT TOP 1 @otpId = id
        FROM dbo.admission_resume_otps
        WHERE draft_id = @draftId
          AND channel = @channel
          AND destination = @destination
          AND otp_hash = @otpHash
          AND verified = 0
          AND deleted = 0
          AND status = 1
          AND valid_till >= GETDATE()
        ORDER BY id DESC;

        IF (@otpId IS NULL)
        BEGIN
            -- increment try count on latest OTP for this channel/destination
            UPDATE dbo.admission_resume_otps
            SET try_count = try_count + 1
            WHERE id = (
                SELECT TOP 1 id
                FROM dbo.admission_resume_otps
                WHERE draft_id=@draftId AND channel=@channel AND destination=@destination AND deleted=0 AND status=1
                ORDER BY id DESC
            );

            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Invalid or expired OTP.';
            RETURN;
        END

        UPDATE dbo.admission_resume_otps
        SET verified = 1,
            verified_at = GETDATE()
        WHERE id = @otpId;

        SET @outputId = @otpId;
        SET @executionStatus = 'TRUE';
        SET @message = 'OTP verified.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO
