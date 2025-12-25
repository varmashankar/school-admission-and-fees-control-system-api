using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.Models
{
    #region Teachers (MAIN)

    public class Teachers
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }
        public int? userId { get; set; }
        public int? roleId { get; set; }
        public int? roleTypeId { get; set; }

        public string teacherId { get; set; }     // T-2025-001 format

        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }

        public string email { get; set; }
        public string mobile { get; set; }
        public string password { get; set; }

        public string dob { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string religion { get; set; }
        public string nationIdNumber { get; set; }

        public string dateOfJoining { get; set; }
        public string designation { get; set; }

        public int? departmentId { get; set; }
        public string primarySubject { get; set; }
        public int? experienceYears { get; set; }

        public bool deleted { get; set; }
        public string deletedTimestamp { get; set; }
        public int? deletedById { get; set; }
        public bool status { get; set; }

    }

    #endregion

    #region GetTeachers (RESPONSE)

    public class GetTeachers
    {
        public int? id { get; set; }
        public int? userId { get; set; }

        public string teacherId { get; set; }

        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }

        public string email { get; set; }
        public string mobile { get; set; }

        public string dob { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string religion { get; set; }
        public string nationIdNumber { get; set; }

        public string dateOfJoining { get; set; }
        public string designation { get; set; }

        public int? departmentId { get; set; }
        public string departmentName { get; set; }   // JOINED VALUE

        public string primarySubject { get; set; }
        public int? experienceYears { get; set; }

        public bool? deleted { get; set; }
        public int? deletedById { get; set; }
        public string deletedTimestamp { get; set; }
        public bool? status { get; set; }

        public List<TeacherContacts> contacts { get; set; } = new List<TeacherContacts>();
        public List<TeacherAcademics> academics { get; set; } = new List<TeacherAcademics>();
        public List<TeacherBankDetails> bankDetails { get; set; } = new List<TeacherBankDetails>();
        public List<TeacherDocuments> documents { get; set; } = new List<TeacherDocuments>();
    }

    #endregion

    #region TeacherFilter

    public class TeacherFilter
    {
        public int? id { get; set; }
        public int? userId { get; set; }
        public int? departmentId { get; set; }
        public bool? deleted { get; set; } = false;
        public int? deletedById { get; set; }
        public bool? status { get; set; } = true;

        public int? pageNo { get; set; }
    }

    #endregion

    #region TeacherContacts

    public class TeacherContacts
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? createdById { get; set; }
        public int? teacherId { get; set; }

        public string email { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }

        public string emergencyPerson { get; set; }
        public string emergencyNumber { get; set; }
        public string emergencyRelation { get; set; }
    }

    #endregion

    #region GetTeacherContacts (RESPONSE)

    public class GetTeacherContacts
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? teacherId { get; set; }

        public string email { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }

        public string emergencyPerson { get; set; }
        public string emergencyNumber { get; set; }
        public string emergencyRelation { get; set; }

        public string createdByName { get; set; }    // optional JOIN
    }

    #endregion

    #region TeacherContactsFilter

    public class TeacherContactsFilter
    {
        public int? id { get; set; }
        public int? teacherId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }

    #endregion

    #region TeacherAcademics

    public class TeacherAcademics
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? createdById { get; set; }
        public int? teacherId { get; set; }

        public string qualification { get; set; }
        public string university { get; set; }
        public string otherCertifications { get; set; }
        public bool isHighestQualification { get; set; }
    }

    #endregion

    #region GetTeacherAcademics (RESPONSE)

    public class GetTeacherAcademics
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? teacherId { get; set; }

        public string qualification { get; set; }
        public string university { get; set; }
        public string otherCertifications { get; set; }
        public bool isHighestQualification { get; set; }

        public string createdByName { get; set; }    // JOINED VALUE
    }

    #endregion

    #region TeacherAcademicsFilter

    public class TeacherAcademicsFilter
    {
        public int? id { get; set; }
        public int? teacherId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }

    #endregion

    #region TeacherBankDetails

    public class TeacherBankDetails
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? createdById { get; set; }
        public int? teacherId { get; set; }

        public string bankName { get; set; }
        public string accountNumber { get; set; }
        public string ifsc { get; set; }
        public string pan { get; set; }
    }

    #endregion

    #region GetTeacherBankDetails (RESPONSE)

    public class GetTeacherBankDetails
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? teacherId { get; set; }

        public string bankName { get; set; }
        public string accountNumber { get; set; }
        public string ifsc { get; set; }
        public string pan { get; set; }

        public string createdByName { get; set; }    // optional
    }

    #endregion

    #region TeacherBankDetailsFilter

    public class TeacherBankDetailsFilter
    {
        public int? id { get; set; }
        public int? teacherId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }

    #endregion

    #region TeacherDocuments

    public class TeacherDocuments
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? createdById { get; set; }
        public int? teacherId { get; set; }

        public string documentType { get; set; }
        public string filePath { get; set; }
    }

    #endregion

    #region GetTeacherDocuments (RESPONSE)

    public class GetTeacherDocuments
    {
        public int? id { get; set; }
        public string creationTimestamp { get; set; }

        public int? teacherId { get; set; }

        public string documentType { get; set; }
        public string filePath { get; set; }

        public string uploadedByName { get; set; }    // optional
    }

    #endregion

    #region TeacherDocumentsFilter

    public class TeacherDocumentsFilter
    {
        public int? id { get; set; }
        public int? teacherId { get; set; }

        public bool? deleted { get; set; } = false;
        public int? pageNo { get; set; }
    }

    #endregion

    #region getTeacherDashboardStats
    public class GetTeacherDashboardStats
    {
        public int? totalTeachers { get; set; }
        public int? newHiresThisMonth { get; set; }
        public int? activeCourses { get; set; }
    }
    #endregion

    #region TeacherDashboardFilter
    public class TeacherDashboardFilter
    {
        public int? academicYearId { get; set; }
    }
    #endregion
}