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
    public class EmergencyContactsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveStudentEmergencyContact(StudentEmergencyContacts dataString)
        {
            try
            {
                TimeZoneInfo INDIA = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                dataString.creationTimestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA).ToString("MM/dd/yyyy HH:mm:ss");

                BALEmergencyContacts bal = new BALEmergencyContacts();
                var resp = bal.saveStudentEmergencyContact(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage getStudentEmergencyContacts(StudentEmergencyFilter dataString)
        {
            try
            {
                BALEmergencyContacts bal = new BALEmergencyContacts();
                var list = bal.getStudentEmergencyContacts(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage deleteStudentEmergencyContact(StudentEmergencyContacts dataString)
        {
            try
            {
                BALEmergencyContacts bal = new BALEmergencyContacts();
                var resp = bal.deleteStudentEmergencyContact(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }
    }
}