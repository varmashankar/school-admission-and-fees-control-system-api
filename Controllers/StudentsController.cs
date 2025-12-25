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
    public class StudentsController : ApiController
    {
        #region helpers
        public string CreateToken(int? id, int? roleType)
        {
            return Function.CreateToken((int)roleType, id);
        }
        #endregion

        #region saveStudents
        [HttpPost]
        public HttpResponseMessage saveStudents(Students dataString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataString.firstName))
                    return Return.returnHttp("201", new { message = "Please Enter Student First Name." });

                // Set creation timestamp
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                dataString.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                // Random Password
                Random random = new Random();
                int[] a = {
            random.Next(57), random.Next(57), random.Next(57), random.Next(57),
            random.Next(57), random.Next(57), random.Next(57), random.Next(57)
        };
                dataString.password = Function.CreatePassword(a);

                BALStudents bal = new BALStudents();
                SPResponse response = bal.saveStudent(dataString);

                // -------------------------------------------------------------------
                //          IF STUDENT SAVED SUCCESSFULLY → INSERT ADMISSION RECORD
                // -------------------------------------------------------------------
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


        #region getStudentList
        [HttpPost]
        public HttpResponseMessage getStudentList(StudentFilter dataString)
        {
            try
            {
                BALStudents func = new BALStudents();
                var list = func.getStudentList(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        #region getStudentDetails
        [HttpPost]
        public HttpResponseMessage getStudentDetails(StudentFilter dataString)
        {
            try
            {
                //if (!dataString.id.HasValue)
                //    return Return.returnHttp("201", "Please Select Student ID.");

                BALStudents func = new BALStudents();
                var student = func.getStudentDetails(dataString);
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

        #region changeStatusStudents
        [HttpPost]
        public HttpResponseMessage changeStatusStudents(Students dataString)
        {
            try
            {
                if (!dataString.id.HasValue) return Return.returnHttp("201", "Invalid student selected.");

                BALStudents func = new BALStudents();
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

        #region deleteStudents
        [HttpPost]
        public HttpResponseMessage deleteStudents(Students dataString)
        {
            try
            {
                if (!dataString.id.HasValue) return Return.returnHttp("201", "Invalid student selected.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                dataString.deletedTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                BALStudents func = new BALStudents();
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

        #region generateAdmissionId
        [HttpPost]
        public HttpResponseMessage generateAdmissionId()
        {
            try
            {
                BALStudents bal = new BALStudents();
                string admissionId = bal.generateAdmissionId();

                return Return.returnHttp("200", admissionId);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
        #endregion

        [HttpPost]
        public HttpResponseMessage generateStudentId()
        {
            try
            {
                BALStudents bal = new BALStudents();
                string studentId = bal.generateStudentId();

                return Return.returnHttp("200", studentId);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occurred. " + ex.Message);
            }
        }

        #region getStudentDashboardStats
        [HttpPost]
        public HttpResponseMessage getStudentDashboardStats(StudentDashboardStatsFilter filter)
        {
            try
            {
                BALStudents bal = new BALStudents();
                var list = bal.getStudentDashboardStats(filter);

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