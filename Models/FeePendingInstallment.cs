using System;

namespace SchoolErpAPI.Models
{
    public class FeePendingInstallment
    {
        public int? installment_id { get; set; }
        public int? fee_structure_id { get; set; }
        public int? installment_no { get; set; }
        public DateTime? due_date { get; set; }
        public decimal? due_amount { get; set; }
        public decimal? paid_amount { get; set; }
        public decimal? balance { get; set; }
        public string status { get; set; }

        public int? days_overdue
        {
            get
            {
                if (!due_date.HasValue) return null;
                return (DateTime.Today - due_date.Value.Date).Days;
            }
        }

        public bool? is_overdue
        {
            get
            {
                if (!due_date.HasValue) return null;
                return due_date.Value.Date < DateTime.Today;
            }
        }
    }
}
