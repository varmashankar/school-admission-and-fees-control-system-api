using System;

namespace SchoolErpAPI.Models
{
    public class ConversionReportFilter
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }

        public long? classId { get; set; }
        public long? streamId { get; set; }

        public string source { get; set; }
    }
}
