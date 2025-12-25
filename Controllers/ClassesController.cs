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
    public class ClassesController : ApiController
    {
        #region saveClass
        [HttpPost]
        public HttpResponseMessage saveClass(Classes data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.className))
                    return Return.returnHttp("201", "Please enter class name.");

                TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZ);
                data.creationTimestamp = now.ToString("MM/dd/yyyy HH:mm:ss");

                BALClasses bal = new BALClasses();
                var response = bal.saveClass(data);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion


        #region getClassList
        [HttpPost]
        public HttpResponseMessage getClassList(ClassFilter filter)
        {
            try
            {
                BALClasses bal = new BALClasses();
                var list = bal.getClassList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion


        #region getClassDetails
        [HttpPost]
        public HttpResponseMessage getClassDetails(ClassFilter filter)
        {
            try
            {
                if (!filter.id.HasValue)
                    return Return.returnHttp("201", "Class ID is required.");

                BALClasses bal = new BALClasses();
                var obj = bal.getClassDetails(filter);

                if (obj == null)
                    return Return.returnHttp("200", new { message = "No class found." });

                return Return.returnHttp("200", obj);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion


        #region changeClassStatus
        [HttpPost]
        public HttpResponseMessage changeClassStatus(Classes data)
        {
            try
            {
                if (!data.id.HasValue)
                    return Return.returnHttp("201", "Invalid class selected.");

                BALClasses bal = new BALClasses();
                var response = bal.changeClassStatus(data);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion


        #region deleteClass
        [HttpPost]
        public HttpResponseMessage deleteClass(Classes data)
        {
            try
            {
                if (!data.id.HasValue)
                    return Return.returnHttp("201", "Invalid class selected.");

                TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZ);
                data.deletedTimestamp = now.ToString("MM/dd/yyyy HH:mm:ss");

                BALClasses bal = new BALClasses();
                var response = bal.deleteClass(data);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion
    }
}