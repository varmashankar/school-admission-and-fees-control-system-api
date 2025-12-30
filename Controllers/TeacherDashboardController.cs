using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class TeacherDashboardController : ApiController
    {
        #region getTeacherDashboardStats

        [HttpPost]
        public HttpResponseMessage getTeacherDashboardStats(TeacherDashboardStatsFilter filter)
        {
            try
            {
                BALTeacherDashboard bal = new BALTeacherDashboard();
                var stats = bal.getTeacherDashboardStats(filter);

                return Return.returnHttp("200", stats);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }

        #endregion

        #region getTodaysClasses

        [HttpPost]
        public HttpResponseMessage getTodaysClasses(TeacherDashboardStatsFilter filter)
        {
            try
            {
                BALTeacherDashboard bal = new BALTeacherDashboard();
                var list = bal.getTodaysClasses(filter);

                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }

        #endregion

        #region getUpcomingExams

        [HttpPost]
        public HttpResponseMessage getUpcomingExams(TeacherDashboardStatsFilter filter)
        {
            try
            {
                BALTeacherDashboard bal = new BALTeacherDashboard();
                var list = bal.getUpcomingExams(filter);

                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }

        #endregion

        #region getAttendanceSummary

        [HttpPost]
        public HttpResponseMessage getAttendanceSummary(TeacherDashboardStatsFilter filter)
        {
            try
            {
                BALTeacherDashboard bal = new BALTeacherDashboard();
                var list = bal.getAttendanceSummary(filter);

                return Return.returnHttp("200", list);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Internal issue: " + ex.Message);
            }
        }

        #endregion

        #region getRecentNotices

        [HttpPost]
        public HttpResponseMessage getRecentNotices()
        {
            try
            {
                BALTeacherDashboard bal = new BALTeacherDashboard();
                var list = bal.getRecentNotices();

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
