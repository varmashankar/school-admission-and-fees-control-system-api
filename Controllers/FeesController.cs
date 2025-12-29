using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using SchoolErpAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class FeesController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage getStudentPendingFees(FeeDueFilter dataString)
        {
            try
            {
                if (dataString == null || !dataString.studentId.HasValue)
                    return Return.returnHttp("201", "Invalid student selected.");

                BALFees bal = new BALFees();
                var list = bal.getPendingFeesByStudent(dataString.studentId.Value);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        private bool TryPopulateAuditFromToken(ref int? userId, ref int? roleId)
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

                int parsedUserId;
                if (tok != null && !string.IsNullOrWhiteSpace(tok.userId) && int.TryParse(tok.userId, out parsedUserId))
                    userId = parsedUserId;

                int parsedRoleId;
                if (tok != null && !string.IsNullOrWhiteSpace(tok.loginAs) && int.TryParse(tok.loginAs, out parsedRoleId))
                    roleId = parsedRoleId;

                return userId.HasValue || roleId.HasValue;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public HttpResponseMessage recordPartialPayment(FeePaymentRequest dataString)
        {
            try
            {
                if (dataString == null) return Return.returnHttp("201", "Invalid request.");

                int? userId = dataString.created_by_id;
                int? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);
                dataString.created_by_id = userId;

                BALFees bal = new BALFees();
                var result = bal.recordPartialPayment(dataString);

                return Return.returnHttp("200", result);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getOutstandingFeesReport(OutstandingFeesFilter dataString)
        {
            try
            {
                BALFees bal = new BALFees();
                var report = bal.getOutstandingFeesReport(dataString);
                return Return.returnHttp("200", report);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage queueFeeWhatsappReminders(FeeReminderQueueFilter dataString)
        {
            try
            {
                if (!SchoolErpAPI.Models.Configuration.IsFeeWhatsappEnabled())
                    return Return.returnHttp("201", "Fee WhatsApp reminders are disabled.");

                if (dataString == null) dataString = new FeeReminderQueueFilter();
                if (!dataString.maxRecipients.HasValue)
                    dataString.maxRecipients = SchoolErpAPI.Models.Configuration.GetFeeWhatsappDefaultQueueMaxRecipients();

                BALFees bal = new BALFees();
                var count = bal.queueFeeWhatsappReminders(dataString);
                return Return.returnHttp("200", new { queuedCount = count });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getFeeWhatsappRemindersToSend(dynamic dataString)
        {
            try
            {
                if (!SchoolErpAPI.Models.Configuration.IsFeeWhatsappEnabled())
                    return Return.returnHttp("201", "Fee WhatsApp reminders are disabled.");

                int maxBatch = SchoolErpAPI.Models.Configuration.GetFeeWhatsappDefaultSendBatchSize();
                try
                {
                    if (dataString != null && dataString.maxBatch != null)
                        maxBatch = (int)dataString.maxBatch;
                }
                catch { }

                BALFees bal = new BALFees();
                var list = bal.getFeeWhatsappRemindersToSend(maxBatch);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage markFeeWhatsappReminderSent(MarkFeeReminderSentRequest dataString)
        {
            try
            {
                if (dataString == null || !dataString.id.HasValue)
                    return Return.returnHttp("201", "Invalid reminder selected.");

                BALFees bal = new BALFees();
                var affected = bal.markFeeWhatsappReminderSent(dataString.id.Value, dataString.providerMessageId);
                return Return.returnHttp("200", new { affected = affected });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage markFeeWhatsappReminderFailed(MarkFeeReminderFailedRequest dataString)
        {
            try
            {
                if (dataString == null || !dataString.id.HasValue)
                    return Return.returnHttp("201", "Invalid reminder selected.");

                if (string.IsNullOrWhiteSpace(dataString.error))
                    dataString.error = "Failed";

                BALFees bal = new BALFees();
                var affected = bal.markFeeWhatsappReminderFailed(dataString.id.Value, dataString.error);
                return Return.returnHttp("200", new { affected = affected });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getFeeHeadList(FeeHeadFilter dataString)
        {
            try
            {
                BALFees bal = new BALFees();
                var list = bal.getFeeHeadList(dataString);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveFeeHead(SaveFeeHeadRequest dataString)
        {
            try
            {
                if (dataString == null) return Return.returnHttp("201", "Invalid request.");

                int? userId = dataString.created_by_id;
                int? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);
                dataString.created_by_id = userId;

                BALFees bal = new BALFees();
                var outIdOrAffected = bal.saveFeeHead(dataString);
                return Return.returnHttp("200", new { result = outIdOrAffected });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteFeeHead(DeleteFeeHeadRequest dataString)
        {
            try
            {
                if (dataString == null || !dataString.id.HasValue) return Return.returnHttp("201", "Invalid request.");

                int? userId = dataString.deleted_by_id;
                int? roleId = null;
                TryPopulateAuditFromToken(ref userId, ref roleId);
                dataString.deleted_by_id = userId;

                BALFees bal = new BALFees();
                var affected = bal.deleteFeeHead(dataString);
                return Return.returnHttp("200", new { affected = affected });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage sendFeeWhatsappRemindersBatch(FeeWhatsappSendBatchRequest dataString)
        {
            try
            {
                if (!SchoolErpAPI.Models.Configuration.IsFeeWhatsappEnabled())
                    return Return.returnHttp("201", "Fee WhatsApp reminders are disabled.");

                int maxBatch = SchoolErpAPI.Models.Configuration.GetFeeWhatsappDefaultSendBatchSize();
                if (dataString != null && dataString.maxBatch.HasValue && dataString.maxBatch.Value > 0)
                    maxBatch = dataString.maxBatch.Value;

                var provider = new WhatsappProviderClient(
                    baseUrl: SchoolErpAPI.Models.Configuration.GetFeeWhatsappProviderBaseUrl(),
                    token: SchoolErpAPI.Models.Configuration.GetFeeWhatsappProviderToken());

                BALFees bal = new BALFees();
                var items = bal.getFeeWhatsappRemindersToSend(maxBatch) ?? new System.Collections.Generic.List<FeeReminderQueueItem>();

                int sent = 0;
                int failed = 0;

                foreach (var item in items)
                {
                    var r = provider.SendText(item.phone, item.message);
                    if (r != null && r.ok)
                    {
                        bal.markFeeWhatsappReminderSent(item.id, r.providerMessageId);
                        sent++;
                    }
                    else
                    {
                        var err = r != null ? r.error : "Failed";
                        if (string.IsNullOrWhiteSpace(err)) err = "Failed";
                        bal.markFeeWhatsappReminderFailed(item.id, err);
                        failed++;
                    }
                }

                return Return.returnHttp("200", new
                {
                    attempted = items.Count,
                    sent = sent,
                    failed = failed
                });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
    }
}
