using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.BAL
{
    public class BALInquiries
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALInquiries()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveInquiry(Inquiry data)
        {
            if (data == null) throw new ArgumentNullException("data");

            if (string.IsNullOrWhiteSpace(data.status))
                data.status = InquiryStatus.New;

            SqlCommand cmd = new SqlCommand("saveInquiry", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<Inquiry>(ref cmd, data);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<Inquiry> getInquiryList(InquiryFilter filter)
        {
            Adp = new SqlDataAdapter("getInquiryList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            if (filter != null)
                function.addClassAttributes<InquiryFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<Inquiry> list = new List<Inquiry>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    Inquiry element = Function.BindData<Inquiry>(Dt.Rows[i], columns);
                    list.Add(element);
                }
            }

            return list;
        }

        public Inquiry getInquiryDetails(InquiryFilter filter)
        {
            var list = getInquiryList(filter);
            if (list != null && list.Count > 0) return list[0];
            return null;
        }

        public SPResponse changeInquiryStatus(Inquiry data)
        {
            SqlCommand cmd = new SqlCommand("changeInquiryStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", (object)data.status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", (object)data.createdById ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<InquiryStatusHistory> getInquiryStatusHistory(InquiryFilter filter)
        {
            Adp = new SqlDataAdapter("getInquiryStatusHistory", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (filter != null)
                Adp.SelectCommand.Parameters.AddWithValue("@inquiryId", (object)filter.id ?? DBNull.Value);
            else
                Adp.SelectCommand.Parameters.AddWithValue("@inquiryId", DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<InquiryStatusHistory> list = new List<InquiryStatusHistory>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    InquiryStatusHistory element = Function.BindData<InquiryStatusHistory>(Dt.Rows[i], columns);
                    list.Add(element);
                }
            }

            return list;
        }

        public SPResponse addInquiryStatusHistory(InquiryStatusHistory data)
        {
            SqlCommand cmd = new SqlCommand("saveInquiryStatusHistory", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<InquiryStatusHistory>(ref cmd, data);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public SPResponse saveInquiryFollowUp(InquiryFollowUp data)
        {
            SqlCommand cmd = new SqlCommand("saveInquiryFollowUp", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<InquiryFollowUp>(ref cmd, data);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<InquiryFollowUp> getDueFollowUps(DateTime? dueBefore)
        {
            Adp = new SqlDataAdapter("getDueInquiryFollowUps", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Adp.SelectCommand.Parameters.AddWithValue("@dueBefore", (object)dueBefore ?? DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<InquiryFollowUp> list = new List<InquiryFollowUp>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    InquiryFollowUp element = Function.BindData<InquiryFollowUp>(Dt.Rows[i], columns);
                    list.Add(element);
                }
            }

            return list;
        }

        public SPResponse markFollowUpReminded(MarkFollowUpRemindedRequest data)
        {
            SqlCommand cmd = new SqlCommand("markInquiryFollowUpReminded", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@followUpId", (object)data.followUpId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@remindedAt", (object)data.remindedAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", (object)data.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<InquiryConversionReportItem> getConversionReport(ConversionReportFilter filter)
        {
            Adp = new SqlDataAdapter("getInquiryConversionReport", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            if (filter != null)
                function.addClassAttributes<ConversionReportFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            var result = new List<InquiryConversionReportItem>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    InquiryConversionReportItem element = Function.BindData<InquiryConversionReportItem>(Dt.Rows[i], columns);
                    result.Add(element);
                }
            }

            return result;
        }
    }
}
