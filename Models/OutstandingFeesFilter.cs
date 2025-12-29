using System;

namespace SchoolErpAPI.Models
{
    public class OutstandingFeesFilter
    {
        public int? classId { get; set; }
        public int? studentId { get; set; }
        public int? academicYearId { get; set; }

        public DateTime? dueFrom { get; set; }
        public DateTime? dueTo { get; set; }

        public bool? onlyOverdue { get; set; }
    }
}
