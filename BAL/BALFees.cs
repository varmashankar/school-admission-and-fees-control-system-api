using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.BAL
{
    public class BALFees
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALFees()
        {
            con = DBConnection.GlobalConnection();
        }

        private decimal GetInstallmentAmount(int installmentId, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand("SELECT amount FROM dbo.fee_installments WHERE id=@id AND deleted=0", con, tx))
            {
                cmd.Parameters.AddWithValue("@id", installmentId);
                var o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return 0m;
                return Convert.ToDecimal(o);
            }
        }

        private decimal GetPaidAmountForInstallment(int studentId, int installmentId, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand("SELECT ISNULL(SUM(paid_amount),0) FROM dbo.fee_due_log WHERE student_id=@sid AND installment_id=@iid AND deleted=0", con, tx))
            {
                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@iid", installmentId);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private DateTime? GetLastPaymentDateForInstallment(int studentId, int installmentId, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand("SELECT MAX(last_payment_date) FROM dbo.fee_due_log WHERE student_id=@sid AND installment_id=@iid AND deleted=0", con, tx))
            {
                cmd.Parameters.AddWithValue("@sid", studentId);
                cmd.Parameters.AddWithValue("@iid", installmentId);
                var o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return null;
                return Convert.ToDateTime(o);
            }
        }

        private int InsertFeePayment(FeePaymentRequest data, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO dbo.fee_payments
(creation_timestamp, created_by_id, student_id, payment_date, amount_paid, payment_mode, transaction_no, receipt_no, deleted, deleted_by_id, deleted_timestamp, status, academic_year_id)
VALUES
(GETDATE(), @created_by_id, @student_id, @payment_date, @amount_paid, @payment_mode, @transaction_no, NULL, 0, NULL, NULL, 1, @academic_year_id);
SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
            {
                cmd.Parameters.AddWithValue("@created_by_id", (object)data.created_by_id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@student_id", (object)data.student_id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@payment_date", (object)data.payment_date ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@amount_paid", (object)data.amount_paid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@payment_mode", (object)data.payment_mode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@transaction_no", (object)data.transaction_no ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@academic_year_id", (object)data.academic_year_id ?? DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void InsertFeeDueLog(int studentId, int installmentId, decimal dueAmount, decimal paidAmount, string status, DateTime? lastPaymentDate, int? deletedById, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO dbo.fee_due_log
(creation_timestamp, student_id, installment_id, due_amount, paid_amount, status, last_payment_date, deleted, deleted_by_id, deleted_timestamp)
VALUES
(GETDATE(), @student_id, @installment_id, @due_amount, @paid_amount, @status, @last_payment_date, 0, @deleted_by_id, NULL);", con, tx))
            {
                cmd.Parameters.AddWithValue("@student_id", studentId);
                cmd.Parameters.AddWithValue("@installment_id", installmentId);
                cmd.Parameters.AddWithValue("@due_amount", dueAmount);
                cmd.Parameters.AddWithValue("@paid_amount", paidAmount);
                cmd.Parameters.AddWithValue("@status", (object)status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@last_payment_date", (object)lastPaymentDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@deleted_by_id", (object)deletedById ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        private string GenerateReceiptNumber(int feePaymentId, int? createdById, SqlTransaction tx)
        {
            using (var cmd = new SqlCommand("sp_generate_fee_receipt_number", con, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fee_payment_id", feePaymentId);
                cmd.Parameters.AddWithValue("@created_by_id", (object)createdById ?? DBNull.Value);

                var outReceipt = new SqlParameter("@out_receipt_number", SqlDbType.VarChar, 200);
                outReceipt.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outReceipt);

                cmd.ExecuteNonQuery();

                return outReceipt.Value == null || outReceipt.Value == DBNull.Value
                    ? null
                    : Convert.ToString(outReceipt.Value);
            }
        }

        public FeePaymentResult recordPartialPayment(FeePaymentRequest data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (!data.student_id.HasValue) throw new ArgumentException("student_id is required");
            if (!data.installment_id.HasValue) throw new ArgumentException("installment_id is required");
            if (!data.amount_paid.HasValue) throw new ArgumentException("amount_paid is required");
            if (data.amount_paid.Value <= 0) throw new ArgumentException("amount_paid must be > 0");

            if (!data.payment_date.HasValue)
                data.payment_date = DateTime.Today;

            if (con.State != ConnectionState.Open)
                con.Open();

            using (var tx = con.BeginTransaction())
            {
                try
                {
                    int studentId = data.student_id.Value;
                    int installmentId = data.installment_id.Value;

                    decimal dueAmount = GetInstallmentAmount(installmentId, tx);
                    if (dueAmount <= 0)
                        throw new InvalidOperationException("Invalid installment or amount not found.");

                    decimal paidBefore = GetPaidAmountForInstallment(studentId, installmentId, tx);
                    decimal balanceBefore = dueAmount - paidBefore;

                    if (balanceBefore <= 0)
                        throw new InvalidOperationException("Installment is already fully paid.");

                    if (data.amount_paid.Value > balanceBefore)
                        throw new InvalidOperationException("Payment cannot exceed outstanding balance.");

                    int feePaymentId = InsertFeePayment(data, tx);

                    decimal paidAfter = paidBefore + data.amount_paid.Value;
                    decimal balanceAfter = dueAmount - paidAfter;

                    string dueStatus;
                    if (balanceAfter <= 0) dueStatus = "paid";
                    else if (paidAfter > 0) dueStatus = "partial";
                    else dueStatus = "pending";

                    InsertFeeDueLog(
                        studentId: studentId,
                        installmentId: installmentId,
                        dueAmount: dueAmount,
                        paidAmount: data.amount_paid.Value,
                        status: dueStatus,
                        lastPaymentDate: data.payment_date.Value.Date,
                        deletedById: null,
                        tx: tx);

                    string receiptNumber = GenerateReceiptNumber(feePaymentId, data.created_by_id, tx);

                    using (var cmd = new SqlCommand("UPDATE dbo.fee_payments SET receipt_no=@r WHERE id=@id", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@r", (object)receiptNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", feePaymentId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();

                    return new FeePaymentResult
                    {
                        fee_payment_id = feePaymentId,
                        receipt_number = receiptNumber,
                        payment_date = data.payment_date,
                        amount_paid = data.amount_paid.Value,
                        due_amount_before = dueAmount,
                        paid_amount_before = paidBefore,
                        due_amount_after = dueAmount,
                        paid_amount_after = paidAfter,
                        due_status_after = dueStatus
                    };
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
                finally
                {
                    try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
                }
            }
        }

        public List<FeePendingInstallment> getPendingFeesByStudent(int studentId)
        {
            Adp = new SqlDataAdapter("sp_calculate_pending_fee", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            Adp.SelectCommand.Parameters.AddWithValue("@student_id", studentId);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<FeePendingInstallment>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    var element = Function.BindData<FeePendingInstallment>(Dt.Rows[i], columns);
                    list.Add(element);
                }
            }

            return list;
        }

        public OutstandingFeesReport getOutstandingFeesReport(OutstandingFeesFilter filter)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                var sql = @"
SELECT
    s.id AS student_id,
    s.student_code,
    (RTRIM(LTRIM(ISNULL(s.first_name,''))) + ' ' + RTRIM(LTRIM(ISNULL(s.last_name,'')))) AS student_name,
    c.id AS class_id,
    (c.class_name + CASE WHEN ISNULL(c.section,'')='' THEN '' ELSE ' - ' + c.section END) AS class_name,
    fi.id AS installment_id,
    fi.installment_no,
    fi.due_date,
    fi.amount AS due_amount,
    ISNULL(paid.paid_amount, 0) AS paid_amount,
    (fi.amount - ISNULL(paid.paid_amount, 0)) AS outstanding_amount,
    CASE WHEN fi.due_date < CAST(GETDATE() AS date) THEN DATEDIFF(DAY, fi.due_date, CAST(GETDATE() AS date)) ELSE 0 END AS days_overdue
FROM dbo.students s
INNER JOIN dbo.classes c ON c.id = s.class_id
INNER JOIN dbo.fee_structures fs ON fs.class_id = c.id AND fs.deleted=0 AND fs.status=1
INNER JOIN dbo.fee_installments fi ON fi.fee_structure_id = fs.id AND fi.deleted=0 AND fi.status=1
OUTER APPLY (
    SELECT SUM(fdl.paid_amount) AS paid_amount
    FROM dbo.fee_due_log fdl
    WHERE fdl.deleted=0 AND fdl.student_id = s.id AND fdl.installment_id = fi.id
) paid
WHERE
    s.deleted=0 AND s.status=1
    AND (fi.amount - ISNULL(paid.paid_amount,0)) > 0
";

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;

                    if (filter != null)
                    {
                        if (filter.studentId.HasValue)
                        {
                            sql += " AND s.id = @studentId";
                            cmd.Parameters.AddWithValue("@studentId", filter.studentId.Value);
                        }

                        if (filter.classId.HasValue)
                        {
                            sql += " AND c.id = @classId";
                            cmd.Parameters.AddWithValue("@classId", filter.classId.Value);
                        }

                        if (filter.academicYearId.HasValue)
                        {
                            sql += " AND fs.academic_year_id = @ay";
                            sql += " AND (s.academic_year_id = @ay OR s.academic_year_id IS NULL)";
                            cmd.Parameters.AddWithValue("@ay", filter.academicYearId.Value);
                        }

                        if (filter.dueFrom.HasValue)
                        {
                            sql += " AND fi.due_date >= @dueFrom";
                            cmd.Parameters.AddWithValue("@dueFrom", filter.dueFrom.Value.Date);
                        }

                        if (filter.dueTo.HasValue)
                        {
                            sql += " AND fi.due_date <= @dueTo";
                            cmd.Parameters.AddWithValue("@dueTo", filter.dueTo.Value.Date);
                        }

                        if (filter.onlyOverdue.HasValue && filter.onlyOverdue.Value)
                        {
                            sql += " AND fi.due_date < CAST(GETDATE() AS date)";
                        }
                    }

                    sql += " ORDER BY c.id, s.id, fi.due_date";

                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    Dt = new DataTable();
                    using (var adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(Dt);
                    }
                }

                var items = new List<OutstandingFeeItem>();
                decimal totalDue = 0m;
                decimal totalPaid = 0m;
                decimal totalOutstanding = 0m;

                var studentSet = new HashSet<int>();

                foreach (DataRow row in Dt.Rows)
                {
                    var item = new OutstandingFeeItem
                    {
                        student_id = Convert.ToInt32(row["student_id"]),
                        student_code = Convert.ToString(row["student_code"]),
                        student_name = Convert.ToString(row["student_name"]),
                        class_id = Convert.ToInt32(row["class_id"]),
                        class_name = Convert.ToString(row["class_name"]),
                        installment_id = Convert.ToInt32(row["installment_id"]),
                        installment_no = Convert.ToInt32(row["installment_no"]),
                        due_date = Convert.ToDateTime(row["due_date"]),
                        due_amount = Convert.ToDecimal(row["due_amount"]),
                        paid_amount = Convert.ToDecimal(row["paid_amount"]),
                        outstanding_amount = Convert.ToDecimal(row["outstanding_amount"]),
                        days_overdue = Convert.ToInt32(row["days_overdue"])
                    };

                    items.Add(item);

                    totalDue += item.due_amount;
                    totalPaid += item.paid_amount;
                    totalOutstanding += item.outstanding_amount;
                    studentSet.Add(item.student_id);
                }

                return new OutstandingFeesReport
                {
                    total_due = totalDue,
                    total_paid = totalPaid,
                    total_outstanding = totalOutstanding,
                    total_students = studentSet.Count,
                    total_installments = items.Count,
                    items = items
                };
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public int queueFeeWhatsappReminders(FeeReminderQueueFilter filter)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                using (var cmd = new SqlCommand("sp_queue_fee_whatsapp_reminders", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@class_id", (object)(filter != null ? filter.classId : null) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@academic_year_id", (object)(filter != null ? filter.academicYearId : null) ?? DBNull.Value);

                    DateTime? dueBefore = (filter != null ? filter.dueBefore : null);
                    cmd.Parameters.AddWithValue("@due_before", (object)(dueBefore.HasValue ? dueBefore.Value.Date : (DateTime?)null) ?? DBNull.Value);

                    bool onlyOverdue = filter != null && filter.onlyOverdue.HasValue ? filter.onlyOverdue.Value : true;
                    cmd.Parameters.AddWithValue("@only_overdue", onlyOverdue);

                    int maxRecipients = filter != null && filter.maxRecipients.HasValue ? filter.maxRecipients.Value : 200;
                    cmd.Parameters.AddWithValue("@max_recipients", maxRecipients);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var o = reader[0];
                            if (o == null || o == DBNull.Value) return 0;
                            return Convert.ToInt32(o);
                        }
                    }

                    return 0;
                }
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public List<FeeReminderQueueItem> getFeeWhatsappRemindersToSend(int maxBatch)
        {
            Adp = new SqlDataAdapter("sp_get_fee_whatsapp_reminders_to_send", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            Adp.SelectCommand.Parameters.AddWithValue("@max_batch", maxBatch);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<FeeReminderQueueItem>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    var element = Function.BindData<FeeReminderQueueItem>(Dt.Rows[i], columns);
                    list.Add(element);
                }
            }

            return list;
        }

        public int markFeeWhatsappReminderSent(int id, string providerMessageId)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                using (var cmd = new SqlCommand("sp_mark_fee_whatsapp_reminder_sent", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@provider_message_id", (object)providerMessageId ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var o = reader[0];
                            if (o == null || o == DBNull.Value) return 0;
                            return Convert.ToInt32(o);
                        }
                    }
                    return 0;
                }
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public int markFeeWhatsappReminderFailed(int id, string error)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                using (var cmd = new SqlCommand("sp_mark_fee_whatsapp_reminder_failed", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@error", (object)error ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var o = reader[0];
                            if (o == null || o == DBNull.Value) return 0;
                            return Convert.ToInt32(o);
                        }
                    }
                    return 0;
                }
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public List<FeeHead> getFeeHeadList(FeeHeadFilter filter)
        {
            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                var sql = @"
SELECT id, creation_timestamp, created_by_id, name, code, frequency, default_amount,
       deleted, deleted_by_id, deleted_timestamp, status
FROM dbo.fee_heads
WHERE 1=1";

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = con;

                    if (filter != null)
                    {
                        if (filter.id.HasValue)
                        {
                            sql += " AND id=@id";
                            cmd.Parameters.AddWithValue("@id", filter.id.Value);
                        }

                        if (filter.deleted.HasValue)
                        {
                            sql += " AND ISNULL(deleted,0)=@deleted";
                            cmd.Parameters.AddWithValue("@deleted", filter.deleted.Value ? 1 : 0);
                        }
                        else
                        {
                            sql += " AND ISNULL(deleted,0)=0";
                        }

                        if (filter.status.HasValue)
                        {
                            sql += " AND ISNULL(status,1)=@status";
                            cmd.Parameters.AddWithValue("@status", filter.status.Value ? 1 : 0);
                        }
                    }
                    else
                    {
                        sql += " AND ISNULL(deleted,0)=0";
                    }

                    sql += " ORDER BY name ASC";

                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    Dt = new DataTable();
                    using (var adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(Dt);
                    }
                }

                var list = new List<FeeHead>();
                foreach (DataRow row in Dt.Rows)
                {
                    list.Add(new FeeHead
                    {
                        id = Convert.ToInt32(row["id"]),
                        creation_timestamp = row["creation_timestamp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["creation_timestamp"]),
                        created_by_id = Convert.ToInt32(row["created_by_id"]),
                        name = Convert.ToString(row["name"]),
                        code = row["code"] == DBNull.Value ? null : Convert.ToString(row["code"]),
                        frequency = row["frequency"] == DBNull.Value ? null : Convert.ToString(row["frequency"]),
                        default_amount = row["default_amount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["default_amount"]),
                        deleted = row["deleted"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["deleted"]),
                        deleted_by_id = row["deleted_by_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["deleted_by_id"]),
                        deleted_timestamp = row["deleted_timestamp"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["deleted_timestamp"]),
                        status = row["status"] == DBNull.Value ? (bool?)null : Convert.ToBoolean(row["status"])
                    });
                }

                return list;
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public int saveFeeHead(SaveFeeHeadRequest data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (string.IsNullOrWhiteSpace(data.name)) throw new ArgumentException("name is required");
            if (!data.default_amount.HasValue) data.default_amount = 0m;

            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                if (!data.id.HasValue)
                {
                    using (var cmd = new SqlCommand(@"
INSERT INTO dbo.fee_heads
(creation_timestamp, created_by_id, name, code, frequency, default_amount, deleted, deleted_by_id, deleted_timestamp, status)
VALUES
(GETDATE(), @created_by_id, @name, @code, @frequency, @default_amount, 0, NULL, NULL, @status);
SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
                    {
                        cmd.Parameters.AddWithValue("@created_by_id", (object)data.created_by_id ?? 0);
                        cmd.Parameters.AddWithValue("@name", data.name.Trim());
                        cmd.Parameters.AddWithValue("@code", (object)(string.IsNullOrWhiteSpace(data.code) ? null : data.code.Trim()) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@frequency", (object)(string.IsNullOrWhiteSpace(data.frequency) ? null : data.frequency.Trim()) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@default_amount", data.default_amount.Value);
                        cmd.Parameters.AddWithValue("@status", (data.status.HasValue ? (data.status.Value ? 1 : 0) : 1));
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                using (var cmd = new SqlCommand(@"
UPDATE dbo.fee_heads
SET name=@name,
    code=@code,
    frequency=@frequency,
    default_amount=@default_amount,
    status=@status
WHERE id=@id AND ISNULL(deleted,0)=0;
SELECT @@ROWCOUNT;", con))
                {
                    cmd.Parameters.AddWithValue("@id", data.id.Value);
                    cmd.Parameters.AddWithValue("@name", data.name.Trim());
                    cmd.Parameters.AddWithValue("@code", (object)(string.IsNullOrWhiteSpace(data.code) ? null : data.code.Trim()) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@frequency", (object)(string.IsNullOrWhiteSpace(data.frequency) ? null : data.frequency.Trim()) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@default_amount", data.default_amount.Value);
                    cmd.Parameters.AddWithValue("@status", (data.status.HasValue ? (data.status.Value ? 1 : 0) : 1));

                    var affected = Convert.ToInt32(cmd.ExecuteScalar());
                    return affected;
                }
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }

        public int deleteFeeHead(DeleteFeeHeadRequest data)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (!data.id.HasValue) throw new ArgumentException("id is required");

            if (con.State != ConnectionState.Open)
                con.Open();

            try
            {
                using (var cmd = new SqlCommand(@"
UPDATE dbo.fee_heads
SET deleted=1,
    deleted_by_id=@deleted_by_id,
    deleted_timestamp=GETDATE(),
    status=0
WHERE id=@id AND ISNULL(deleted,0)=0;
SELECT @@ROWCOUNT;", con))
                {
                    cmd.Parameters.AddWithValue("@id", data.id.Value);
                    cmd.Parameters.AddWithValue("@deleted_by_id", (object)data.deleted_by_id ?? DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                try { if (con.State == ConnectionState.Open) con.Close(); } catch { }
            }
        }
    }
}
