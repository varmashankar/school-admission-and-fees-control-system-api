using SchoolErpAPI.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SchoolErpAPI.BAL
{
    public class BALAdmissionDrafts
    {
        private readonly SqlConnection con;

        public BALAdmissionDrafts()
        {
            con = DBConnection.GlobalConnection();
        }

        public static string Sha256Hex(string input)
        {
            if (input == null) input = string.Empty;
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(bytes.Length * 2);
                for (int i = 0; i < bytes.Length; i++) sb.Append(bytes[i].ToString("x2"));
                return sb.ToString();
            }
        }

        public SPResponse createAdmissionDraft(int? studentId, string admissionNo)
        {
            var cmd = new SqlCommand("createAdmissionDraft", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@studentId", (object)studentId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@admissionNo", admissionNo);
            cmd.Parameters.AddWithValue("@createdById", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            return function.getDefaultSPOutput(cmd, con);
        }

        public SPResponse attachStudentToDraft(int draftId, int studentId)
        {
            var cmd = new SqlCommand("attachStudentToAdmissionDraft", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@draftId", draftId);
            cmd.Parameters.AddWithValue("@studentId", studentId);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            return function.getDefaultSPOutput(cmd, con);
        }

        public AdmissionDraft getDraftByAdmissionNo(string admissionNo)
        {
            using (var adp = new SqlDataAdapter("getAdmissionDraftByAdmissionNo", con))
            {
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@admissionNo", (object)admissionNo ?? DBNull.Value);

                var dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count == 0) return null;

                var cols = new System.Collections.Generic.List<string>();
                foreach (DataColumn dc in dt.Columns) cols.Add(dc.ColumnName);
                return Function.BindData<AdmissionDraft>(dt.Rows[0], cols);
            }
        }

        public AdmissionDraft getDraftByToken(string token)
        {
            var tokenHash = Sha256Hex(token);
            using (var adp = new SqlDataAdapter("getAdmissionDraftByTokenHash", con))
            {
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@tokenHash", tokenHash);

                var dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count == 0) return null;

                var cols = new System.Collections.Generic.List<string>();
                foreach (DataColumn dc in dt.Columns) cols.Add(dc.ColumnName);
                return Function.BindData<AdmissionDraft>(dt.Rows[0], cols);
            }
        }

        public SPResponse createResumeToken(int draftId, string rawToken, DateTime? expiresAt)
        {
            var tokenHash = Sha256Hex(rawToken);
            var cmd = new SqlCommand("createAdmissionResumeToken", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@draftId", draftId);
            cmd.Parameters.AddWithValue("@tokenHash", tokenHash);
            cmd.Parameters.AddWithValue("@expiresAt", (object)expiresAt ?? DateTime.Now.AddDays(30));
            cmd.Parameters.AddWithValue("@createdById", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            return function.getDefaultSPOutput(cmd, con);
        }

        public SPResponse createOtp(int draftId, string channel, string destination, string rawOtp, DateTime? validTill)
        {
            var otpHash = Sha256Hex(rawOtp);
            var cmd = new SqlCommand("createAdmissionResumeOtp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@draftId", draftId);
            cmd.Parameters.AddWithValue("@channel", (object)channel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@destination", (object)destination ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@otpHash", otpHash);
            cmd.Parameters.AddWithValue("@validTill", (object)validTill ?? DateTime.Now.AddMinutes(10));
            cmd.Parameters.AddWithValue("@createdById", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            return function.getDefaultSPOutput(cmd, con);
        }

        public SPResponse verifyOtp(int draftId, string channel, string destination, string rawOtp)
        {
            var otpHash = Sha256Hex(rawOtp);
            var cmd = new SqlCommand("verifyAdmissionResumeOtp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@draftId", draftId);
            cmd.Parameters.AddWithValue("@channel", (object)channel ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@destination", (object)destination ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@otpHash", otpHash);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            return function.getDefaultSPOutput(cmd, con);
        }

        public static string GenerateUrlSafeToken(int byteLength = 32)
        {
            var bytes = new byte[byteLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string GenerateOtp(int length = 6)
        {
            if (length < 4) length = 4;
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append((bytes[i] % 10).ToString());
            }
            return sb.ToString();
        }
    }
}
