using System;

namespace SchoolErpAPI.Models
{
    public class FeePaymentRequest
    {
        public int? student_id { get; set; }
        public int? installment_id { get; set; }

        public DateTime? payment_date { get; set; }
        public decimal? amount_paid { get; set; }

        public string payment_mode { get; set; }
        public string transaction_no { get; set; }

        public int? academic_year_id { get; set; }

        public int? created_by_id { get; set; }

        public string status { get; set; }
    }
}
