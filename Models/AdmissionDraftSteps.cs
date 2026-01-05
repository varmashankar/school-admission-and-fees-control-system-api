namespace SchoolErpAPI.Models
{
    public class SaveStudentStepRequest
    {
        public string resume_token { get; set; }
        public Students student { get; set; }
    }

    public class SaveParentStepRequest
    {
        public string resume_token { get; set; }
        public StudentParents parent { get; set; }
        public string address { get; set; }
    }

    public class SaveAcademicStepRequest
    {
        public string resume_token { get; set; }
        public string student_type { get; set; } // new | transfer
        public StudentPreviousSchoolDetails previous_school { get; set; }
    }

    public class AdmissionDraftPayload
    {
        public AdmissionDraft draft { get; set; }
        public GetStudents student { get; set; }
        public System.Collections.Generic.List<GetStudentParents> parents { get; set; }
        public System.Collections.Generic.List<GetStudentPreviousSchoolDetails> previous_schools { get; set; }
        public System.Collections.Generic.List<StudentDocuments> documents { get; set; }
    }

    public class UploadDraftDocumentRequest
    {
        public string resume_token { get; set; }
        public StudentDocuments document { get; set; }
    }
}
