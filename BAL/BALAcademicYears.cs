using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALAcademicYears
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALAcademicYears()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveAcademicYear
        public SPResponse saveAcademicYear(AcademicYears data)
        {
            SqlCommand cmd = new SqlCommand("saveAcademicYear", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<AcademicYears>(ref cmd, data);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getAcademicYearList
        public List<GetAcademicYear> getAcademicYearList(AcademicYearFilter filter)
        {
            Adp = new SqlDataAdapter("getAcademicYearList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<AcademicYearFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetAcademicYear> list = new List<GetAcademicYear>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetAcademicYear item = Function.BindData<GetAcademicYear>(Dt.Rows[i], columns);
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        #region getAcademicYearDetails
        public GetAcademicYear getAcademicYearDetails(AcademicYearFilter filter)
        {
            try
            {
                var list = getAcademicYearList(filter);
                if (list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in academic year details: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region changeStatus
        public SPResponse changeStatus(AcademicYears data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForAcademicYear", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", data.status);
            cmd.Parameters.AddWithValue("@userId", (object)data.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleTypeId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region deleteAcademicYear
        public SPResponse deleteAcademicYear(AcademicYears data)
        {
            SqlCommand cmd = new SqlCommand("deleteAcademicYear", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            DateTime deletedTs;
            if (!DateTime.TryParse(data.deletedTimestamp, out deletedTs))
                deletedTs = DateTime.Now;

            cmd.Parameters.AddWithValue("@deletedTimestamp", deletedTs.ToString("MM/dd/yyyy HH:mm:ss"));
            cmd.Parameters.AddWithValue("@userId", (object)data.deletedById ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleTypeId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion
    }
}