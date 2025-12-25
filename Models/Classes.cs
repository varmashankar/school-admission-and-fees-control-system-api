using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class Classes
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public string className { get; set; }
        public string section { get; set; }
        public string classCode { get; set; }

        public int? classTeacherId { get; set; }
        public int? roomId { get; set; }
        public int? streamId { get; set; }

        public bool? deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }

        public bool? status { get; set; }

        public int? schoolId { get; set; } = null;
        public string board { get; set; }
        public string medium { get; set; }

        public int? academicYearId { get; set; }

        public int? userId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetClasses
    {
        public int? id { get; set; }
        public string createdTimestamp { get; set; }
        public string className { get; set; }
        public string section { get; set; }
        public string classCode { get; set; }
        public int? classTeacherId { get; set; }
        public int? roomId { get; set; }
        public int? streamId { get; set; }
        public string board { get; set; }
        public string medium { get; set; }
        public string teacherName { get; set; }
        public int? studentCount { get; set; }
        public string roomName { get; set; }
        public string streamName { get; set; }
        public bool? status { get; set; }
    }

    public class ClassFilter
    {
        public int? id { get; set; }
        public int? schoolId { get; set; }
        public bool? deleted { get; set; }
        public bool? status { get; set; }
        public int? academicYearId { get; set; }
        public int? pageNo { get; set; }
    }

}