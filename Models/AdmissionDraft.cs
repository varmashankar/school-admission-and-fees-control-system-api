using System;

namespace SchoolErpAPI.Models
{
    public class AdmissionDraft
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? createdById { get; set; }

        public int? student_id { get; set; }
        public string admission_no { get; set; }

        public int? step_completed { get; set; }
        public bool? submitted { get; set; }

        public bool? deleted { get; set; }
        public int? deletedById { get; set; }
        public string deletedTimestamp { get; set; }
        public bool? status { get; set; } = true;
    }

    public class AdmissionDraftCreateRequest
    {
        public string admission_grade { get; set; }
        public string admission_year { get; set; }
        public string admission_dob { get; set; }

        // Optional starter contact info for OTP path
        public string parent_mobile { get; set; }
        public string parent_email { get; set; }
    }

    public class AdmissionDraftResumeRequest
    {
        public string token { get; set; }
    }

    public class AdmissionDraftSendOtpRequest
    {
        public string admission_no { get; set; }
    }

    public class AdmissionDraftVerifyOtpRequest
    {
        public string admission_no { get; set; }
        public string channel { get; set; } // EMAIL | SMS
        public string destination { get; set; }
        public string otp { get; set; }
    }
}
