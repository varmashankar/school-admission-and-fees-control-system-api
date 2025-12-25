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
    public class PermissionController : ApiController
    {
        #region savePermission

        [HttpPost]
        public HttpResponseMessage savePermission(Permission dataString)
        {
            try
            {
                if (!dataString.actionNameId.HasValue)
                {
                    return Return.returnHttp("201", "Please Select Action Name, It's a mandatory.");
                }

                if (!dataString.roleId.HasValue)
                {
                    return Return.returnHttp("201", "Please Select Role Type, It's a mandatory.");
                }

                if (!dataString.permission.HasValue)
                {
                    return Return.returnHttp("201", "Please Select Permission Type, It's a mandatory.");
                }

                //Creation Timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIA_ZONE);

                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALPermission func = new BALPermission();
                SPResponse response = func.savePermission(dataString);

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

        #region getPermissionList

        [HttpPost]
        public HttpResponseMessage getPermissionList(PermissionFilter dataString)
        {
            try
            {
                List<Permission> getPermissionList = new List<Permission>();

                BALPermission func = new BALPermission();
                getPermissionList = func.getPermissionList(dataString);

                return Return.returnHttp("200", getPermissionList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getPermissionDetails

        [HttpPost]
        public HttpResponseMessage getPermissionDetails(PermissionFilter dataString)
        {
            try
            {

                if (!dataString.id.HasValue)
                {
                    return Return.returnHttp("201", "Invalid Permission selected. Please try again.");
                }

                BALPermission func = new BALPermission();
                Permission permission = func.getPermissionDetails(dataString);

                return Return.returnHttp("200", permission);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region changePermissionStatus

        [HttpPost]
        public HttpResponseMessage changePermissionStatus(Permission dataString)
        {
            try
            {
                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid Permission selected. Please try again.");
                }

                BALPermission func = new BALPermission();
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

        #region deletePermission

        [HttpPost]
        public HttpResponseMessage deletePermission(Permission dataString)
        {

            try
            {
                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid Permission selected. Please try again.");
                }

                //creation_timestamp
                TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIAN_ZONE);

                dataString.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALPermission func = new BALPermission();
                SPResponse response = func.deletePermission(dataString);

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