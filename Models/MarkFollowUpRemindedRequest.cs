using System;

namespace SchoolErpAPI.Models
{
    public class MarkFollowUpRemindedRequest
    {
        public long? followUpId { get; set; }
        public DateTime? remindedAt { get; set; }

        public long? userId { get; set; }
        public long? roleId { get; set; }
    }
}
