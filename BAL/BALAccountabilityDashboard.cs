using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.BAL
{
    public class BALAccountabilityDashboard
    {
        private readonly SqlConnection con;
        private SqlDataAdapter Adp;
        private DataTable Dt;

        public BALAccountabilityDashboard()
        {
            con = DBConnection.GlobalConnection();
        }

        public List<StaffFollowUpSummaryItem> getStaffFollowUpSummary(AccountabilityDashboardFilter filter)
        {
            Adp = new SqlDataAdapter("sp_accountability_staff_followup_summary", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Adp.SelectCommand.Parameters.AddWithValue("@fromDate", (object)filter?.fromDate ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@toDate", (object)filter?.toDate ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@staffId", (object)filter?.staffId ?? DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<StaffFollowUpSummaryItem>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                foreach (DataRow row in Dt.Rows)
                    list.Add(Function.BindData<StaffFollowUpSummaryItem>(row, columns));
            }

            return list;
        }

        public List<MissedInquiryItem> getMissedInquiries(AccountabilityDashboardFilter filter)
        {
            Adp = new SqlDataAdapter("sp_accountability_missed_inquiries", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Adp.SelectCommand.Parameters.AddWithValue("@asOf", (object)filter?.toDate ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@staffId", (object)filter?.staffId ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@maxDaysOverdue", (object)filter?.maxDaysOverdue ?? DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<MissedInquiryItem>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                foreach (DataRow row in Dt.Rows)
                    list.Add(Function.BindData<MissedInquiryItem>(row, columns));
            }

            return list;
        }

        public List<AdmissionLossReasonItem> getAdmissionLossReasons(AccountabilityDashboardFilter filter)
        {
            Adp = new SqlDataAdapter("sp_accountability_admission_loss_reasons", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Adp.SelectCommand.Parameters.AddWithValue("@fromDate", (object)filter?.fromDate ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@toDate", (object)filter?.toDate ?? DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<AdmissionLossReasonItem>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                foreach (DataRow row in Dt.Rows)
                    list.Add(Function.BindData<AdmissionLossReasonItem>(row, columns));
            }

            return list;
        }

        public List<FeeCollectionDelayItem> getFeeCollectionDelays(AccountabilityDashboardFilter filter)
        {
            Adp = new SqlDataAdapter("sp_accountability_fee_collection_delays", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", (object)filter?.academicYearId ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@asOf", (object)filter?.toDate ?? DBNull.Value);
            Adp.SelectCommand.Parameters.AddWithValue("@maxDaysOverdue", (object)filter?.maxDaysOverdue ?? DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var list = new List<FeeCollectionDelayItem>();
            if (Dt.Rows.Count > 0)
            {
                var columns = new List<string>();
                foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                foreach (DataRow row in Dt.Rows)
                    list.Add(Function.BindData<FeeCollectionDelayItem>(row, columns));
            }

            return list;
        }
    }
}
