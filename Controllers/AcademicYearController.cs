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
    public class AcademicYearController : ApiController
    {
        #region saveAcademicYear
        [HttpPost]
        public HttpResponseMessage saveAcademicYear(AcademicYears data)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(data.year_code))
                    return Return.returnHttp("201", new { message = "Please Enter Year Code." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAcademicYears bal = new BALAcademicYears();
                SPResponse response = bal.saveAcademicYear(data);

                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }
        #endregion

        #region getAcademicYearList
        [HttpPost]
        public HttpResponseMessage getAcademicYearList(AcademicYearFilter filter)
        {
            try
            {
                BALAcademicYears bal = new BALAcademicYears();
                var list = bal.getAcademicYearList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region getAcademicYearDetails
        [HttpPost]
        public HttpResponseMessage getAcademicYearDetails(AcademicYearFilter filter)
        {
            try
            {
                if (!filter.id.HasValue) return Return.returnHttp("201", "Please Select Academic Year ID.");

                BALAcademicYears bal = new BALAcademicYears();
                var item = bal.getAcademicYearDetails(filter);
                return item == null ? Return.returnHttp("200", new { message = "No record found" }) : Return.returnHttp("200", item);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region changeStatus
        [HttpPost]
        public HttpResponseMessage changeStatusAcademicYears(AcademicYears data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid academic year selected.");

                BALAcademicYears bal = new BALAcademicYears();
                var response = bal.changeStatus(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region delete
        [HttpPost]
        public HttpResponseMessage deleteAcademicYears(AcademicYears data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid academic year selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAcademicYears bal = new BALAcademicYears();
                var response = bal.deleteAcademicYear(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion
    }
}