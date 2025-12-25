using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class Dashboard
    {
    }

    public class GetDashboardStats 
    {
        public int? totalStudents { get; set; }
        public int? totalTeachers { get; set; }
        public int? feesCollectedInLakhs { get; set; }
        public int? upcomingEvents { get; set; }
    }
    public class DashboardStatsFilter
    {
        public int? academicYearId { get; set; }
    }

}