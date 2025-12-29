namespace SchoolErpAPI.Models
{
    public static class InquiryStatus
    {
        public const string New = "NEW";
        public const string Contacted = "CONTACTED";
        public const string FollowUp = "FOLLOW_UP";
        public const string VisitScheduled = "VISIT_SCHEDULED";
        public const string FormSubmitted = "FORM_SUBMITTED";
        public const string Converted = "CONVERTED";
        public const string Lost = "LOST";

        public static bool IsValid(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;

            status = status.Trim();
            return status == New
                   || status == Contacted
                   || status == FollowUp
                   || status == VisitScheduled
                   || status == FormSubmitted
                   || status == Converted
                   || status == Lost;
        }
    }
}
