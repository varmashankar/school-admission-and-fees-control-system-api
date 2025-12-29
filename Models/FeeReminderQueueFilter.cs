using System;

namespace SchoolErpAPI.Models
{
    public class FeeReminderQueueFilter
    {
        public int? classId { get; set; }
        public int? academicYearId { get; set; }
        public DateTime? dueBefore { get; set; }
        public bool? onlyOverdue { get; set; }
        public int? maxRecipients { get; set; }
    }
}
