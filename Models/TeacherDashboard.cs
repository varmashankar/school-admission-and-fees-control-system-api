using System;
using System.Collections.Generic;

namespace SchoolErpAPI.Models
{
    #region TeacherDashboardFilter

    public class TeacherDashboardStatsFilter
    {
        public int? teacherId { get; set; }
        public int? academicYearId { get; set; }
    }

    #endregion

    #region TeacherDashboardStats

    public class TeacherDashboardStats
    {
        // Today's schedule
        public int? todaysClasses { get; set; }
        public int? completedClasses { get; set; }
        public int? pendingClasses { get; set; }

        // Students
        public int? totalStudents { get; set; }
        public int? presentToday { get; set; }
        public int? absentToday { get; set; }

        // Assignments/Exams
        public int? pendingAssignments { get; set; }
        public int? upcomingExams { get; set; }

        // Attendance summary
        public decimal? attendancePercentage { get; set; }

        // Quick counts
        public int? totalClassesAssigned { get; set; }
        public int? totalSubjectsAssigned { get; set; }
    }

    #endregion

    #region TodayClassItem

    public class TodayClassItem
    {
        public int? periodNo { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public int? classId { get; set; }
        public string className { get; set; }
        public string section { get; set; }
        public int? subjectId { get; set; }
        public string subjectName { get; set; }
        public int? roomId { get; set; }
        public string roomNo { get; set; }
        public bool? isCompleted { get; set; }
    }

    #endregion

    #region RecentActivityItem

    public class TeacherRecentActivityItem
    {
        public int? id { get; set; }
        public string activityType { get; set; }
        public string description { get; set; }
        public string timestamp { get; set; }
        public string relatedEntity { get; set; }
        public int? relatedEntityId { get; set; }
    }

    #endregion

    #region UpcomingExamItem

    public class UpcomingExamItem
    {
        public int? examId { get; set; }
        public string examName { get; set; }
        public int? subjectId { get; set; }
        public string subjectName { get; set; }
        public int? classId { get; set; }
        public string className { get; set; }
        public string examDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public decimal? maxMarks { get; set; }
    }

    #endregion

    #region AttendanceSummaryItem

    public class TeacherAttendanceSummaryItem
    {
        public int? classId { get; set; }
        public string className { get; set; }
        public int? totalStudents { get; set; }
        public int? presentCount { get; set; }
        public int? absentCount { get; set; }
        public decimal? attendancePercentage { get; set; }
        public string attendanceDate { get; set; }
    }

    #endregion

    #region StudentPerformanceItem

    public class StudentPerformanceItem
    {
        public int? studentId { get; set; }
        public string studentName { get; set; }
        public string studentCode { get; set; }
        public int? classId { get; set; }
        public string className { get; set; }
        public decimal? averageMarks { get; set; }
        public string grade { get; set; }
        public decimal? attendancePercentage { get; set; }
    }

    #endregion

    #region NoticeItem

    public class TeacherNoticeItem
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string publishDate { get; set; }
        public string expireDate { get; set; }
    }

    #endregion
}
