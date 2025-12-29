using System;

namespace SchoolErpAPI.Models
{
    public class OutstandingFeeItem
    {
        public int student_id { get; set; }
        public string student_code { get; set; }
        public string student_name { get; set; }

        public int class_id { get; set; }
        public string class_name { get; set; }

        public int installment_id { get; set; }
        public int installment_no { get; set; }
        public DateTime due_date { get; set; }

        public decimal due_amount { get; set; }
        public decimal paid_amount { get; set; }
        public decimal outstanding_amount { get; set; }

        public int days_overdue { get; set; }
    }
}
