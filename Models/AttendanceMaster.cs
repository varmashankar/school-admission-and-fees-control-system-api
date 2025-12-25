    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class AttendanceMaster
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }

        public int? classId { get; set; }
        public string attendanceDate { get; set; }   // string format used in controllers
        public int? takenByTeacherId { get; set; }
        public int? academicYearId { get; set; }

        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; }

        // auth meta
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetAttendanceMaster
    {
        public int? id { get; set; }
        public string creation_timestamp { get; set; }
        public int? created_by_id { get; set; }

        public int? class_id { get; set; }
        public string attendance_date { get; set; }
        public int? taken_by_teacher_id { get; set; }
        public int? academic_year_id { get; set; }

        public bool? deleted { get; set; }
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; }
    }

    public class AttendanceMasterFilter
    {
        public int? id { get; set; }
        public int? classId { get; set; }
        public string attendanceDate { get; set; }
        public int? academicYearId { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }

    // details
    public class AttendanceDetails
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }

        public int? attendanceMasterId { get; set; }
        public int? studentId { get; set; }
        public string attendanceStatus { get; set; }  // 'P'/'A' etc.
        public string remarks { get; set; }

        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; }

        // auth meta
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }
    }

    public class GetAttendanceDetails
    {
        public int? id { get; set; }
        public string creation_timestamp { get; set; }
        public int? created_by_id { get; set; }

        public int? attendance_master_id { get; set; }
        public int? student_id { get; set; }
        public string attendance_status { get; set; }
        public string remarks { get; set; }

        public bool? deleted { get; set; }
        public string deleted_timestamp { get; set; }
        public int? deleted_by_id { get; set; }
        public bool? status { get; set; }
    }

    public class AttendanceDetailsFilter
    {
        public int? id { get; set; }
        public int? attendanceMasterId { get; set; }
        public int? studentId { get; set; }
        public string attendanceStatus { get; set; }
        public bool? deleted { get; set; } = false;
        public bool? status { get; set; } = true;
        public int? pageNo { get; set; }
    }
}