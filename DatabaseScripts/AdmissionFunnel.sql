SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/*
Admission Funnel - schema + stored procedures (school_erp conventions)

Conventions aligned to SchoolERP/DB/schoolfullerp.sql:
- snake_case table/column names
- soft delete via `deleted`, `deleted_by_id`, `deleted_timestamp`
- enable/disable via `status` (bit)
- audit via `creation_timestamp`, `created_by_id`
- stored procedures return @executionStatus/@message pattern
*/

/* =========================================================
   TABLES
   ========================================================= */

IF OBJECT_ID('dbo.inquiries', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.inquiries
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_inquiries_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        inquiry_no NVARCHAR(50) NULL,

        first_name NVARCHAR(100) NULL,
        last_name NVARCHAR(100) NULL,

        phone NVARCHAR(30) NULL,
        email NVARCHAR(255) NULL,

        class_id INT NULL,
        stream_id INT NULL,

        source NVARCHAR(100) NULL,
        notes NVARCHAR(MAX) NULL,

        inquiry_status NVARCHAR(50) NOT NULL CONSTRAINT DF_inquiries_inquiry_status DEFAULT('NEW'),
        next_follow_up_at DATETIME NULL,

        converted_student_id INT NULL,

        deleted BIT NOT NULL CONSTRAINT DF_inquiries_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_inquiries_status DEFAULT(1)
    );

    CREATE INDEX ix_inquiries_status ON dbo.inquiries(inquiry_status);
    CREATE INDEX ix_inquiries_class_stream ON dbo.inquiries(class_id, stream_id);
    CREATE INDEX ix_inquiries_next_follow_up_at ON dbo.inquiries(next_follow_up_at);
    CREATE INDEX ix_inquiries_creation_timestamp ON dbo.inquiries(creation_timestamp);
END
GO

IF OBJECT_ID('dbo.inquiry_follow_ups', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.inquiry_follow_ups
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_inquiry_follow_ups_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        inquiry_id INT NOT NULL,

        follow_up_at DATETIME NULL,
        channel NVARCHAR(50) NULL,
        remarks NVARCHAR(MAX) NULL,

        is_reminded BIT NOT NULL CONSTRAINT DF_inquiry_follow_ups_is_reminded DEFAULT(0),
        reminded_at DATETIME NULL,

        deleted BIT NOT NULL CONSTRAINT DF_inquiry_follow_ups_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_inquiry_follow_ups_status DEFAULT(1)
    );

    ALTER TABLE dbo.inquiry_follow_ups WITH CHECK
    ADD CONSTRAINT fk_inquiry_follow_ups_inquiries
        FOREIGN KEY (inquiry_id) REFERENCES dbo.inquiries(id);

    CREATE INDEX ix_inquiry_follow_ups_due ON dbo.inquiry_follow_ups(is_reminded, follow_up_at);
END
GO

IF OBJECT_ID('dbo.inquiry_status_history', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.inquiry_status_history
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL CONSTRAINT DF_inquiry_status_history_creation_timestamp DEFAULT(GETDATE()),
        created_by_id INT NULL,

        inquiry_id INT NOT NULL,

        from_status NVARCHAR(50) NULL,
        to_status NVARCHAR(50) NOT NULL,
        remarks NVARCHAR(MAX) NULL,

        deleted BIT NOT NULL CONSTRAINT DF_inquiry_status_history_deleted DEFAULT(0),
        deleted_by_id INT NULL,
        deleted_timestamp DATETIME NULL,

        status BIT NOT NULL CONSTRAINT DF_inquiry_status_history_status DEFAULT(1)
    );

    ALTER TABLE dbo.inquiry_status_history WITH CHECK
    ADD CONSTRAINT fk_inquiry_status_history_inquiries
        FOREIGN KEY (inquiry_id) REFERENCES dbo.inquiries(id);

    CREATE INDEX ix_inquiry_status_history_inquiry_id ON dbo.inquiry_status_history(inquiry_id, creation_timestamp);
END
GO

/* =========================================================
   STORED PROCEDURES
   ========================================================= */

