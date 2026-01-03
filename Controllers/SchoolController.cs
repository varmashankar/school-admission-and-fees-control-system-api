using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class SchoolController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveSchool(School data)
        {
            try
            {
                if (data == null) return Return.returnHttp("201", "Invalid payload for school.");

                if (string.IsNullOrWhiteSpace(data.school_name))
                    return Return.returnHttp("201", new { message = "Please enter school name." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALSchool bal = new BALSchool();
                SPResponse response = bal.saveSchool(data);

                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }

        [HttpPost]
        public HttpResponseMessage getSchoolList(SchoolFilter filter)
        {
            try
            {
                BALSchool bal = new BALSchool();
                var list = bal.getSchoolList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getSchoolDetails(SchoolFilter filter)
        {
            try
            {
                if (filter == null || !filter.id.HasValue)
                    return Return.returnHttp("201", "School ID is required.");

                BALSchool bal = new BALSchool();
                var obj = bal.getSchoolDetails(filter);

                if (obj == null)
                    return Return.returnHttp("200", new { message = "No school found." });

                return Return.returnHttp("200", obj);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage changeSchoolStatus(School data)
        {
            try
            {
                if (data == null || !data.id.HasValue)
                    return Return.returnHttp("201", "Invalid school selected.");

                BALSchool bal = new BALSchool();
                var response = bal.changeSchoolStatus(data);

                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteSchool(School data)
        {
            try
            {
                if (data == null || !data.id.HasValue)
                    return Return.returnHttp("201", "Invalid school selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALSchool bal = new BALSchool();
                var response = bal.deleteSchool(data);

                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
    }
}
