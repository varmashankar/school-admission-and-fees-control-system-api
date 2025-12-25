using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALTeachers
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALTeachers()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveTeachers

        public SPResponse saveTeacher(Teachers dataString)
        {
            SqlCommand cmd = new SqlCommand("saveTeacher", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<Teachers>(ref cmd, dataString);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }

        #endregion

        #region getTeacherList

        public List<GetTeachers> getTeacherList(TeacherFilter dataString)
        {
            Adp = new SqlDataAdapter("getTeacherList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<TeacherFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetTeachers> teacherList = new List<GetTeachers>();
            if(Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for(int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetTeachers teacher = Function.BindData<GetTeachers>(Dt.Rows[i], columns);
                    teacherList.Add(teacher);
                }
            }
            return teacherList;
        }

        #endregion

        #region getTeacherDetails

        public GetTeachers getTeacherDetails(TeacherFilter dataString)
        {
            try
            {
                var list = getTeacherList(dataString);
                if(list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in teacher details: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region changeStatus
        public SPResponse changeStatus(Teachers dataString)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForTeacher", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)dataString.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", dataString.status);
            cmd.Parameters.AddWithValue("@userId", (object)dataString.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)dataString.roleId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region deleteStudent
        public SPResponse deleteStudent(Teachers dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteTeacher", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)dataString.id ?? DBNull.Value);
            // expect deletedTimestamp is set on model
            DateTime deletedTs;
            if (!DateTime.TryParse(dataString.deletedTimestamp, out deletedTs))
                deletedTs = DateTime.Now;

            cmd.Parameters.AddWithValue("@deletedTimestamp", deletedTs);
            cmd.Parameters.AddWithValue("@userId", (object)dataString.deletedById ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)dataString.roleId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region GenerateTeacherId

        public string generateTeacherId()
        {
            SqlCommand cmd = new SqlCommand("generateTeacherId", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (con.State == ConnectionState.Closed)
                con.Open();

            string admissionId = Convert.ToString(cmd.ExecuteScalar());
            con.Close();

            return admissionId;
        }

        #endregion

        #region GetTeacherDashBoardStats
        public List<GetTeacherDashboardStats> getTeacherDashboardStats(TeacherDashboardFilter filter)
        {
            Adp = new SqlDataAdapter("getTeacherDashboardStats", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (filter.academicYearId.HasValue)
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", filter.academicYearId.Value);
            else
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetTeacherDashboardStats> list = new List<GetTeacherDashboardStats>();

            if(Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn col in Dt.Columns)
                    columns.Add(col.ColumnName);

                for(int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetTeacherDashboardStats obj = Function.BindData<GetTeacherDashboardStats>(Dt.Rows[i], columns);

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion
    }
}