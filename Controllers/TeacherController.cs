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
    public class TeacherController : ApiController
    {
        #region helpers
        public string CreateToken(int? id, int? roleType)
        {
            return Function.CreateToken((int)roleType, id);
        }
        #endregion

        #region login
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

                BALTeachers func = new BALTeachers();
                LoginResponse response = func.chkTeacherName(dataString);

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

        #region saveTeachers
        [HttpPost]
        public HttpResponseMessage saveTeachers(Teachers dataString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataString.firstName))
                    return Return.returnHttp("201", new { message = "Please Enter Teacher First Name." });

                // set creationTimestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                //Random Password
                Random random = new Random();
                int[] a = { random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57), random.Next(57) };
                dataString.password = Function.CreatePassword(a);

                BALTeachers bal= new BALTeachers();
                SPResponse response = bal.saveTeacher(dataString);

                if (response.executionStatus == "TRUE")
                {
                    // SUCCESS
                    return Return.returnHttp("200", response.message);
                }
                else
                {
                    // FAILURE
                    return Return.returnHttp("201", response.message);
                }
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", new { message = "Some Internal Issue Occurred. " + ex.Message });
            }
        }
        #endregion

        #region getTeachers
        [HttpPost]
        public HttpResponseMessage getTeacherList(TeacherFilter dataString)
        {
            try
            {
                BALTeachers func = new BALTeachers();
                var list = func.getTeacherList(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region getTeacherDetails
        [HttpPost]
        public HttpResponseMessage getTeacherDetails(TeacherFilter dataString)
        {
            try
            {
                if (!dataString.id.HasValue)
                    return Return.returnHttp("201", "Please Select Student ID.");

                BALTeachers func = new BALTeachers();
                var student = func.getTeacherDetails(dataString);
                return student == null
                ? Return.returnHttp("200", new { message = "No student found" })
                : Return.returnHttp("200", student);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region changeStatus
        [HttpPost]
        public HttpResponseMessage changeStatusTeachers(Teachers dataString)
        {
            try
            {
                if (!dataString.id.HasValue) return Return.returnHttp("201", "Invalid teacher selected.");

                BALTeachers func = new BALTeachers();
                var response = func.changeStatus(dataString);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region delete
        [HttpPost]
        public HttpResponseMessage deleteTeachers(Teachers dataString)
        {
            try
            {
                if (!dataString.id.HasValue) return Return.returnHttp("201", "Invalid teacher selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                dataString.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALTeachers func = new BALTeachers();
                var response = func.deleteStudent(dataString);
                if (response.executionStatus != "TRUE") return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region generateTeacherId
        [HttpPost]
        public HttpResponseMessage generateTeacherId()
        {
            try
            {
                BALTeachers bal = new BALTeachers();
                string teacherId = bal.generateTeacherId();

                return Return.returnHttp("200", teacherId);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region getTeacherDashboardStats
        [HttpPost]
        public HttpResponseMessage getTeacherDashboardStats(TeacherDashboardFilter filter)
        {
            try
            {
                BALTeachers bal = new BALTeachers();
                var list = bal.getTeacherDashboardStats(filter);

                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }
        #endregion
    }
}