using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
     #region Students (MAIN)

    public class Students
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }

        public string studentCode { get; set; }

        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }

        public string dob { get; set; }
        public string gender { get; set; }
        public string bloodGroup { get; set; }
        public string nationality { get; set; }
        public string religion { get; set; }
        public string nationIdNumber { get; set; }

        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }

        public string address { get; set; }
        public string admissionDate { get; set; }
        public int? classId { get; set; }

        public bool isTransportRequired { get; set; }
        public string siblingInfo { get; set; }
        public string medicalInfo { get; set; }
        public string admissionNo { get; set; }
        public bool deleted { get; set; }
        public int? deletedById { get; set; }
        public string deletedTimestamp { get; set; }
        public bool status { get; set; }

        // nested lists allowed per your choice
        public List<StudentParents> parents { get; set; } = new List<StudentParents>();
        public List<StudentEmergencyContacts> emergencyContacts { get; set; } = new List<StudentEmergencyContacts>();
        public List<StudentPreviousSchoolDetails> previousSchools { get; set; } = new List<StudentPreviousSchoolDetails>();
        public List<StudentDocuments> documents { get; set; } = new List<StudentDocuments>();
    }

    #endregion


    #region GetStudents (RESPONSE)

    public class GetStudents
    {
        public int? id { get; set; }
        public string studentCode { get; set; }

        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }

        public string dob { get; set; }
        public string gender { get; set; }

        public string email { get; set; }
        public string phone { get; set; }

        public string address { get; set; }
        public string admissionDate { get; set; }
        public int? classId { get; set; }
        public string className { get; set; }       // optional joined value

        public bool isTransportRequired { get; set; }
        public string siblingInfo { get; set; }
        public string medicalInfo { get; set; }

        public bool? deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool? status { get; set; }
        public int? classTeacherId { get; set; }
        public string teacherName { get; set; }

        // nested lists preserved (Option 2)
        public List<StudentParents> parents { get; set; } = new List<StudentParents>();
        public List<StudentEmergencyContacts> emergencyContacts { get; set; } = new List<StudentEmergencyContacts>();
        public List<StudentPreviousSchoolDetails> previousSchools { get; set; } = new List<StudentPreviousSchoolDetails>();
        public List<StudentDocuments> documents { get; set; } = new List<StudentDocuments>();
    }

    #endregion


    #region StudentFilter (SEARCH / PAGINATION)

    public class StudentFilter
    {
        public int? id { get; set; }
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public string studentCode { get; set; }
        public int? classId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; } = true;

        public int? pageNo { get; set; }
    }

    #endregion


    #region StudentParents (MAIN)
    public class StudentParents
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? userId { get; set; }
        public int? studentId { get; set; }

        public string parentType { get; set; }   // Father / Mother / Guardian
        public string fullName { get; set; }
        public string mobile { get; set; }
        public string occupation { get; set; }
        public bool isGuardian { get; set; }
        public string relationship { get; set; }
    }
    #endregion


    #region GetStudentParents (RESPONSE)
    public class GetStudentParents
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? studentId { get; set; }
        public string parentType { get; set; }
        public string fullName { get; set; }
        public string mobile { get; set; }
        public string occupation { get; set; }
        public bool isGuardian { get; set; }
        public string relationship { get; set; }
    }
    #endregion


    #region StudentParentFilter
    public class StudentParentFilter
    {
        public int? id { get; set; }
        public int? studentId { get; set; }

        public int? userId { get; set; }
        public int? pageNo { get; set; }
    }
    #endregion



    #region StudentEmergencyContacts (MAIN)
    public class StudentEmergencyContacts
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? userId { get; set; }
        public int? studentId { get; set; }

        public string contactName { get; set; }
        public string contactNumber { get; set; }
        public string relation { get; set; }
    }
    #endregion


    #region GetStudentEmergencyContacts (RESPONSE)
    public class GetStudentEmergencyContacts
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? studentId { get; set; }
        public string contactName { get; set; }
        public string contactNumber { get; set; }
        public string relation { get; set; }

        public string createdByName { get; set; } // optional JOIN
    }
    #endregion


    #region StudentEmergencyFilter
    public class StudentEmergencyFilter
    {
        public int? id { get; set; }
        public int? studentId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }
    #endregion



    #region StudentPreviousSchoolDetails (MAIN)
    public class StudentPreviousSchoolDetails
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? userId { get; set; }
        public int? studentId { get; set; }

        public string schoolName { get; set; }
        public string previousClass { get; set; }
        public string tcNumber { get; set; }
    }
    #endregion


    #region GetStudentPreviousSchoolDetails (RESPONSE)
    public class GetStudentPreviousSchoolDetails
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? studentId { get; set; }
        public string schoolName { get; set; }
        public string previousClass { get; set; }
        public string tcNumber { get; set; }

        public string createdByName { get; set; }  // optional
    }
    #endregion


    #region StudentPreviousSchoolFilter
    public class StudentPreviousSchoolFilter
    {
        public int? id { get; set; }
        public int? studentId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }
    #endregion



    #region StudentDocuments (MAIN)
    public class StudentDocuments
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? userId { get; set; }
        public int? studentId { get; set; }

        public string documentType { get; set; }
        public string filePath { get; set; }
    }
    #endregion


    #region GetStudentDocuments (RESPONSE)
    public class GetStudentDocuments
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? studentId { get; set; }

        public string documentType { get; set; }
        public string filePath { get; set; }

        public string uploadedByName { get; set; }  // optional
    }
    #endregion


    #region StudentDocumentFilter
    public class StudentDocumentFilter
    {
        public int? id { get; set; }
        public int? studentId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }
    #endregion

    #region StudentAdmission

    public class StudentAdmission
    {
        public string student_id { get; set; }
        public string admission_no { get; set; }
        public int? applied_class_id { get; set; }
        public string admission_date { get; set; }
        public string status { get; set; }
        public int? created_by_id { get; set; }
    }


    #endregion

    #region StudentDashboard
    public class GetStudentDashboardStats
    {
        public int? totalStudents { get; set; }
        public int? newAdmissions { get; set; }
        public int? admissionGrowthPercent { get; set; }

        public int? presentToday { get; set; }
        public int? absentToday { get; set; }
    }
    #endregion

    #region StudentDashboardStatsFilter
    public class StudentDashboardStatsFilter
    {
        public int? academicYearId { get; set; }
    }
    #endregion

}