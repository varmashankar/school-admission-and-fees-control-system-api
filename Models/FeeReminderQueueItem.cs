using System;

namespace SchoolErpAPI.Models
{
    public class FeeReminderQueueItem
    {
        public int id { get; set; }
        public int student_id { get; set; }
        public int installment_id { get; set; }
        public DateTime? due_date { get; set; }
        public decimal outstanding_amount { get; set; }
        public string phone { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public int try_count { get; set; }
        public DateTime? last_try_at { get; set; }
    }
}
