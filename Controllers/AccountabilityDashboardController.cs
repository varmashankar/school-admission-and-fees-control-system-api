using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class AccountabilityDashboardController : ApiController
    {
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

        private HttpResponseMessage EnsureAccess(string actionName)
        {
            int? userId = null;
            int? roleTypeId = null;
            TryPopulateAuditFromToken(ref userId, ref roleTypeId);

            Function fn = new Function();
            // IMPORTANT: action_names.controller_name is stored as DeclaringType.Name, i.e. "<ControllerName>Controller"
            string access = fn.checkUserAccess(userId, roleTypeId, "AccountabilityDashboardController", actionName);
            if (!string.Equals(access, "TRUE", StringComparison.OrdinalIgnoreCase))
                return Return.returnHttp("201", access);

            return null;
        }

        private static void DebugLogMissedInquiriesShape(object list)
        {
            try
            {
#if DEBUG
                if (list == null)
                {
                    System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries: list is null");
                    return;
                }

                // Serialize with same settings as API (ignores null/default), so we capture the *actual* JSON-shape.
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                    list,
                    new Newtonsoft.Json.JsonSerializerSettings
                    {
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore
                    });

                var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                if (token.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries: payload is not an array. Type=" + token.Type);
                    return;
                }

                var arr = (Newtonsoft.Json.Linq.JArray)token;
                int total = arr.Count;

                int withAssigned = arr.Count(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object && ((Newtonsoft.Json.Linq.JObject)t).Property("assignedToStaffName", StringComparison.OrdinalIgnoreCase) != null);
                int withoutAssigned = total - withAssigned;

                var keySets = arr
                    .Where(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    .Select(t => (Newtonsoft.Json.Linq.JObject)t)
                    .Select(o => string.Join(",", o.Properties().Select(p => p.Name).OrderBy(n => n, StringComparer.OrdinalIgnoreCase)))
                    .GroupBy(s => s)
                    .Select(g => new { keys = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToList();

                System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries: rows=" + total + ", assignedToStaffName present=" + withAssigned + ", missing=" + withoutAssigned);
                foreach (var ks in keySets)
                {
                    System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries: keys(count=" + ks.count + ")=" + ks.keys);
                }

                var missingExamples = arr
                    .Where(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    .Select(t => (Newtonsoft.Json.Linq.JObject)t)
                    .Where(o => o.Property("assignedToStaffName", StringComparison.OrdinalIgnoreCase) == null)
                    .Take(5)
                    .Select(o => new
                    {
                        inquiryId = o.Property("inquiryId", StringComparison.OrdinalIgnoreCase)?.Value?.ToString(),
                        assignedToStaffId = o.Property("assignedToStaffId", StringComparison.OrdinalIgnoreCase)?.Value?.ToString(),
                        assignedTo = o.Property("assignedTo", StringComparison.OrdinalIgnoreCase)?.Value?.ToString(),
                        staffId = o.Property("staffId", StringComparison.OrdinalIgnoreCase)?.Value?.ToString()
                    })
                    .ToList();

                foreach (var ex in missingExamples)
                {
                    System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries missing assignedToStaffName example: inquiryId=" + ex.inquiryId + ", assignedToStaffId=" + ex.assignedToStaffId + ", assignedTo=" + ex.assignedTo + ", staffId=" + ex.staffId);
                }
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("[AccountabilityDashboard] getMissedInquiries shape log failed: " + ex.Message);
#endif
            }
        }

        [HttpPost]
        public HttpResponseMessage getStaffFollowUpSummary(AccountabilityDashboardFilter filter)
        {
            try
            {
                var denied = EnsureAccess("getStaffFollowUpSummary");
                if (denied != null) return denied;

                BALAccountabilityDashboard bal = new BALAccountabilityDashboard();
                var list = bal.getStaffFollowUpSummary(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getMissedInquiries(AccountabilityDashboardFilter filter)
        {
            try
            {
                var denied = EnsureAccess("getMissedInquiries");
                if (denied != null) return denied;

                BALAccountabilityDashboard bal = new BALAccountabilityDashboard();
                var list = bal.getMissedInquiries(filter);

                DebugLogMissedInquiriesShape(list);

                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getAdmissionLossReasons(AccountabilityDashboardFilter filter)
        {
            try
            {
                var denied = EnsureAccess("getAdmissionLossReasons");
                if (denied != null) return denied;

                BALAccountabilityDashboard bal = new BALAccountabilityDashboard();
                var list = bal.getAdmissionLossReasons(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage getFeeCollectionDelays(AccountabilityDashboardFilter filter)
        {
            try
            {
                var denied = EnsureAccess("getFeeCollectionDelays");
                if (denied != null) return denied;

                BALAccountabilityDashboard bal = new BALAccountabilityDashboard();
                var list = bal.getFeeCollectionDelays(filter);
                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

#if DEBUG
        [HttpPost]
        public HttpResponseMessage debug_getMissedInquiriesShape(AccountabilityDashboardFilter filter)
        {
            try
            {
                var denied = EnsureAccess("getMissedInquiries");
                if (denied != null) return denied;

                BALAccountabilityDashboard bal = new BALAccountabilityDashboard();
                var list = bal.getMissedInquiries(filter);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                    list,
                    new Newtonsoft.Json.JsonSerializerSettings
                    {
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore
                    });

                var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                if (token.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                    return Return.returnHttp("200", new { rows = 0, error = "Payload is not an array", tokenType = token.Type.ToString() });

                var arr = (Newtonsoft.Json.Linq.JArray)token;
                int total = arr.Count;
                int withAssigned = arr.Count(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object && ((Newtonsoft.Json.Linq.JObject)t).Property("assignedToStaffName", StringComparison.OrdinalIgnoreCase) != null);

                var topKeySets = arr
                    .Where(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    .Select(t => (Newtonsoft.Json.Linq.JObject)t)
                    .Select(o => o.Properties().Select(p => p.Name).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray())
                    .GroupBy(keys => string.Join(",", keys))
                    .Select(g => new { keys = g.Key.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToList();

                var missingExamples = arr
                    .Where(t => t.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    .Select(t => (Newtonsoft.Json.Linq.JObject)t)
                    .Where(o => o.Property("assignedToStaffName", StringComparison.OrdinalIgnoreCase) == null)
                    .Take(20)
                    .Select(o => new
                    {
                        inquiryId = o.Property("inquiryId", StringComparison.OrdinalIgnoreCase)?.Value,
                        keys = o.Properties().Select(p => p.Name).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray(),
                        assignedToStaffId = o.Property("assignedToStaffId", StringComparison.OrdinalIgnoreCase)?.Value,
                        assignedTo = o.Property("assignedTo", StringComparison.OrdinalIgnoreCase)?.Value,
                        staffId = o.Property("staffId", StringComparison.OrdinalIgnoreCase)?.Value
                    })
                    .ToList();

                return Return.returnHttp("200", new
                {
                    rows = total,
                    assignedToStaffNamePresent = withAssigned,
                    assignedToStaffNameMissing = total - withAssigned,
                    topKeySets = topKeySets,
                    missingExamples = missingExamples
                });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
#endif
    }
}
