using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.BAL
{
    public class BALSchool
    {
        private readonly SqlConnection con;
        private SqlDataAdapter Adp;
        private DataTable Dt;

        public BALSchool()
        {
            con = DBConnection.GlobalConnection();
        }

        public SPResponse saveSchool(School data)
        {
            if (data == null) throw new ArgumentNullException("data");

            // NOTE: In current DB dump, school save SP is named plural.
            SqlCommand cmd = new SqlCommand("saveSchools", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function fn = new Function();
            fn.addClassAttributes<School>(ref cmd, data);
            fn.addDefaultSPOutput(ref cmd);

            return fn.getDefaultSPOutput(cmd, con);
        }

        public List<GetSchool> getSchoolList(SchoolFilter filter)
        {
            // NOTE: In current DB dump, school list SP is named plural.
            Adp = new SqlDataAdapter("getSchools", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function fn = new Function();
            fn.addClassAttributes<SchoolFilter>(ref Adp, filter ?? new SchoolFilter());

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetSchool> list = new List<GetSchool>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn col in Dt.Columns)
                    columns.Add(col.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetSchool obj = Function.BindData<GetSchool>(Dt.Rows[i], columns);
                    list.Add(obj);
                }
            }

            return list;
        }

        public GetSchool getSchoolDetails(SchoolFilter filter)
        {
            var list = getSchoolList(filter);
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        public SPResponse changeSchoolStatus(School data)
        {
            if (data == null) throw new ArgumentNullException("data");

            SqlCommand cmd = new SqlCommand("changeStatusForSchool", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", (object)data.status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", (object)data.userId ?? DBNull.Value);

            Function fn = new Function();
            fn.addDefaultSPOutput(ref cmd);

            return fn.getDefaultSPOutput(cmd, con);
        }

        public SPResponse deleteSchool(School data)
        {
            if (data == null) throw new ArgumentNullException("data");

            SqlCommand cmd = new SqlCommand("deleteSchool", con);
            cmd.CommandType = CommandType.StoredProcedure;

            DateTime dt;
            if (!DateTime.TryParse(data.deletedTimestamp, out dt))
                dt = DateTime.Now;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@deletedTimestamp", dt);
            cmd.Parameters.AddWithValue("@deletedById", (object)data.deletedById ?? DBNull.Value);

            Function fn = new Function();
            fn.addDefaultSPOutput(ref cmd);

            return fn.getDefaultSPOutput(cmd, con);
        }
    }
}
