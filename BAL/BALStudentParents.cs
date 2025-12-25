using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALStudentParents
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALStudentParents()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveStudentParents(StudentParents dataString)
        {
            SqlCommand cmd = new SqlCommand("saveStudentParents", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentParents>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<GetStudentParents> getStudentParents(StudentParentFilter dataString)
        {
            Adp = new SqlDataAdapter("getStudentParents", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentParentFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetStudentParents> list = new List<GetStudentParents>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);
                for (int i = 0; i < Dt.Rows.Count; i++)
                    list.Add(Function.BindData<GetStudentParents>(Dt.Rows[i], columns));
            }
            return list;
        }

        public SPResponse deleteStudentParent(StudentParentFilter dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteStudentParent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)dataString.id ?? DBNull.Value);
            //cmd.Parameters.AddWithValue("@deletedTimestamp", (object)dataString.creationTimestamp ?? DBNull.Value);
            //cmd.Parameters.AddWithValue("@userId", (object)dataString.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
    }
}