using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class DashboardController : ApiController
    {
        #region getDashboard
        [HttpPost]
        public HttpResponseMessage getDashboardStats(DashboardStatsFilter filter)
        {
            try
            {
                BALDashboard balDash = new BALDashboard();
                var list = balDash.getDashboardStats(filter);

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