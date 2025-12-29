using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class InquiriesController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage saveInquiry(Inquiry dataString)
        {
            try
            {
                if (dataString == null) return Return.returnHttp("201", "Invalid request.");

                if (!string.IsNullOrWhiteSpace(dataString.status) && !InquiryStatus.IsValid(dataString.status))
                    return Return.returnHttp("201", "Invalid inquiry status.");

                BALInquiries bal = new BALInquiries();
                var response = bal.saveInquiry(dataString);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getInquiryList(InquiryFilter dataString)
        {
            try
            {
                BALInquiries bal = new BALInquiries();
                var list = bal.getInquiryList(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getInquiryDetails(InquiryFilter dataString)
        {
            try
            {
                BALInquiries bal = new BALInquiries();
                var item = bal.getInquiryDetails(dataString);

                return item == null
                    ? Return.returnHttp("200", new { message = "No inquiry found" })
                    : Return.returnHttp("200", item);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        private bool TryPopulateAuditFromToken(ref long? userId, ref long? roleId)
        {
            try
            {
                IEnumerable<string> headers;
                if (Request == null || Request.Headers == null || !Request.Headers.TryGetValues("token", out headers))
                    return false;

                string token = headers.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(token))
                    return false;

                Function fn = new Function();
                var tok = fn.decryptToken(token);

                long parsedUserId;
                if (tok != null && !string.IsNullOrWhiteSpace(tok.userId) && long.TryParse(tok.userId, out parsedUserId))
                    userId = parsedUserId;

                long parsedRoleId;
                if (tok != null && !string.IsNullOrWhiteSpace(tok.loginAs) && long.TryParse(tok.loginAs, out parsedRoleId))
                    roleId = parsedRoleId;

                return userId.HasValue || roleId.HasValue;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public HttpResponseMessage changeInquiryStatus(Inquiry dataString)
        {
            try
            {
                if (!dataString.id.HasValue) return Return.returnHttp("201", "Invalid inquiry selected.");
                if (!InquiryStatus.IsValid(dataString.status)) return Return.returnHttp("201", "Invalid inquiry status.");

                long? userId = null;
                long? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);

                dataString.createdById = userId.HasValue ? (int?)Convert.ToInt32(userId.Value) : (int?)null;
                dataString.roleId = roleId.HasValue ? (int?)Convert.ToInt32(roleId.Value) : (int?)null;

                BALInquiries bal = new BALInquiries();
                var response = bal.changeInquiryStatus(dataString);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getInquiryStatusHistory(InquiryFilter dataString)
        {
            try
            {
                if (dataString == null || !dataString.id.HasValue)
                    return Return.returnHttp("201", "Invalid inquiry selected.");

                BALInquiries bal = new BALInquiries();
                var list = bal.getInquiryStatusHistory(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveInquiryStatusHistory(InquiryStatusHistory dataString)
        {
            try
            {
                if (dataString == null || !dataString.inquiryId.HasValue)
                    return Return.returnHttp("201", "Invalid inquiry selected.");

                if (!InquiryStatus.IsValid(dataString.toStatus))
                    return Return.returnHttp("201", "Invalid inquiry status.");

                BALInquiries bal = new BALInquiries();
                var response = bal.addInquiryStatusHistory(dataString);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveInquiryFollowUp(InquiryFollowUp dataString)
        {
            try
            {
                if (!dataString.inquiryId.HasValue) return Return.returnHttp("201", "Invalid inquiry selected.");

                long? userId = null;
                long? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);

                dataString.createdById = userId.HasValue ? (int?)Convert.ToInt32(userId.Value) : (int?)null;
                dataString.roleId = roleId.HasValue ? (int?)Convert.ToInt32(roleId.Value) : (int?)null;

                BALInquiries bal = new BALInquiries();
                var response = bal.saveInquiryFollowUp(dataString);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getDueFollowUps(InquiryFilter dataString)
        {
            try
            {
                DateTime? dueBefore = null;
                if (dataString != null && !string.IsNullOrWhiteSpace(dataString.toDate))
                {
                    DateTime parsed;
                    if (DateTime.TryParse(dataString.toDate, out parsed))
                        dueBefore = parsed;
                }

                BALInquiries bal = new BALInquiries();
                var list = bal.getDueFollowUps(dueBefore);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage markFollowUpReminded(MarkFollowUpRemindedRequest dataString)
        {
            try
            {
                if (dataString == null || !dataString.followUpId.HasValue)
                    return Return.returnHttp("201", "Invalid follow-up selected.");

                if (!dataString.remindedAt.HasValue)
                    dataString.remindedAt = DateTime.UtcNow;

                long? userId = null;
                long? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);

                dataString.userId = userId;
                dataString.roleId = roleId;

                BALInquiries bal = new BALInquiries();
                var response = bal.markFollowUpReminded(dataString);

                if (response.executionStatus != "TRUE")
                    return Return.returnHttp("201", response.message);

                return Return.returnHttp("200", response.message);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getConversionReport(ConversionReportFilter dataString)
        {
            try
            {
                BALInquiries bal = new BALInquiries();
                var list = bal.getConversionReport(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
    }
}
