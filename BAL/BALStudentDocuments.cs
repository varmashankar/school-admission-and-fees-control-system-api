using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALStudentDocuments
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALStudentDocuments()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveStudentDocument(StudentDocuments dataString)
        {
            SqlCommand cmd = new SqlCommand("saveStudentDocument", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentDocuments>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<StudentDocuments> getStudentDocuments(StudentDocuments dataString)
        {
            Adp = new SqlDataAdapter("getStudentDocuments", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentDocuments>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<StudentDocuments> list = new List<StudentDocuments>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);
                for (int i = 0; i < Dt.Rows.Count; i++)
                    list.Add(Function.BindData<StudentDocuments>(Dt.Rows[i], columns));
            }
            return list;
        }

        public SPResponse deleteStudentDocument(StudentDocuments dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteStudentDocument", con);
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