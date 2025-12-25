using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALAttendance
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALAttendance()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveAttendanceMaster
        public SPResponse saveAttendanceMaster(AttendanceMaster data)
        {
            SqlCommand cmd = new SqlCommand("saveAttendanceMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<AttendanceMaster>(ref cmd, data);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getAttendanceMasterList
        public List<GetAttendanceMaster> getAttendanceMasterList(AttendanceMasterFilter filter)
        {
            Adp = new SqlDataAdapter("getAttendanceMasterList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<AttendanceMasterFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetAttendanceMaster> list = new List<GetAttendanceMaster>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetAttendanceMaster item = Function.BindData<GetAttendanceMaster>(Dt.Rows[i], columns);
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        #region getAttendanceMasterDetails
        public GetAttendanceMaster getAttendanceMasterDetails(AttendanceMasterFilter filter)
        {
            try
            {
                var list = getAttendanceMasterList(filter);
                if (list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in attendance master details: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region changeStatus
        public SPResponse changeStatusMaster(AttendanceMaster data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForAttendanceMaster", con);
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

        #region deleteAttendanceMaster
        public SPResponse deleteAttendanceMaster(AttendanceMaster data)
        {
            SqlCommand cmd = new SqlCommand("deleteAttendanceMaster", con);
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

        // ----------------- Details -----------------

        #region saveAttendanceDetails
        public SPResponse saveAttendanceDetails(AttendanceDetails data)
        {
            SqlCommand cmd = new SqlCommand("saveAttendanceDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<AttendanceDetails>(ref cmd, data);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getAttendanceDetailsList
        public List<GetAttendanceDetails> getAttendanceDetailsList(AttendanceDetailsFilter filter)
        {
            Adp = new SqlDataAdapter("getAttendanceDetailsList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<AttendanceDetailsFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetAttendanceDetails> list = new List<GetAttendanceDetails>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetAttendanceDetails item = Function.BindData<GetAttendanceDetails>(Dt.Rows[i], columns);
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        #region changeStatusDetails
        public SPResponse changeStatusDetails(AttendanceDetails data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForAttendanceDetails", con);
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

        #region deleteAttendanceDetails
        public SPResponse deleteAttendanceDetails(AttendanceDetails data)
        {
            SqlCommand cmd = new SqlCommand("deleteAttendanceDetails", con);
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