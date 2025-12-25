using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALPermission
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALPermission()
        {
            con = DBConnection.GlobalConnection();
        }

        #region savePermission

        public SPResponse savePermission(Permission dataString)
        {
            SqlCommand cmd = new SqlCommand("savePermission", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<Permission>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region getPermissionList

        public List<Permission> getPermissionList(PermissionFilter dataString)
        {
            Adp = new SqlDataAdapter("getPermissionList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<PermissionFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<Permission> getPermissionList = new List<Permission>();

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
                    Permission element = Function.BindData<Permission>(Dt.Rows[i], columns);
                    getPermissionList.Add(element);
                }
            }

            return getPermissionList;

        }

        #endregion      

        #region getPermissionDetails

        public Permission getPermissionDetails(PermissionFilter dataString)
        {
            List<Permission> permissions = getPermissionList(dataString);

            if (permissions.Count > 0)
            {
                return permissions[0];
            }

            return null;
        }

        #endregion

        #region changeStatus

        public SPResponse changeStatus(Permission dataString)
        {

            SqlCommand cmd = new SqlCommand("changeStatusForPermission", con);
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

        #region deletePermission

        public SPResponse deletePermission(Permission dataString)
        {
            SqlCommand cmd = new SqlCommand("deletePermission", con);
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