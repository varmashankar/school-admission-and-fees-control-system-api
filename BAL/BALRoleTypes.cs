using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALRoleTypes
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALRoleTypes()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveRoleTypes

        public SPResponse saveRoleTypes(RoleTypes dataString)
        {
            SqlCommand cmd = new SqlCommand("saveRoleTypes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<RoleTypes>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region getRoleTypesList

        public List<RoleTypes> getRoleTypesList(RoleTypesFilter dataString)
        {
            Adp = new SqlDataAdapter("getRoleTypesList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<RoleTypesFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<RoleTypes> RoleTypeList = new List<RoleTypes>();

            if (Dt.Rows.Count > 0)
            {
                // Get all columns' name
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                {
                    columns.Add(dc.ColumnName);
                }

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    RoleTypes element = Function.BindData<RoleTypes>(Dt.Rows[i], columns);
                    RoleTypeList.Add(element);
                }
            }

            return RoleTypeList;

        }

        #endregion      

        #region getRoleTypesDetails

        public RoleTypes getRoleTypesDetails(RoleTypesFilter dataString)
        {
            List<RoleTypes> RoleType = getRoleTypesList(dataString);

            if (RoleType.Count > 0)
            {
                return RoleType[0];
            }

            return null;
        }

        #endregion

        #region changeStatus

        public SPResponse changeStatus(RoleTypes dataString)
        {

            SqlCommand cmd = new SqlCommand("changeStatusForRoleTypes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@status", dataString.status);
            cmd.Parameters.AddWithValue("@id", dataString.id);
            cmd.Parameters.AddWithValue("@userId", dataString.userId);
            cmd.Parameters.AddWithValue("@roleTypeId", dataString.roleTypeId);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region deleteRoleTypes

        public SPResponse deleteRoleTypes(RoleTypes dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteRoleTypes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", dataString.id);
            cmd.Parameters.AddWithValue("@userId", dataString.userId);
            cmd.Parameters.AddWithValue("@roleTypeId", dataString.roleTypeId);
            cmd.Parameters.AddWithValue("@deletedTimestamp", dataString.deletedTimestamp);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion        

    }
}