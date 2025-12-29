using System;

namespace SchoolErpAPI.Models
{
    public class FeePaymentResult
    {
        public int fee_payment_id { get; set; }
        public string receipt_number { get; set; }
        public DateTime? payment_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal? due_amount_before { get; set; }
        public decimal? paid_amount_before { get; set; }
        public decimal? due_amount_after { get; set; }
        public decimal? paid_amount_after { get; set; }
        public string due_status_after { get; set; }
    }
}
