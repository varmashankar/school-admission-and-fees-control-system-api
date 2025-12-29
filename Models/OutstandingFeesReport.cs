using System.Collections.Generic;

namespace SchoolErpAPI.Models
{
    public class OutstandingFeesReport
    {
        public decimal total_due { get; set; }
        public decimal total_paid { get; set; }
        public decimal total_outstanding { get; set; }

        public int total_students { get; set; }
        public int total_installments { get; set; }

        public List<OutstandingFeeItem> items { get; set; }
    }
}