-- generateInquiryId
IF OBJECT_ID('dbo.generateInquiryId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.generateInquiryId;
GO
CREATE PROCEDURE dbo.generateInquiryId
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @year CHAR(4) = YEAR(GETDATE());
    DECLARE @prefix VARCHAR(20) = 'INQ-' + @year + '-';
    DECLARE @nextId INT;

    SELECT @nextId =
        ISNULL(
            MAX(
                CASE 
                    WHEN inquiry_no LIKE @prefix + '%'
                    THEN TRY_CAST(PARSENAME(REPLACE(inquiry_no, '-', '.'), 1) AS INT)
                    ELSE 0
                END
            ), 0
        ) + 1
    FROM dbo.inquiries;

    SELECT @prefix + RIGHT('0000' + CAST(@nextId AS VARCHAR(10)), 4) AS InquiryId;
END
GO

-- saveInquiry
IF OBJECT_ID('dbo.saveInquiry', 'P') IS NOT NULL
    DROP PROCEDURE dbo.saveInquiry;
GO
CREATE PROCEDURE dbo.saveInquiry
    @id INT = NULL,
    @creationTimestamp DATETIME = NULL,
    @createdById INT = NULL,
    @roleId INT = NULL,

    @inquiryNo NVARCHAR(50) = NULL,
    @firstName NVARCHAR(100) = NULL,
    @lastName NVARCHAR(100) = NULL,
    @phone NVARCHAR(30) = NULL,
    @email NVARCHAR(255) = NULL,
    @classId INT = NULL,
    @streamId INT = NULL,
    @source NVARCHAR(100) = NULL,
    @notes NVARCHAR(MAX) = NULL,
    @status NVARCHAR(50) = NULL,
    @nextFollowUpAt DATETIME = NULL,
    @convertedStudentId INT = NULL,

    @deleted BIT = NULL,
    @deletedById INT = NULL,
    @deletedTimestamp DATETIME = NULL,
    @recordStatus BIT = NULL,

    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@creationTimestamp IS NULL) SET @creationTimestamp = GETDATE();
        IF (@status IS NULL OR LTRIM(RTRIM(@status)) = '') SET @status = 'NEW';

        -- Auto-generate inquiry_no if not provided
        IF (@inquiryNo IS NULL OR LTRIM(RTRIM(@inquiryNo)) = '')
        BEGIN
            DECLARE @genNo NVARCHAR(50);
            DECLARE @year CHAR(4) = YEAR(GETDATE());
            DECLARE @prefix VARCHAR(20) = 'INQ-' + @year + '-';
            DECLARE @nextId INT;

            SELECT @nextId =
                ISNULL(
                    MAX(
                        CASE 
                            WHEN inquiry_no LIKE @prefix + '%'
                            THEN TRY_CAST(PARSENAME(REPLACE(inquiry_no, '-', '.'), 1) AS INT)
                            ELSE 0
                        END
                    ), 0
                ) + 1
            FROM dbo.inquiries;

            SET @genNo = @prefix + RIGHT('0000' + CAST(@nextId AS VARCHAR(10)), 4);
            SET @inquiryNo = @genNo;
        END

        IF (@id IS NULL)
        BEGIN
            INSERT INTO dbo.inquiries
            (
                creation_timestamp, created_by_id,
                inquiry_no,
                first_name, last_name,
                phone, email,
                class_id, stream_id,
                source, notes,
                inquiry_status, next_follow_up_at,
                converted_student_id,
                deleted, deleted_by_id, deleted_timestamp,
                status
            )
            VALUES
            (
                @creationTimestamp, @createdById,
                @inquiryNo,
                @firstName, @lastName,
                @phone, @email,
                @classId, @streamId,
                @source, @notes,
                @status, @nextFollowUpAt,
                @convertedStudentId,
                ISNULL(@deleted,0), @deletedById, @deletedTimestamp,
                ISNULL(@recordStatus,1)
            );

            SET @outputId = SCOPE_IDENTITY();
            SET @executionStatus = 'TRUE';
            SET @message = 'Inquiry saved successfully.';
            RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM dbo.inquiries WHERE id=@id AND deleted=0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Inquiry not found.';
            RETURN;
        END

        UPDATE dbo.inquiries
        SET inquiry_no = ISNULL(@inquiryNo, inquiry_no),
            first_name = @firstName,
            last_name = @lastName,
            phone = @phone,
            email = @email,
            class_id = @classId,
            stream_id = @streamId,
            source = @source,
            notes = @notes,
            inquiry_status = ISNULL(@status, inquiry_status),
            next_follow_up_at = @nextFollowUpAt,
            converted_student_id = @convertedStudentId,
            deleted = ISNULL(@deleted, deleted),
            deleted_by_id = @deletedById,
            deleted_timestamp = @deletedTimestamp,
            status = ISNULL(@recordStatus, status)
        WHERE id = @id;

        SET @outputId = @id;
        SET @executionStatus = 'TRUE';
        SET @message = 'Inquiry updated successfully.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

