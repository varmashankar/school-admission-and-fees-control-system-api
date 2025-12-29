using System;

namespace SchoolErpAPI.Models
{
    public class InquiryFollowUp
    {
        public long? id { get; set; }
        public long? inquiryId { get; set; }

        public string followUpAt { get; set; }
        public string channel { get; set; }
        public string remarks { get; set; }

        public bool? isReminded { get; set; }
        public string remindedAt { get; set; }

        public long? createdById { get; set; }
        public long? roleId { get; set; }

        public string createdAt { get; set; }
    }
}
