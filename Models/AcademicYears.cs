using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class AcademicYears
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public string year_code { get; set; }
        public string start_date { get; set; }   // string dates used in controllers
        public string end_date { get; set; }

        public bool is_active { get; set; }
        public bool deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; } = true;

        // meta for auth
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetAcademicYear
    {
        public int? id { get; set; }
        public string creation_timestamp { get; set; }
        public int? created_by_id { get; set; }

        public string year_code { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }

        public bool? is_active { get; set; }
        public bool? deleted { get; set; } = false;
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; } = true;
    }

    public class AcademicYearFilter
    {
        public int? id { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }
}