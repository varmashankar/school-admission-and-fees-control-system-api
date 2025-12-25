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
    public class RoleTypesController : ApiController
    {
        #region saveRoleTypes

        [HttpPost]
        public HttpResponseMessage saveRoleTypes(RoleTypes dataString)
        {

            try
            {
                if (string.IsNullOrEmpty(dataString.title))
                {
                    return Return.returnHttp("201", "Please Enter Title, It's a mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALRoleTypes func = new BALRoleTypes();
                SPResponse response = func.saveRoleTypes(dataString);

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

        #region getRoleTypesList

        [HttpPost]
        public HttpResponseMessage getRoleTypesList(RoleTypesFilter dataString)
        {
            try
            {
                List<RoleTypes> RoleTypeList = new List<RoleTypes>();

                BALRoleTypes func = new BALRoleTypes();
                RoleTypeList = func.getRoleTypesList(dataString);

                return Return.returnHttp("200", RoleTypeList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getRoleTypesDetails

        [HttpPost]
        public HttpResponseMessage getRoleTypesDetails(RoleTypesFilter dataString)
        {
            try
            {

                if (!dataString.id.HasValue)
                {
                    return Return.returnHttp("201", "Please Select Admin User, It's a mandatory.");
                }

                BALRoleTypes func = new BALRoleTypes();
                RoleTypes RoleType = func.getRoleTypesDetails(dataString);

                return Return.returnHttp("200", RoleType);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region changeRoleTypesStatus

        [HttpPost]
        public HttpResponseMessage changeRoleTypesStatus(RoleTypes dataString)
        {
            try
            {

                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid admin user selected. Please try again.");
                }

                BALRoleTypes func = new BALRoleTypes();
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

        #region deleteRoleTypes

        [HttpPost]
        public HttpResponseMessage deleteRoleTypes(RoleTypes dataString)
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

                BALRoleTypes func = new BALRoleTypes();
                SPResponse response = func.deleteRoleTypes(dataString);

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