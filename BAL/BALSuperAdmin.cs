using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALSuperAdmin
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALSuperAdmin()
        {
            con = DBConnection.GlobalConnection();
        }

        #region Login

        public LoginResponse chkSuperAdminName(Login dataString)
        {
            string[] message = new string[3];

            SqlCommand cmd = new SqlCommand("chkSuperAdminName", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<Login>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            cmd.Parameters.Add("@roleTypeId", SqlDbType.Int, 4);
            cmd.Parameters["@roleTypeId"].Direction = ParameterDirection.Output;

            LoginResponse response = new LoginResponse();

            con.Open();
            cmd.ExecuteNonQuery();

            response.message = Convert.ToString(cmd.Parameters["@message"].Value);
            response.id = Convert.ToInt32(cmd.Parameters["@outputId"].Value);
            response.executionStatus = Convert.ToString(cmd.Parameters["@executionStatus"].Value);
            response.roleTypeId = Convert.ToInt32(cmd.Parameters["@roleTypeId"].Value);

            con.Close();

            return response;
        }

        #endregion      

        #region genOTPForSuperAdmin

        public SPResponse genOTPForSuperAdmin(OTP dataString)
        {

            SqlCommand cmd = new SqlCommand("genOTPForSuperAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<OTP>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region verifyOTPForSuperAdmin

        public SPResponse verifyOTPForSuperAdmin(OTP dataString)
        {

            SqlCommand cmd = new SqlCommand("verifyOTPForSuperAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<OTP>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region forgetPassword

        public SPResponse forgetPassword(ForgetPassword dataString)
        {

            SqlCommand cmd = new SqlCommand("forgetPasswordForSuperAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<ForgetPassword>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion        

        #region saveSuperAdmin

        public SPResponse saveSuperAdmin(SuperAdmin dataString)
        {
            SqlCommand cmd = new SqlCommand("saveSuperAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<SuperAdmin>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region getSuperAdminList

        public List<GetSuperAdmin> getSuperAdminList(SuperAdminFilter dataString)
        {
            Adp = new SqlDataAdapter("getSuperAdminList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<SuperAdminFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetSuperAdmin> SuperAdminList = new List<GetSuperAdmin>();

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
                    GetSuperAdmin element = Function.BindData<GetSuperAdmin>(Dt.Rows[i], columns);
                    SuperAdminList.Add(element);
                }
            }

            return SuperAdminList;

        }

        #endregion      

        #region getSuperAdminDetails

        public GetSuperAdmin getSuperAdminDetails(SuperAdminFilter dataString)
        {
            List<GetSuperAdmin> SuperAdmin = getSuperAdminList(dataString);

            if (SuperAdmin.Count > 0)
            {
                return SuperAdmin[0];
            }

            return null;
        }

        #endregion

        #region changePassword

        public SPResponse changePassword(ChangePassword dataString)
        {
            SqlCommand cmd = new SqlCommand("changePasswordForSuperAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function()
                ;
            function.addClassAttributes<ChangePassword>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region changeStatus

        public SPResponse changeStatus(SuperAdmin dataString)
        {

            SqlCommand cmd = new SqlCommand("changeStatusForSuperAdmin", con);
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

        #region deleteSuperAdmin

        public SPResponse deleteSuperAdmin(SuperAdmin dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteSuperAdmin", con);
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