using System;

namespace SchoolErpAPI.Models
{
    public class School
    {
        public int? id { get; set; }

        public string school_name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string logo_path { get; set; }

        public bool? deleted { get; set; } = false;
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }

        public bool? status { get; set; } = true;

        public string creationTimestamp { get; set; }
        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetSchool
    {
        public int? id { get; set; }
        public string school_name { get; set; }
        public string code { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string logo_path { get; set; }
        public bool? deleted { get; set; }
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; }
    }

    public class SchoolFilter
    {
        public int? id { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }
}
