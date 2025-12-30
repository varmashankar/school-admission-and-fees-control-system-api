using System;

namespace SchoolErpAPI.Models
{
    public class InquiryFollowUp
    {
        public int? id { get; set; }
        public int? inquiryId { get; set; }

        public string followUpAt { get; set; }
        public string channel { get; set; }
        public string remarks { get; set; }

        public bool? isReminded { get; set; }
        public string remindedAt { get; set; }

        public int? userId { get; set; }
        public int? roleId { get; set; }

        public string createdAt { get; set; }
    }
}
