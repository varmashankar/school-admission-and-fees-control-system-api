using System;

namespace SchoolErpAPI.Models
{
    public class InquiryStatusHistory
    {
        public long? id { get; set; }
        public long? inquiryId { get; set; }

        public string fromStatus { get; set; }
        public string toStatus { get; set; }

        public string remarks { get; set; }

        public long? changedById { get; set; }
        public long? roleId { get; set; }

        public DateTime? changedAt { get; set; }
    }
}
