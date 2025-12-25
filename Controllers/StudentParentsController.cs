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
    public class StudentParentsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveStudentParents(StudentParents dataString)
        {
            try
            {
                TimeZoneInfo INDIA = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime dt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA);
                dataString.creationTimestamp = dt.ToString("MM/dd/yyyy HH:mm:ss");

                BALStudentParents bal = new BALStudentParents();
                var resp = bal.saveStudentParents(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage getStudentParents(StudentParentFilter dataString)
        {
            try
            {
                BALStudentParents bal = new BALStudentParents();
                var list = bal.getStudentParents(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex) 
            { 
                //return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
                System.Diagnostics.Debug.WriteLine("SQL ERROR >>> " + ex.Message);
                throw;
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteStudentParent(StudentParentFilter dataString)
        {
            try
            {
                BALStudentParents bal = new BALStudentParents();
                var resp = bal.deleteStudentParent(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }
    }
}