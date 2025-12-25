using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class AdminController : ApiController
    {
        #region Token
        public string CreateToken(int? id, int? roleType)
        {
            return Function.CreateToken((int)roleType, id);
        }

        #endregion

        #region Login

        [HttpPost]
        public HttpResponseMessage login(Login dataString)
        {
            try
            {
                if (string.IsNullOrEmpty(dataString.username))
                {
                    return Return.returnHttp("201", "Please Enter Username, It's a Mandatory.");
                }

                if (string.IsNullOrEmpty(dataString.password))
                {
                    return Return.returnHttp("201", "Please Enter Password, It's a Mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAdmin func = new BALAdmin();

                LoginResponse response = func.chkAdminName(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                int id = Convert.ToInt32(response.id);
                int roleTypeId = Convert.ToInt32(response.roleTypeId);

                HttpResponseMessage httpResponse = Return.returnHttp("200", response.message);
                httpResponse.Headers.Add("token", CreateToken(id, roleTypeId));

                return httpResponse;
            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region generateOTP

        [HttpPost]
        public HttpResponseMessage generateOTP(OTP dataString)
        {
            try
            {

                if ((string.IsNullOrEmpty(dataString.email)) && (string.IsNullOrEmpty(dataString.mobile)))
                {
                    return Return.returnHttp("201", "Please enter Email or Mobile. Please try again.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                //OTP Generate
                Random rnd = new Random();
                int[] a = { rnd.Next(8), rnd.Next(8), rnd.Next(8), rnd.Next(8), rnd.Next(8), rnd.Next(8) };
                string otp = Function.OTP(a);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");
                dataString.otp = otp;

                BALAdmin func = new BALAdmin();

                SPResponse response = func.genOTPForAdmin(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                //Send SMS Code
                Function function = new Function();

                /*function.sendEmail(dataString.email, "Verification code", "Please use the verification code.\n" +
                     "OTP : " + otp +
                     "\n\nIf you didn’t request this, you can ignore this email.", "");*/

                return Return.returnHttp("200", response.message);

            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please refresh." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region verifyOTP

        [HttpPost]
        public HttpResponseMessage verifyOTP(OTP dataString)
        {
            try
            {

                if ((string.IsNullOrEmpty(dataString.email)) && (string.IsNullOrEmpty(dataString.mobile)))
                {
                    return Return.returnHttp("201", "Please enter Email or Mobile. Please try again.");
                }

                if (String.IsNullOrEmpty(dataString.otp))
                {
                    return Return.returnHttp("201", "Please enter otp , it's mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                BALAdmin func = new BALAdmin();

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                SPResponse response = func.verifyOTPForAdmin(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);
            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region forgetPassword

        [HttpPost]
        public HttpResponseMessage forgetPassword(ForgetPassword dataString)
        {
            try
            {

                if ((string.IsNullOrEmpty(dataString.email)) && (string.IsNullOrEmpty(dataString.mobile)))
                {
                    return Return.returnHttp("201", "Please enter Email or Mobile. Please try again.");
                }
                if (String.IsNullOrEmpty(dataString.otp))
                {
                    return Return.returnHttp("201", "Please enter otp , it's mandatory.");
                }
                if (String.IsNullOrEmpty(dataString.password))
                {
                    return Return.returnHttp("201", "Please enter password, it's mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAdmin func = new BALAdmin();

                SPResponse response = func.forgetPassword(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);
            }
            catch (Exception e)
            {
                return Return.returnHttp("202", "Some technical fault occured. Please contact Developer." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region saveAdminUsers

        [HttpPost]
        public HttpResponseMessage saveAdminUsers(AdminUsers dataString)
        {

            try
            {
                if (string.IsNullOrEmpty(dataString.name))
                {
                    return Return.returnHttp("201", "Please Enter Name, It's a mandatory.");
                }

                if (string.IsNullOrEmpty(dataString.mobile))
                {
                    return Return.returnHttp("201", "Please Enter Mobile, It's a mandatory.");
                }

                if (string.IsNullOrEmpty(dataString.email))
                {
                    return Return.returnHttp("201", "Please Enter Email, It's a mandatory.");
                }

                //Random Password
                Random random = new Random();
                int[] a = { random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57) };
                dataString.password = Function.CreatePassword(a);

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAdmin BALAdminUsers = new BALAdmin();
                SPResponse response = BALAdminUsers.saveAdmin(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getAdminUsersList

        [HttpPost]
        public HttpResponseMessage getAdminUsersList(AdminUsersFilter dataString)
        {
            try
            {
                List<GetAdminUsers> AdminUsersList = new List<GetAdminUsers>();

                BALAdmin func = new BALAdmin();
                AdminUsersList = func.getAdminList(dataString);

                return Return.returnHttp("200", AdminUsersList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getAdminUsersDetails

        [HttpPost]
        public HttpResponseMessage getAdminDetails(AdminUsersFilter dataString)
        {
            try
            {
                if (!dataString.id.HasValue)
                {
                    return Return.returnHttp("201", "Please Select Employee User ID, It's a mandatory.");
                }

                BALAdmin func = new BALAdmin();

                // Avoid conflict in SP filters: don't pass userId when using id
                dataString.userId = null;

                GetAdminUsers AdminUsers = func.getAdminDetails(dataString);

                if (AdminUsers == null)
                {
                    return Return.returnHttp("200", new { message = "No admin user found for the given ID." });
                }

                return Return.returnHttp("200", AdminUsers);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getAdminDetails: " + ex.Message + "\n" + ex.StackTrace);
                return Return.returnHttp("201", "Some Internal Issue Occurred. Please try again.");
            }
        }


        #endregion

        #region getProfile

        [HttpPost]
        public HttpResponseMessage getProfile(AdminUsersFilter dataString)
        {
            try
            {
                if (!dataString.userId.HasValue)
                {
                    return Return.returnHttp("201", "User ID is required to get profile.");
                }

                BALAdmin func = new BALAdmin();

                // Pass only id; do not pass userId to avoid SP conflict
                dataString.id = dataString.userId;
                dataString.userId = null;

                GetAdminUsers profile = func.getAdminDetails(dataString);

                if (profile == null)
                {
                    return Return.returnHttp("200", new { message = "Profile not found." });
                }

                return Return.returnHttp("200", profile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getProfile: " + ex.Message + "\n" + ex.StackTrace);
                return Return.returnHttp("201", "Some Internal Issue Occurred. Please try again.");
            }
        }


        #endregion

        #region updateProfile

        [HttpPost]
        public HttpResponseMessage updateProfile(AdminUsers dataString)
        {

            try
            {
                if (string.IsNullOrEmpty(dataString.name))
                {
                    return Return.returnHttp("201", "Please Enter Name, It's a mandatory.");
                }

                if (string.IsNullOrEmpty(dataString.mobile))
                {
                    return Return.returnHttp("201", "Please Enter Mobile, It's a mandatory.");
                }

                if (string.IsNullOrEmpty(dataString.email))
                {
                    return Return.returnHttp("201", "Please Enter Email, It's a mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");
                dataString.id = dataString.userId;

                BALAdmin func = new BALAdmin();
                SPResponse response = func.saveAdmin(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region changePassword

        [HttpPost]
        public HttpResponseMessage changePassword(ChangePassword dataString)
        {
            try
            {
                if (String.IsNullOrEmpty(dataString.oldPassword))
                {
                    return Return.returnHttp("201", "Please Enter Old Password, It's a mandatory.");
                }
                if (String.IsNullOrEmpty(dataString.newPassword))
                {
                    return Return.returnHttp("201", "Please Enter New Password, It's a mandatory.");
                }

                BALAdmin func = new BALAdmin();
                SPResponse response = func.changePassword(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);

            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region changeStatusAdminUsers

        [HttpPost]
        public HttpResponseMessage changeStatusAdminUsers(AdminUsers dataString)
        {
            try
            {

                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid admin user selected. Please try again.");
                }

                BALAdmin func = new BALAdmin();
                SPResponse response = func.changeStatus(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);

            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region deleteAdminUsers

        [HttpPost]
        public HttpResponseMessage deleteAdminUsers(AdminUsers dataString)
        {

            try
            {
                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid admin user selected. Please try again.");
                }

                //creation_timestamp
                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIAN_ZONE);

                dataString.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAdmin func = new BALAdmin();
                SPResponse response = func.deleteAdmin(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);
            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion
    }
}