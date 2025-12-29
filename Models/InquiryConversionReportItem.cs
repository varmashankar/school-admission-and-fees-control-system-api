using System;

namespace SchoolErpAPI.Models
{
    public class InquiryConversionReportItem
    {
        public string source { get; set; }
        public long? totalInquiries { get; set; }
        public long? convertedInquiries { get; set; }
        public decimal? conversionRate { get; set; }

        public long? classId { get; set; }
        public string className { get; set; }

        public long? streamId { get; set; }
        public string streamName { get; set; }

        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }
}
