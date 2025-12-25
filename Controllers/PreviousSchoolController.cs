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
    public class PreviousSchoolController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveStudentPreviousSchool(StudentPreviousSchoolDetails dataString)
        {
            try
            {
                TimeZoneInfo INDIA = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                dataString.creationTimestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA).ToString("MM/dd/yyyy HH:mm:ss");

                BALPreviousSchool bal = new BALPreviousSchool();
                var resp = bal.saveStudentPreviousSchool(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage getStudentPreviousSchool(StudentPreviousSchoolFilter dataString)
        {
            try
            {
                BALPreviousSchool bal = new BALPreviousSchool();
                var list = bal.getStudentPreviousSchool(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage deleteStudentPreviousSchool(StudentPreviousSchoolDetails dataString)
        {
            try
            {
                BALPreviousSchool bal = new BALPreviousSchool();
                var resp = bal.deleteStudentPreviousSchool(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }
    }
}