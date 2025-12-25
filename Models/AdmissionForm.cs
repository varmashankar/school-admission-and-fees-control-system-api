using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    public class AdmissionForm
    {
        // 1) Main student record
        public Students student { get; set; }

        // 2) Parents / Guardian
        public StudentParents father { get; set; }
        public StudentParents mother { get; set; }
        public StudentParents guardian { get; set; } // optional (isGuardian = true)

        // 3) Emergency contact (single entry from your form)
        public StudentEmergencyContacts emergencyContact { get; set; }

        // 4) Previous school (single entry from your form)
        public StudentPreviousSchoolDetails previousSchool { get; set; }

        // 5) Documents (meta only – paths or base64 handled by client/controller)
        public List<StudentDocuments> documents { get; set; }
    }
}