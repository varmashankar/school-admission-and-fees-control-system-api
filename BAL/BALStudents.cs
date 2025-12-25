using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALStudents
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALStudents()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveStudent
        public SPResponse saveStudent(Students dataString)
        {
            SqlCommand cmd = new SqlCommand("saveStudents", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<Students>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region saveStudentAdmission
        public SPResponse saveStudentAdmission(StudentAdmission data)
        {
            SqlCommand cmd = new SqlCommand("saveStudentAdmission", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentAdmission>(ref cmd, data);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion


        #region getStudentList
        public List<GetStudents> getStudentList(StudentFilter dataString)
        {
            Adp = new SqlDataAdapter("getStudentList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            foreach (DataColumn col in Dt.Columns)
            {
                System.Diagnostics.Debug.WriteLine(col.ColumnName + " => " + col.DataType.FullName);
            }

            List<GetStudents> StudentList = new List<GetStudents>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                    columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetStudents element = Function.BindData<GetStudents>(Dt.Rows[i], columns);
                    StudentList.Add(element);
                }
            }
            return StudentList;
        }
        #endregion

        #region getStudentDetails
        public GetStudents getStudentDetails(StudentFilter dataString)
        {
            try
            {
                var list = getStudentList(dataString);
                if (list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getStudentDetails: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region changeStatus
        public SPResponse changeStatus(Students dataString)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForStudent", con);
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
        public SPResponse deleteStudent(Students dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteStudent", con);
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

        public string generateAdmissionId()
        {
            SqlCommand cmd = new SqlCommand("generateAdmissionId", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (con.State == ConnectionState.Closed)
                con.Open();

            string admissionId = Convert.ToString(cmd.ExecuteScalar());
            con.Close();

            return admissionId;
        }

        public string generateStudentId()
        {
            SqlCommand cmd = new SqlCommand("generateStudentId", con);
            cmd.CommandType = CommandType.StoredProcedure;

            if (con.State == ConnectionState.Closed)
                con.Open();

            string studentId = Convert.ToString(cmd.ExecuteScalar());
            con.Close();

            return studentId;
        }

        #region getStudentDashboardStats
        public List<GetStudentDashboardStats> getStudentDashboardStats(StudentDashboardStatsFilter filter)
        {
            Adp = new SqlDataAdapter("getStudentDashboardStats", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            if (filter.academicYearId.HasValue)
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", filter.academicYearId.Value);
            else
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", DBNull.Value);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetStudentDashboardStats> list = new List<GetStudentDashboardStats>();

            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn col in Dt.Columns)
                    columns.Add(col.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetStudentDashboardStats obj =
                        Function.BindData<GetStudentDashboardStats>(Dt.Rows[i], columns);

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion


    }

}