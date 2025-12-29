using System;

namespace SchoolErpAPI.Models
{
    public class AccountabilityDashboardFilter
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int? academicYearId { get; set; }
        public int? staffId { get; set; }
        public int? maxDaysOverdue { get; set; }
    }

    public class StaffFollowUpSummaryItem
    {
        public int? staffId { get; set; }
        public string staffName { get; set; }
        public int? dueFollowUps { get; set; }
        public int? overdueFollowUps { get; set; }
        public DateTime? lastFollowUpAt { get; set; }
    }

    public class MissedInquiryItem
    {
        public int? inquiryId { get; set; }
        public string studentName { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
        public DateTime? inquiryDate { get; set; }
        public DateTime? nextFollowUpAt { get; set; }
        public int? assignedToStaffId { get; set; }
        public string assignedToStaffName { get; set; }
        public int? daysOverdue { get; set; }
    }

    public class AdmissionLossReasonItem
    {
        public string reason { get; set; }
        public int? count { get; set; }
    }

    public class FeeCollectionDelayItem
    {
        public int? studentId { get; set; }
        public string studentName { get; set; }
        public string className { get; set; }
        public decimal? totalDue { get; set; }
        public DateTime? dueDate { get; set; }
        public int? daysOverdue { get; set; }
        public string lastReminderStatus { get; set; }
        public DateTime? lastReminderAt { get; set; }
    }
}
