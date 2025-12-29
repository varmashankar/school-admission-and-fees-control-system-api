-- Fee WhatsApp reminder queue objects

IF OBJECT_ID('dbo.fee_whatsapp_reminder_queue', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.fee_whatsapp_reminder_queue (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        creation_timestamp DATETIME NOT NULL DEFAULT(GETDATE()),
        student_id INT NOT NULL,
        installment_id INT NOT NULL,
        due_date DATE NULL,
        outstanding_amount DECIMAL(12,2) NOT NULL,
        phone NVARCHAR(20) NULL,
        message NVARCHAR(1000) NOT NULL,
        status VARCHAR(20) NOT NULL DEFAULT('queued'), -- queued|sent|failed|cancelled
        try_count INT NOT NULL DEFAULT(0),
        last_try_at DATETIME NULL,
        sent_at DATETIME NULL,
        provider_message_id NVARCHAR(200) NULL,
        last_error NVARCHAR(1000) NULL
    );

    CREATE NONCLUSTERED INDEX ix_fee_whatsapp_queue_status_created
        ON dbo.fee_whatsapp_reminder_queue(status, creation_timestamp);
END
GO

IF OBJECT_ID('dbo.sp_queue_fee_whatsapp_reminders', 'P') IS NULL
EXEC('CREATE PROCEDURE dbo.sp_queue_fee_whatsapp_reminders AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_queue_fee_whatsapp_reminders
    @class_id INT = NULL,
    @academic_year_id INT = NULL,
    @due_before DATE = NULL,
    @only_overdue BIT = 1,
    @created_by_id INT = NULL,
    @max_recipients INT = 200
AS
BEGIN
    SET NOCOUNT ON;

    IF @due_before IS NULL
        SET @due_before = CAST(GETDATE() AS DATE);

    ;WITH due_cte AS (
        SELECT TOP (@max_recipients)
            s.id AS student_id,
            fi.id AS installment_id,
            fi.due_date,
            fi.amount AS due_amount,
            ISNULL(paid.paid_amount, 0) AS paid_amount,
            (fi.amount - ISNULL(paid.paid_amount,0)) AS outstanding_amount,
            s.phone,
            (RTRIM(LTRIM(ISNULL(s.first_name,''))) + ' ' + RTRIM(LTRIM(ISNULL(s.last_name,'')))) AS student_name
        FROM dbo.students s
        INNER JOIN dbo.classes c ON c.id = s.class_id
        INNER JOIN dbo.fee_structures fs ON fs.class_id = c.id AND fs.deleted=0 AND fs.status=1
        INNER JOIN dbo.fee_installments fi ON fi.fee_structure_id = fs.id AND fi.deleted=0 AND fi.status=1
        OUTER APPLY (
            SELECT SUM(fdl.paid_amount) AS paid_amount
            FROM dbo.fee_due_log fdl
            WHERE fdl.deleted=0 AND fdl.student_id=s.id AND fdl.installment_id=fi.id
        ) paid
        WHERE s.deleted=0 AND s.status=1
          AND (fi.amount - ISNULL(paid.paid_amount,0)) > 0
          AND fi.due_date <= @due_before
          AND (@class_id IS NULL OR c.id=@class_id)
          AND (@academic_year_id IS NULL OR fs.academic_year_id=@academic_year_id)
          AND (@only_overdue = 0 OR fi.due_date < CAST(GETDATE() AS DATE))
          AND NOT EXISTS (
              SELECT 1
              FROM dbo.fee_whatsapp_reminder_queue q
              WHERE q.status IN ('queued','sent')
                AND q.student_id = s.id
                AND q.installment_id = fi.id
          )
        ORDER BY fi.due_date ASC
    )
    INSERT INTO dbo.fee_whatsapp_reminder_queue
    (student_id, installment_id, due_date, outstanding_amount, phone, message, status)
    SELECT
        d.student_id,
        d.installment_id,
        d.due_date,
        d.outstanding_amount,
        d.phone,
        CONCAT('Fee due reminder: ', d.student_name, ', outstanding: ', FORMAT(d.outstanding_amount,'N2'), ', due date: ', CONVERT(VARCHAR(10), d.due_date, 120)),
        'queued'
    FROM due_cte d;

    SELECT @@ROWCOUNT AS queued_count;
END
GO

IF OBJECT_ID('dbo.sp_get_fee_whatsapp_reminders_to_send', 'P') IS NULL
EXEC('CREATE PROCEDURE dbo.sp_get_fee_whatsapp_reminders_to_send AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_get_fee_whatsapp_reminders_to_send
    @max_batch INT = 25
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@max_batch)
        id,
        student_id,
        installment_id,
        due_date,
        outstanding_amount,
        phone,
        message,
        status,
        try_count,
        last_try_at
    FROM dbo.fee_whatsapp_reminder_queue
    WHERE status='queued'
    ORDER BY creation_timestamp ASC;
END
GO

IF OBJECT_ID('dbo.sp_mark_fee_whatsapp_reminder_sent', 'P') IS NULL
EXEC('CREATE PROCEDURE dbo.sp_mark_fee_whatsapp_reminder_sent AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_mark_fee_whatsapp_reminder_sent
    @id INT,
    @provider_message_id NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.fee_whatsapp_reminder_queue
    SET status='sent',
        sent_at=GETDATE(),
        provider_message_id=@provider_message_id,
        last_error=NULL
    WHERE id=@id;

    SELECT @@ROWCOUNT AS affected;
END
GO

IF OBJECT_ID('dbo.sp_mark_fee_whatsapp_reminder_failed', 'P') IS NULL
EXEC('CREATE PROCEDURE dbo.sp_mark_fee_whatsapp_reminder_failed AS BEGIN SET NOCOUNT ON; END');
GO

ALTER PROCEDURE dbo.sp_mark_fee_whatsapp_reminder_failed
    @id INT,
    @error NVARCHAR(1000)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.fee_whatsapp_reminder_queue
    SET status='failed',
        try_count = try_count + 1,
        last_try_at = GETDATE(),
        last_error = @error
    WHERE id=@id;

    SELECT @@ROWCOUNT AS affected;
END
GO
