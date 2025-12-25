using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALEmergencyContacts
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALEmergencyContacts()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveStudentEmergencyContact(StudentEmergencyContacts dataString)
        {
            SqlCommand cmd = new SqlCommand("saveStudentEmergencyContact", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentEmergencyContacts>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }

        public List<GetStudentEmergencyContacts> getStudentEmergencyContacts(StudentEmergencyFilter dataString)
        {
            Adp = new SqlDataAdapter("getStudentEmergencyContacts", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StudentEmergencyFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetStudentEmergencyContacts> list = new List<GetStudentEmergencyContacts>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);
                for (int i = 0; i < Dt.Rows.Count; i++)
                    list.Add(Function.BindData<GetStudentEmergencyContacts>(Dt.Rows[i], columns));
            }
            return list;
        }

        public SPResponse deleteStudentEmergencyContact(StudentEmergencyContacts dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteStudentEmergencyContact", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)dataString.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deletedTimestamp", (object)dataString.creationTimestamp ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", (object)dataString.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
    }
}