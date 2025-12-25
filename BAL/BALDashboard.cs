using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALDashboard
    {

        private SqlConnection con;
        private SqlDataAdapter Adp;
        private DataTable Dt;

        public BALDashboard()
        {
            con = DBConnection.GlobalConnection();
        }

        #region getDashboardStats
        public List<GetDashboardStats> getDashboardStats(DashboardStatsFilter filter)
        {
            Adp = new SqlDataAdapter("getDashboardStats", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (filter.academicYearId.HasValue)
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", filter.academicYearId.Value);
            else
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetDashboardStats> list = new List<GetDashboardStats>();

            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn col in Dt.Columns)
                    columns.Add(col.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetDashboardStats obj = Function.BindData<GetDashboardStats>(Dt.Rows[i], columns);
                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion


    }
}