-- getInquiryList
IF OBJECT_ID('dbo.getInquiryList', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getInquiryList;
GO
CREATE PROCEDURE dbo.getInquiryList
    @id INT = NULL,
    @inquiryNo NVARCHAR(50) = NULL,
    @status NVARCHAR(50) = NULL,
    @classId INT = NULL,
    @streamId INT = NULL,
    @fromDate DATETIME = NULL,
    @toDate DATETIME = NULL,
    @includeConverted BIT = NULL,
    @pageNo INT = NULL,
    @pageSize INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (@pageNo IS NULL OR @pageNo <= 0) SET @pageNo = 1;
    IF (@pageSize IS NULL OR @pageSize <= 0) SET @pageSize = 50;

    ;WITH Q AS
    (
        SELECT i.*
        FROM dbo.inquiries i
        WHERE i.deleted = 0
          AND i.status = 1
          AND (@id IS NULL OR i.id = @id)
          AND (@inquiryNo IS NULL OR i.inquiry_no = @inquiryNo)
          AND (@status IS NULL OR i.inquiry_status = @status)
          AND (@classId IS NULL OR i.class_id = @classId)
          AND (@streamId IS NULL OR i.stream_id = @streamId)
          AND (@fromDate IS NULL OR i.creation_timestamp >= @fromDate)
          AND (@toDate IS NULL OR i.creation_timestamp < DATEADD(DAY, 1, @toDate))
          AND (
                @includeConverted IS NULL
                OR (@includeConverted = 1)
                OR (@includeConverted = 0 AND i.converted_student_id IS NULL)
              )
    )
    SELECT *
    FROM Q
    ORDER BY id DESC
    OFFSET (@pageNo - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END
GO

-- changeInquiryStatus
IF OBJECT_ID('dbo.changeInquiryStatus', 'P') IS NOT NULL
    DROP PROCEDURE dbo.changeInquiryStatus;
GO
CREATE PROCEDURE dbo.changeInquiryStatus
    @id INT,
    @status NVARCHAR(50),
    @userId INT = NULL,
    @roleTypeId INT = NULL,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @fromStatus NVARCHAR(50);
        SELECT @fromStatus = inquiry_status FROM dbo.inquiries WHERE id = @id AND deleted=0;

        IF (@fromStatus IS NULL)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Inquiry not found.';
            RETURN;
        END

        UPDATE dbo.inquiries
        SET inquiry_status = @status
        WHERE id = @id;

        INSERT INTO dbo.inquiry_status_history
        (
            creation_timestamp,
            created_by_id,
            inquiry_id,
            from_status,
            to_status,
            remarks,
            deleted,
            deleted_by_id,
            deleted_timestamp,
            status
        )
        VALUES
        (
            GETDATE(),
            @userId,
            @id,
            @fromStatus,
            @status,
            NULL,
            0,
            NULL,
            NULL,
            1
        );

        SET @outputId = @id;
        SET @executionStatus = 'TRUE';
        SET @message = 'Inquiry status updated successfully.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

-- saveInquiryFollowUp
IF OBJECT_ID('dbo.saveInquiryFollowUp', 'P') IS NOT NULL
    DROP PROCEDURE dbo.saveInquiryFollowUp;
GO
CREATE PROCEDURE dbo.saveInquiryFollowUp
    @id INT = NULL,
    @creationTimestamp DATETIME = NULL,
    @createdById INT = NULL,
    @roleId INT = NULL,

    @inquiryId INT,
    @followUpAt DATETIME = NULL,
    @channel NVARCHAR(50) = NULL,
    @remarks NVARCHAR(MAX) = NULL,

    @isReminded BIT = NULL,
    @remindedAt DATETIME = NULL,

    @deleted BIT = NULL,
    @deletedById INT = NULL,
    @deletedTimestamp DATETIME = NULL,
    @recordStatus BIT = NULL,

    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@creationTimestamp IS NULL) SET @creationTimestamp = GETDATE();

        IF NOT EXISTS(SELECT 1 FROM dbo.inquiries WHERE id=@inquiryId AND deleted=0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Inquiry not found.';
            RETURN;
        END

        IF (@id IS NULL)
        BEGIN
            INSERT INTO dbo.inquiry_follow_ups
            (
                creation_timestamp,
                created_by_id,
                inquiry_id,
                follow_up_at,
                channel,
                remarks,
                is_reminded,
                reminded_at,
                deleted,
                deleted_by_id,
                deleted_timestamp,
                status
            )
            VALUES
            (
                @creationTimestamp,
                @createdById,
                @inquiryId,
                @followUpAt,
                @channel,
                @remarks,
                ISNULL(@isReminded,0),
                @remindedAt,
                ISNULL(@deleted,0),
                @deletedById,
                @deletedTimestamp,
                ISNULL(@recordStatus,1)
            );

            SET @outputId = SCOPE_IDENTITY();

            UPDATE dbo.inquiries
            SET next_follow_up_at = @followUpAt
            WHERE id = @inquiryId;

            SET @executionStatus = 'TRUE';
            SET @message = 'Follow-up saved successfully.';
            RETURN;
        END

        IF NOT EXISTS(SELECT 1 FROM dbo.inquiry_follow_ups WHERE id=@id AND deleted=0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Follow-up not found.';
            RETURN;
        END

        UPDATE dbo.inquiry_follow_ups
        SET follow_up_at = @followUpAt,
            channel = @channel,
            remarks = @remarks,
            is_reminded = ISNULL(@isReminded, is_reminded),
            reminded_at = @remindedAt,
            deleted = ISNULL(@deleted, deleted),
            deleted_by_id = @deletedById,
            deleted_timestamp = @deletedTimestamp,
            status = ISNULL(@recordStatus, status)
        WHERE id = @id;

        SET @outputId = @id;
        SET @executionStatus = 'TRUE';
        SET @message = 'Follow-up updated successfully.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

-- getDueInquiryFollowUps
IF OBJECT_ID('dbo.getDueInquiryFollowUps', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getDueInquiryFollowUps;
GO
CREATE PROCEDURE dbo.getDueInquiryFollowUps
    @dueBefore DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF (@dueBefore IS NULL) SET @dueBefore = GETDATE();

    SELECT f.*
    FROM dbo.inquiry_follow_ups f
    WHERE f.deleted = 0
      AND f.status = 1
      AND f.is_reminded = 0
      AND f.follow_up_at IS NOT NULL
      AND f.follow_up_at <= @dueBefore
    ORDER BY f.follow_up_at ASC;
END
GO

-- markInquiryFollowUpReminded
IF OBJECT_ID('dbo.markInquiryFollowUpReminded', 'P') IS NOT NULL
    DROP PROCEDURE dbo.markInquiryFollowUpReminded;
GO
CREATE PROCEDURE dbo.markInquiryFollowUpReminded
    @followUpId INT,
    @remindedAt DATETIME = NULL,
    @userId INT = NULL,
    @roleTypeId INT = NULL,
    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@remindedAt IS NULL) SET @remindedAt = GETDATE();

        UPDATE dbo.inquiry_follow_ups
        SET is_reminded = 1,
            reminded_at = ISNULL(reminded_at, @remindedAt)
        WHERE id = @followUpId
          AND deleted = 0
          AND status = 1
          AND is_reminded = 0;

        SET @outputId = @followUpId;
        SET @executionStatus = 'TRUE';
        SET @message = 'Follow-up marked as reminded.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

-- saveInquiryStatusHistory
IF OBJECT_ID('dbo.saveInquiryStatusHistory', 'P') IS NOT NULL
    DROP PROCEDURE dbo.saveInquiryStatusHistory;
GO
CREATE PROCEDURE dbo.saveInquiryStatusHistory
    @id INT = NULL,
    @creationTimestamp DATETIME = NULL,
    @changedById INT = NULL,
    @roleId INT = NULL,

    @inquiryId INT,
    @fromStatus NVARCHAR(50) = NULL,
    @toStatus NVARCHAR(50),
    @remarks NVARCHAR(MAX) = NULL,

    @deleted BIT = NULL,
    @deletedById INT = NULL,
    @deletedTimestamp DATETIME = NULL,
    @recordStatus BIT = NULL,

    @outputId INT OUTPUT,
    @executionStatus VARCHAR(10) OUTPUT,
    @message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF (@creationTimestamp IS NULL) SET @creationTimestamp = GETDATE();

        IF NOT EXISTS(SELECT 1 FROM dbo.inquiries WHERE id=@inquiryId AND deleted=0)
        BEGIN
            SET @outputId = 0;
            SET @executionStatus = 'FALSE';
            SET @message = 'Inquiry not found.';
            RETURN;
        END

        INSERT INTO dbo.inquiry_status_history
        (
            creation_timestamp,
            created_by_id,
            inquiry_id,
            from_status,
            to_status,
            remarks,
            deleted,
            deleted_by_id,
            deleted_timestamp,
            status
        )
        VALUES
        (
            @creationTimestamp,
            @changedById,
            @inquiryId,
            @fromStatus,
            @toStatus,
            @remarks,
            ISNULL(@deleted,0),
            @deletedById,
            @deletedTimestamp,
            ISNULL(@recordStatus,1)
        );

        SET @outputId = SCOPE_IDENTITY();
        SET @executionStatus = 'TRUE';
        SET @message = 'Inquiry status history saved.';
    END TRY
    BEGIN CATCH
        SET @outputId = 0;
        SET @executionStatus = 'FALSE';
        SET @message = ERROR_MESSAGE();
    END CATCH
END
GO

-- getInquiryStatusHistory
IF OBJECT_ID('dbo.getInquiryStatusHistory', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getInquiryStatusHistory;
GO
CREATE PROCEDURE dbo.getInquiryStatusHistory
    @inquiryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT h.*
    FROM dbo.inquiry_status_history h
    WHERE h.inquiry_id = @inquiryId
      AND h.deleted = 0
      AND h.status = 1
    ORDER BY h.creation_timestamp DESC, h.id DESC;
END
GO

-- getInquiryConversionReport
IF OBJECT_ID('dbo.getInquiryConversionReport', 'P') IS NOT NULL
    DROP PROCEDURE dbo.getInquiryConversionReport;
GO
CREATE PROCEDURE dbo.getInquiryConversionReport
    @fromDate DATETIME = NULL,
    @toDate DATETIME = NULL,
    @classId INT = NULL,
    @streamId INT = NULL,
    @source NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Q AS
    (
        SELECT
            i.source,
            i.class_id,
            i.stream_id,
            COUNT(1) AS totalInquiries,
            SUM(CASE WHEN i.converted_student_id IS NULL THEN 0 ELSE 1 END) AS convertedInquiries
        FROM dbo.inquiries i
        WHERE i.deleted = 0
          AND i.status = 1
          AND (@fromDate IS NULL OR i.creation_timestamp >= @fromDate)
          AND (@toDate IS NULL OR i.creation_timestamp < DATEADD(DAY, 1, @toDate))
          AND (@classId IS NULL OR i.class_id = @classId)
          AND (@streamId IS NULL OR i.stream_id = @streamId)
          AND (@source IS NULL OR i.source = @source)
        GROUP BY i.source, i.class_id, i.stream_id
    )
    SELECT
        Q.source,
        Q.totalInquiries,
        Q.convertedInquiries,
        CASE WHEN Q.totalInquiries = 0 THEN 0
             ELSE CAST(Q.convertedInquiries AS DECIMAL(18,4)) / CAST(Q.totalInquiries AS DECIMAL(18,4))
        END AS conversionRate,
        Q.class_id AS classId,
        NULL AS className,
        Q.stream_id AS streamId,
        NULL AS streamName,
        @fromDate AS fromDate,
        @toDate AS toDate
    FROM Q
    ORDER BY conversionRate DESC;
END
GO
