using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.BAL
{
    public class BALPreviousSchool
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALPreviousSchool()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveStudentPreviousSchool(StudentPreviousSchoolDetails dataString)
        {
            SqlCommand cmd = new SqlCommand("saveStudentPreviousSchool", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentPreviousSchoolDetails>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<GetStudentPreviousSchoolDetails> getStudentPreviousSchool(StudentPreviousSchoolFilter dataString)
        {
            Adp = new SqlDataAdapter("getStudentPreviousSchool", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentPreviousSchoolFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetStudentPreviousSchoolDetails> list = new List<GetStudentPreviousSchoolDetails>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);
                for (int i = 0; i < Dt.Rows.Count; i++)
                    list.Add(Function.BindData<GetStudentPreviousSchoolDetails>(Dt.Rows[i], columns));
            }
            return list;
        }

        public SPResponse deleteStudentPreviousSchool(StudentPreviousSchoolDetails dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteStudentPreviousSchool", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)dataString.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deletedTimestamp", (object)dataString.creationTimestamp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
    }
}