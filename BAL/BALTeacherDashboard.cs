using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.BAL
{
    public class BALTeacherDashboard
    {
        private readonly SqlConnection con;
        private SqlDataAdapter Adp;
        private DataTable Dt;

        public BALTeacherDashboard()
        {
            con = DBConnection.GlobalConnection();
        }

        #region getTeacherDashboardStats

        public TeacherDashboardStats getTeacherDashboardStats(TeacherDashboardStatsFilter filter)
        {
            var stats = new TeacherDashboardStats();

            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();

                // Get today's day of week (1=Sunday, 2=Monday, etc. in SQL Server)
                int todayDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;

                // Today's Classes Count
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM timetable 
                    WHERE teacher_id = @teacherId 
                    AND day_of_week = @dayOfWeek 
                    AND ISNULL(deleted, 0) = 0 
                    AND ISNULL(status, 1) = 1
                    AND (@academicYearId IS NULL OR academic_year_id = @academicYearId)", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@dayOfWeek", todayDayOfWeek);
                    cmd.Parameters.AddWithValue("@academicYearId", (object)filter?.academicYearId ?? DBNull.Value);
                    stats.todaysClasses = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Total Students in Teacher's Classes
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT s.id) 
                    FROM students s
                    INNER JOIN classes c ON c.id = s.class_id
                    WHERE (c.class_teacher_id = @teacherId 
                           OR c.id IN (SELECT DISTINCT class_id FROM subject_teachers WHERE teacher_id = @teacherId AND ISNULL(status, 1) = 1))
                    AND ISNULL(s.deleted, 0) = 0 
                    AND ISNULL(s.status, 1) = 1", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.totalStudents = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Total Classes Assigned
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT class_id) 
                    FROM subject_teachers 
                    WHERE teacher_id = @teacherId 
                    AND ISNULL(status, 1) = 1", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.totalClassesAssigned = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Total Subjects Assigned
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT subject_id) 
                    FROM subject_teachers 
                    WHERE teacher_id = @teacherId 
                    AND ISNULL(status, 1) = 1", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.totalSubjectsAssigned = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Upcoming Exams Count (next 30 days)
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM exam_subjects es
                    INNER JOIN exams e ON e.id = es.exam_id
                    INNER JOIN subject_teachers st ON st.class_id = es.class_id AND st.subject_id = es.subject_id
                    WHERE st.teacher_id = @teacherId
                    AND es.exam_date >= CAST(GETDATE() AS DATE)
                    AND es.exam_date <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
                    AND ISNULL(es.deleted, 0) = 0
                    AND ISNULL(e.deleted, 0) = 0", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.upcomingExams = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Today's Attendance - Present
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM attendance_details ad
                    INNER JOIN attendance_master am ON am.id = ad.attendance_master_id
                    INNER JOIN classes c ON c.id = am.class_id
                    WHERE am.attendance_date = CAST(GETDATE() AS DATE)
                    AND ad.attendance_status = 'P'
                    AND (c.class_teacher_id = @teacherId 
                         OR c.id IN (SELECT DISTINCT class_id FROM subject_teachers WHERE teacher_id = @teacherId))
                    AND ISNULL(ad.deleted, 0) = 0", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.presentToday = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Today's Attendance - Absent
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM attendance_details ad
                    INNER JOIN attendance_master am ON am.id = ad.attendance_master_id
                    INNER JOIN classes c ON c.id = am.class_id
                    WHERE am.attendance_date = CAST(GETDATE() AS DATE)
                    AND ad.attendance_status = 'A'
                    AND (c.class_teacher_id = @teacherId 
                         OR c.id IN (SELECT DISTINCT class_id FROM subject_teachers WHERE teacher_id = @teacherId))
                    AND ISNULL(ad.deleted, 0) = 0", con))
                {
                    cmd.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                    stats.absentToday = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Calculate attendance percentage
                if (stats.presentToday.HasValue && stats.absentToday.HasValue)
                {
                    int total = stats.presentToday.Value + stats.absentToday.Value;
                    stats.attendancePercentage = total > 0 
                        ? Math.Round((decimal)stats.presentToday.Value / total * 100, 2) 
                        : 0;
                }

                // Pending classes (completed today vs scheduled)
                stats.completedClasses = 0; // Can be expanded with actual tracking
                stats.pendingClasses = stats.todaysClasses;
                stats.pendingAssignments = 0; // Placeholder - needs assignment table

                con.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getTeacherDashboardStats: " + ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return stats;
        }

        #endregion

        #region getTodaysClasses

        public List<TodayClassItem> getTodaysClasses(TeacherDashboardStatsFilter filter)
        {
            var list = new List<TodayClassItem>();

            try
            {
                int todayDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;

                Adp = new SqlDataAdapter(@"
                    SELECT 
                        t.period_no,
                        pm.start_time,
                        pm.end_time,
                        t.class_id,
                        c.class_name,
                        c.section,
                        t.subject_id,
                        s.subject_name,
                        t.room_id,
                        r.room_no
                    FROM timetable t
                    LEFT JOIN classes c ON c.id = t.class_id
                    LEFT JOIN subjects s ON s.id = t.subject_id
                    LEFT JOIN rooms r ON r.id = t.room_id
                    LEFT JOIN period_master pm ON pm.period_no = t.period_no
                    WHERE t.teacher_id = @teacherId
                    AND t.day_of_week = @dayOfWeek
                    AND ISNULL(t.deleted, 0) = 0
                    AND ISNULL(t.status, 1) = 1
                    AND (@academicYearId IS NULL OR t.academic_year_id = @academicYearId)
                    ORDER BY t.period_no", con);

                Adp.SelectCommand.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);
                Adp.SelectCommand.Parameters.AddWithValue("@dayOfWeek", todayDayOfWeek);
                Adp.SelectCommand.Parameters.AddWithValue("@academicYearId", (object)filter?.academicYearId ?? DBNull.Value);

                Dt = new DataTable();
                Adp.Fill(Dt);

                if (Dt.Rows.Count > 0)
                {
                    var columns = new List<string>();
                    foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                    foreach (DataRow row in Dt.Rows)
                        list.Add(Function.BindData<TodayClassItem>(row, columns));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getTodaysClasses: " + ex.Message);
            }

            return list;
        }

        #endregion

        #region getUpcomingExams

        public List<UpcomingExamItem> getUpcomingExams(TeacherDashboardStatsFilter filter)
        {
            var list = new List<UpcomingExamItem>();

            try
            {
                Adp = new SqlDataAdapter(@"
                    SELECT 
                        e.id AS exam_id,
                        e.exam_name,
                        es.subject_id,
                        s.subject_name,
                        es.class_id,
                        c.class_name + ' ' + ISNULL(c.section, '') AS class_name,
                        es.exam_date,
                        es.start_time,
                        es.end_time,
                        es.max_marks
                    FROM exam_subjects es
                    INNER JOIN exams e ON e.id = es.exam_id
                    INNER JOIN subjects s ON s.id = es.subject_id
                    INNER JOIN classes c ON c.id = es.class_id
                    INNER JOIN subject_teachers st ON st.class_id = es.class_id AND st.subject_id = es.subject_id
                    WHERE st.teacher_id = @teacherId
                    AND es.exam_date >= CAST(GETDATE() AS DATE)
                    AND es.exam_date <= DATEADD(DAY, 30, CAST(GETDATE() AS DATE))
                    AND ISNULL(es.deleted, 0) = 0
                    AND ISNULL(e.deleted, 0) = 0
                    ORDER BY es.exam_date ASC", con);

                Adp.SelectCommand.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);

                Dt = new DataTable();
                Adp.Fill(Dt);

                if (Dt.Rows.Count > 0)
                {
                    var columns = new List<string>();
                    foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                    foreach (DataRow row in Dt.Rows)
                        list.Add(Function.BindData<UpcomingExamItem>(row, columns));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getUpcomingExams: " + ex.Message);
            }

            return list;
        }

        #endregion

        #region getAttendanceSummary

        public List<TeacherAttendanceSummaryItem> getAttendanceSummary(TeacherDashboardStatsFilter filter)
        {
            var list = new List<TeacherAttendanceSummaryItem>();

            try
            {
                Adp = new SqlDataAdapter(@"
                    SELECT 
                        c.id AS class_id,
                        c.class_name + ' ' + ISNULL(c.section, '') AS class_name,
                        COUNT(DISTINCT s.id) AS total_students,
                        SUM(CASE WHEN ad.attendance_status = 'P' THEN 1 ELSE 0 END) AS present_count,
                        SUM(CASE WHEN ad.attendance_status = 'A' THEN 1 ELSE 0 END) AS absent_count,
                        CAST(CAST(SUM(CASE WHEN ad.attendance_status = 'P' THEN 1 ELSE 0 END) AS DECIMAL) / 
                             NULLIF(COUNT(*), 0) * 100 AS DECIMAL(5,2)) AS attendance_percentage,
                        am.attendance_date
                    FROM classes c
                    INNER JOIN students s ON s.class_id = c.id AND ISNULL(s.deleted, 0) = 0
                    LEFT JOIN attendance_master am ON am.class_id = c.id AND am.attendance_date = CAST(GETDATE() AS DATE)
                    LEFT JOIN attendance_details ad ON ad.attendance_master_id = am.id AND ad.student_id = s.id
                    WHERE (c.class_teacher_id = @teacherId 
                           OR c.id IN (SELECT DISTINCT class_id FROM subject_teachers WHERE teacher_id = @teacherId))
                    AND ISNULL(c.deleted, 0) = 0
                    GROUP BY c.id, c.class_name, c.section, am.attendance_date
                    ORDER BY c.class_name", con);

                Adp.SelectCommand.Parameters.AddWithValue("@teacherId", (object)filter?.teacherId ?? DBNull.Value);

                Dt = new DataTable();
                Adp.Fill(Dt);

                if (Dt.Rows.Count > 0)
                {
                    var columns = new List<string>();
                    foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                    foreach (DataRow row in Dt.Rows)
                        list.Add(Function.BindData<TeacherAttendanceSummaryItem>(row, columns));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getAttendanceSummary: " + ex.Message);
            }

            return list;
        }

        #endregion

        #region getRecentNotices

        public List<TeacherNoticeItem> getRecentNotices(int limit = 5)
        {
            var list = new List<TeacherNoticeItem>();

            try
            {
                Adp = new SqlDataAdapter(@"
                    SELECT TOP (@limit)
                        id,
                        title,
                        description,
                        publish_date,
                        expire_date
                    FROM notice_board
                    WHERE ISNULL(deleted, 0) = 0
                    AND ISNULL(status, 1) = 1
                    AND (expire_date IS NULL OR expire_date >= CAST(GETDATE() AS DATE))
                    AND publish_date <= GETDATE()
                    ORDER BY publish_date DESC", con);

                Adp.SelectCommand.Parameters.AddWithValue("@limit", limit);

                Dt = new DataTable();
                Adp.Fill(Dt);

                if (Dt.Rows.Count > 0)
                {
                    var columns = new List<string>();
                    foreach (DataColumn col in Dt.Columns) columns.Add(col.ColumnName);

                    foreach (DataRow row in Dt.Rows)
                        list.Add(Function.BindData<TeacherNoticeItem>(row, columns));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in getRecentNotices: " + ex.Message);
            }

            return list;
        }

        #endregion
    }
}
