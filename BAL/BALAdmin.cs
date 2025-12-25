using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALAdmin
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALAdmin()
        {
            con = DBConnection.GlobalConnection();
        }

        #region Login

        public LoginResponse chkAdminName(Login dataString)
        {
            string[] message = new string[3];

            SqlCommand cmd = new SqlCommand("chkAdminUserName", con);
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

        #region genOTPForAdmin

        public SPResponse genOTPForAdmin(OTP dataString)
        {

            SqlCommand cmd = new SqlCommand("genOTPForAdminUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<OTP>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region verifyOTPForAdmin

        public SPResponse verifyOTPForAdmin(OTP dataString)
        {

            SqlCommand cmd = new SqlCommand("verifyOTPForAdminUser", con);
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

            SqlCommand cmd = new SqlCommand("forgetPasswordForAdminUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<ForgetPassword>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region saveAdmin

        public SPResponse saveAdmin(AdminUsers dataString)
        {
            SqlCommand cmd = new SqlCommand("saveAdminUsers", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function function = new Function();

            function.addClassAttributes<AdminUsers>(ref cmd, dataString);
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region getAdminList

        public List<GetAdminUsers> getAdminList(AdminUsersFilter dataString)
        {
            Adp = new SqlDataAdapter("getAdminUserList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<AdminUsersFilter>(ref Adp, dataString);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetAdminUsers> AdminList = new List<GetAdminUsers>();

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
                    GetAdminUsers element = Function.BindData<GetAdminUsers>(Dt.Rows[i], columns);
                    AdminList.Add(element);
                }
            }

            return AdminList;

        }

        #endregion

        #region getAdminDetails

        public GetAdminUsers getAdminDetails(AdminUsersFilter dataString)
        {
            try
            {
                List<GetAdminUsers> adminUser = getAdminList(dataString);

                if (adminUser != null && adminUser.Count > 0)
                {
                    return adminUser[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                System.Diagnostics.Debug.WriteLine("Error in getAdminDetails: " + ex.Message + "\n" + ex.StackTrace);

                // Return null if something went wrong
                return null;
            }
        }


        #endregion

        #region changePassword

        public SPResponse changePassword(ChangePassword dataString)
        {
            SqlCommand cmd = new SqlCommand("changePasswordForAdminUser", con);
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

        public SPResponse changeStatus(AdminUsers dataString)
        {

            SqlCommand cmd = new SqlCommand("changeStatusForAdminUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@status", dataString.status);
            cmd.Parameters.AddWithValue("@id", dataString.id);
            cmd.Parameters.AddWithValue("@userId", dataString.userId);
            cmd.Parameters.AddWithValue("@roleTypeId", dataString.roleId);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion

        #region deleteAdmin

        public SPResponse deleteAdmin(AdminUsers dataString)
        {
            SqlCommand cmd = new SqlCommand("deleteAdminUser", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", dataString.id);
            cmd.Parameters.AddWithValue("@userId", dataString.userId);
            cmd.Parameters.AddWithValue("@roleTypeId", dataString.roleId);
            cmd.Parameters.AddWithValue("@deletedTimestamp", dataString.deletedTimestamp);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion
    }
}
