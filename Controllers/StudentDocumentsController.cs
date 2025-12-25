using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class StudentDocumentsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveStudentDocument(StudentDocuments dataString)
        {
            try
            {
                TimeZoneInfo INDIA = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                dataString.creationTimestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA).ToString("MM/dd/yyyy HH:mm:ss");

                // If file_path contains base64, decode & save to disk here (controller-level) or let BAL do it.
                if (!string.IsNullOrEmpty(dataString.filePath) && dataString.filePath.StartsWith("data:"))
                {
                    try
                    {
                        var idx = dataString.filePath.IndexOf("base64,");
                        string base64 = dataString.filePath;
                        if (idx >= 0) base64 = dataString.filePath.Substring(idx + 7);

                        byte[] bytes = Convert.FromBase64String(base64);
                        string folder = System.Web.Hosting.HostingEnvironment.MapPath("~/uploads/documents/" + dataString.studentId+ "/");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        string ext = ".bin";
                        if (dataString.filePath.StartsWith("data:"))
                        {
                            int a = dataString.filePath.IndexOf(":");
                            int b = dataString.filePath.IndexOf(";");
                            if (a >= 0 && b > a)
                            {
                                var mime = dataString.filePath.Substring(a + 1, b - a - 1);
                                if (mime.Contains("/"))
                                {
                                    var parts = mime.Split('/');
                                    ext = "." + parts[1];
                                }
                            }
                        }

                        string fileName = Guid.NewGuid().ToString() + ext;
                        string fullPath = Path.Combine(folder, fileName);
                        File.WriteAllBytes(fullPath, bytes);
                        dataString.filePath = "/uploads/documents/" + dataString.studentId + "/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        return Return.returnHttp("201", "File decode/save failed: " + ex.Message);
                    }
                }

                BALStudentDocuments bal = new BALStudentDocuments();
                var resp = bal.saveStudentDocument(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage getStudentDocuments(StudentDocuments dataString)
        {
            try
            {
                BALStudentDocuments bal = new BALStudentDocuments();
                var list = bal.getStudentDocuments(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }

        [HttpPost]
        public HttpResponseMessage deleteStudentDocument(StudentDocuments dataString)
        {
            try
            {
                BALStudentDocuments bal = new BALStudentDocuments();
                var resp = bal.deleteStudentDocument(dataString);
                if (resp.executionStatus != "TRUE") return Return.returnHttp("201", resp.message);
                return Return.returnHttp("200", resp.message);
            }
            catch (Exception ex) { return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message); }
        }
    }
}