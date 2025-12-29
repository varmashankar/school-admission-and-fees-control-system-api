using System;

namespace SchoolErpAPI.Models
{
    public class Inquiry
    {
        public int? id { get; set; }

        public string inquiryNo { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }

        public string phone { get; set; }
        public string email { get; set; }

        public int? classId { get; set; }
        public int? streamId { get; set; }

        public string source { get; set; }
        public string notes { get; set; }

        public string status { get; set; }

        public string nextFollowUpAt { get; set; }

        public int? convertedStudentId { get; set; }

        public int? createdById { get; set; }
        public int? roleId { get; set; }

        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }
}
