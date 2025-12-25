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
    public class AttendanceController : ApiController
    {
        #region saveAttendanceMaster
        [HttpPost]
        public HttpResponseMessage saveAttendanceMaster(AttendanceMaster data)
        {
            try
            {
                if (!data.classId.HasValue)
                    return Return.returnHttp("201", new { message = "Please select class." });

                if (string.IsNullOrWhiteSpace(data.attendanceDate))
                    return Return.returnHttp("201", new { message = "Please provide attendance date." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAttendance bal = new BALAttendance();
                SPResponse response = bal.saveAttendanceMaster(data);

                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }
        #endregion

        #region getAttendanceMasterList
        [HttpPost]
        public HttpResponseMessage getAttendanceMasterList(AttendanceMasterFilter filter)
        {
            try
            {
                BALAttendance bal = new BALAttendance();
                var list = bal.getAttendanceMasterList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region getAttendanceMasterDetails
        [HttpPost]
        public HttpResponseMessage getAttendanceMasterDetails(AttendanceMasterFilter filter)
        {
            try
            {
                if (!filter.id.HasValue) return Return.returnHttp("201", "Please select attendance master id.");

                BALAttendance bal = new BALAttendance();
                var item = bal.getAttendanceMasterDetails(filter);
                return item == null ? Return.returnHttp("200", new { message = "No record found" }) : Return.returnHttp("200", item);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region changeStatusMaster
        [HttpPost]
        public HttpResponseMessage changeStatusAttendanceMaster(AttendanceMaster data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid attendance master selected.");

                BALAttendance bal = new BALAttendance();
                var response = bal.changeStatusMaster(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region deleteMaster
        [HttpPost]
        public HttpResponseMessage deleteAttendanceMaster(AttendanceMaster data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid attendance master selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAttendance bal = new BALAttendance();
                var response = bal.deleteAttendanceMaster(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        // ---------------- Details ----------------

        #region saveAttendanceDetails
        [HttpPost]
        public HttpResponseMessage saveAttendanceDetails(AttendanceDetails data)
        {
            try
            {
                if (!data.attendanceMasterId.HasValue) return Return.returnHttp("201", new { message = "Attendance master is required." });
                if (!data.studentId.HasValue) return Return.returnHttp("201", new { message = "Student is required." });

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAttendance bal = new BALAttendance();
                SPResponse response = bal.saveAttendanceDetails(data);
                if (response.executionStatus == "TRUE") return Return.returnHttp("200", response.message);
                return Return.returnHttp("201", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }
        #endregion

        #region getAttendanceDetailsList
        [HttpPost]
        public HttpResponseMessage getAttendanceDetailsList(AttendanceDetailsFilter filter)
        {
            try
            {
                BALAttendance bal = new BALAttendance();
                var list = bal.getAttendanceDetailsList(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region changeStatusDetails
        [HttpPost]
        public HttpResponseMessage changeStatusAttendanceDetails(AttendanceDetails data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid attendance detail selected.");

                BALAttendance bal = new BALAttendance();
                var response = bal.changeStatusDetails(data);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);
                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }
        #endregion

        #region deleteDetails
        [HttpPost]
        public HttpResponseMessage deleteAttendanceDetails(AttendanceDetails data)
        {
            try
            {
                if (!data.id.HasValue) return Return.returnHttp("201", "Invalid attendance detail selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                data.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALAttendance bal = new BALAttendance();
                var response = bal.deleteAttendanceDetails(data);
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