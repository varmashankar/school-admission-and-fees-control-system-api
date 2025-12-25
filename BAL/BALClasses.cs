using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALClasses
    {
        private SqlConnection con;
        private SqlDataAdapter Adp;
        private DataTable Dt;

        public BALClasses()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveClass
        public SPResponse saveClass(Classes data)
        {
            SqlCommand cmd = new SqlCommand("saveClasses", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function fn = new Function();
            fn.addClassAttributes<Classes>(ref cmd, data);
            fn.addDefaultSPOutput(ref cmd);

            SPResponse response = fn.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getClassList
        public List<GetClasses> getClassList(ClassFilter filter)
        {
            Adp = new SqlDataAdapter("getClassList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function fn = new Function();
            fn.addClassAttributes<ClassFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);


            List<GetClasses> list = new List<GetClasses>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn col in Dt.Columns)
                    columns.Add(col.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetClasses obj = Function.BindData<GetClasses>(Dt.Rows[i], columns);
                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion

        #region getClassDetails
        public GetClasses getClassDetails(ClassFilter filter)
        {
            var list = getClassList(filter);
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }
        #endregion

        #region changeClassStatus
        public SPResponse changeClassStatus(Classes data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForClass", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", data.status);
            cmd.Parameters.AddWithValue("@userId", (object)data.userId?? DBNull.Value);

            Function fn = new Function();
            fn.addDefaultSPOutput(ref cmd);

            return fn.getDefaultSPOutput(cmd, con);
        }
        #endregion

        #region deleteClass
        public SPResponse deleteClass(Classes data)
        {
            SqlCommand cmd = new SqlCommand("deleteClass", con);
            cmd.CommandType = CommandType.StoredProcedure;

            DateTime dt;
            if (!DateTime.TryParse(data.deletedTimestamp, out dt))
                dt = DateTime.Now;

            cmd.Parameters.AddWithValue("@id", data.id);
            cmd.Parameters.AddWithValue("@deletedTimestamp", dt);
            cmd.Parameters.AddWithValue("@deletedById", data.deletedById);

            Function fn = new Function();
            fn.addDefaultSPOutput(ref cmd);

            return fn.getDefaultSPOutput(cmd, con);
        }
        #endregion
    }
}