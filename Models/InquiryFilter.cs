using System;

namespace SchoolErpAPI.Models
{
    public class InquiryFilter
    {
        public long? id { get; set; }
        public string inquiryNo { get; set; }

        public string status { get; set; }
        public long? classId { get; set; }
        public long? streamId { get; set; }

        public string fromDate { get; set; }
        public string toDate { get; set; }

        public bool? includeConverted { get; set; }

        public int? pageNo { get; set; }
        public int? pageSize { get; set; }
    }
}
