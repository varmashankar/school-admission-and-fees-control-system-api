using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class AdmissionsDraftController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage createDraft(AdmissionDraftCreateRequest req)
        {
            try
            {
                var studentsBal = new BALStudents();
                var admissionNo = studentsBal.generateAdmissionId();

                var draftBal = new BALAdmissionDrafts();

                // Option A: do not create a student record at draft creation.
                var draftRes = draftBal.createAdmissionDraft(null, admissionNo);
                if (draftRes.executionStatus != "TRUE")
                    return Return.returnHttp("201", draftRes.message);

                var token = BALAdmissionDrafts.GenerateUrlSafeToken();
                var tokenRes = draftBal.createResumeToken(draftRes.id ?? 0, token, DateTime.Now.AddDays(30));
                if (tokenRes.executionStatus != "TRUE")
                    return Return.returnHttp("201", tokenRes.message);

                return Return.returnHttp("200", new
                {
                    draft_id = draftRes.id,
                    student_id = (int?)null,
                    admission_no = admissionNo,
                    resume_token = token
                });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage getDraftByToken(string token)
        {
            try
            {
                var bal = new BALAdmissionDrafts();
                var draft = bal.getDraftByToken(token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                return Return.returnHttp("200", draft);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage sendResumeOtp(AdmissionDraftSendOtpRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.admission_no))
                    return Return.returnHttp("201", "Admission number is required.");

                var bal = new BALAdmissionDrafts();
                var draft = bal.getDraftByAdmissionNo(req.admission_no);
                if (draft == null)
                    return Return.returnHttp("201", "Draft not found.");

                if (!draft.student_id.HasValue || draft.student_id.Value <= 0)
                    return Return.returnHttp("201", "No destination available for OTP. Please complete parent contact details first.");

                // Get student to know destinations
                var studentsBal = new BALStudents();
                var student = studentsBal.getStudentDetails(new StudentFilter { id = draft.student_id });

                var otp = BALAdmissionDrafts.GenerateOtp(6);
                var validTill = DateTime.Now.AddMinutes(10);

                bool sentAny = false;

                if (student != null && !string.IsNullOrWhiteSpace(student.phone))
                {
                    var r1 = bal.createOtp(draft.id ?? 0, "SMS", student.phone, otp, validTill);
                    if (r1.executionStatus == "TRUE") sentAny = true;
                }

                if (student != null && !string.IsNullOrWhiteSpace(student.email))
                {
                    var r2 = bal.createOtp(draft.id ?? 0, "EMAIL", student.email, otp, validTill);
                    if (r2.executionStatus == "TRUE") sentAny = true;
                }

                if (!sentAny)
                    return Return.returnHttp("201", "No destination available for OTP. Please complete parent contact details first.");

                return Return.returnHttp("200", new
                {
                    message = "OTP generated.",
                    valid_till = validTill.ToString("o"),
                    destinations = new
                    {
                        mobile = student?.phone,
                        email = student?.email
                    }
                });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage verifyResumeOtp(AdmissionDraftVerifyOtpRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.admission_no) || string.IsNullOrWhiteSpace(req.channel) || string.IsNullOrWhiteSpace(req.destination) || string.IsNullOrWhiteSpace(req.otp))
                    return Return.returnHttp("201", "Invalid request.");

                var bal = new BALAdmissionDrafts();
                var draft = bal.getDraftByAdmissionNo(req.admission_no);
                if (draft == null)
                    return Return.returnHttp("201", "Draft not found.");

                var verifyRes = bal.verifyOtp(draft.id ?? 0, req.channel.ToUpperInvariant(), req.destination, req.otp);
                if (verifyRes.executionStatus != "TRUE")
                    return Return.returnHttp("201", verifyRes.message);

                // Issue resume token after OTP verification
                var token = BALAdmissionDrafts.GenerateUrlSafeToken();
                var tokenRes = bal.createResumeToken(draft.id ?? 0, token, DateTime.Now.AddDays(30));
                if (tokenRes.executionStatus != "TRUE")
                    return Return.returnHttp("201", tokenRes.message);

                return Return.returnHttp("200", new
                {
                    resume_token = token,
                    admission_no = draft.admission_no,
                    student_id = draft.student_id
                });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveStudentStep(SaveStudentStepRequest req)
        {
            try
            {
                if (req == null)
                    return Return.returnHttp("201", "Invalid request.");

                if (string.IsNullOrWhiteSpace(req.resume_token))
                    return Return.returnHttp("201", "Missing resume token.");

                if (req.student == null)
                    return Return.returnHttp("201", "Missing student payload.");

                var draftBal = new BALAdmissionDrafts();
                var draft = draftBal.getDraftByToken(req.resume_token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                var studentsBal = new BALStudents();

                // Ensure timestamp is set for SP conventions
                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                req.student.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                // Ensure admission number maps to the draft
                req.student.admissionNo = draft.admission_no;

                // Draft flow defaults: avoid NULL created_by_id and ensure active by default
                if (!req.student.roleId.HasValue || req.student.roleId.Value <= 0)
                    req.student.roleId = 4;
                if (!req.student.roleTypeId.HasValue || req.student.roleTypeId.Value <= 0)
                    req.student.roleTypeId = req.student.roleId;
                if (!req.student.userId.HasValue || req.student.userId.Value <= 0)
                    req.student.userId = 4;

                // Students.deleted/status are non-nullable in the API model
                req.student.deleted = false;
                req.student.status = true;

                // Option A: create the student record on first student step
                if (!draft.student_id.HasValue || draft.student_id.Value <= 0)
                {
                    // studentCode is required by DB unique index; generate if missing
                    if (string.IsNullOrWhiteSpace(req.student.studentCode))
                        req.student.studentCode = studentsBal.generateStudentId();

                    req.student.id = null;
                    var createRes = studentsBal.saveStudent(req.student);
                    if (createRes.executionStatus != "TRUE")
                        return Return.returnHttp("201", createRes.message);

                    var attachRes = draftBal.attachStudentToDraft(draft.id ?? 0, createRes.id ?? 0);
                    if (attachRes.executionStatus != "TRUE")
                        return Return.returnHttp("201", attachRes.message);

                    return Return.returnHttp("200", new { message = "Student step saved.", student_id = createRes.id, admission_no = draft.admission_no });
                }

                // Existing draft already attached to a student – update student
                req.student.id = draft.student_id;
                req.student.status = true;
                req.student.deleted = false;

                var res = studentsBal.saveStudent(req.student);
                if (res.executionStatus != "TRUE")
                    return Return.returnHttp("201", res.message);

                return Return.returnHttp("200", new { message = "Student step saved.", student_id = draft.student_id, admission_no = draft.admission_no });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveParentStep(SaveParentStepRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.resume_token) || req.parent == null)
                    return Return.returnHttp("201", "Invalid request.");

                var draftBal = new BALAdmissionDrafts();
                var draft = draftBal.getDraftByToken(req.resume_token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                if (!draft.student_id.HasValue || draft.student_id.Value <= 0)
                    return Return.returnHttp("201", "Please complete Student Details step first.");

                TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                var ts = datetime.ToString("MM/dd/yyyy HH:mm:ss");

                // Save parent
                var parentsBal = new BALStudentParents();
                req.parent.studentId = draft.student_id;
                req.parent.creationTimestamp = ts;

                var pres = parentsBal.saveStudentParents(req.parent);
                if (pres.executionStatus != "TRUE")
                    return Return.returnHttp("201", pres.message);

                // Update student phone/email/address from parent step if provided
                var studentsBal = new BALStudents();
                var studentDetails = studentsBal.getStudentDetails(new StudentFilter { id = draft.student_id });
                if (studentDetails == null)
                    return Return.returnHttp("201", "Student not found. Please complete Student Details step again.");

                var studentUpdate = new Students
                {
                    id = draft.student_id,
                    studentCode = studentDetails.studentCode,
                    firstName = studentDetails?.firstName,
                    middleName = studentDetails?.middleName,
                    lastName = studentDetails?.lastName,
                    dob = studentDetails?.dob,
                    gender = studentDetails?.gender,
                    email = studentDetails?.email,
                    phone = studentDetails?.phone,
                    address = studentDetails?.address,
                    medicalInfo = studentDetails?.medicalInfo,
                    admissionNo = draft.admission_no,
                    classId = studentDetails?.classId,
                    roleId = 0,
                    creationTimestamp = ts
                };

                // Prefer parent provided contact/address if present
                if (!string.IsNullOrWhiteSpace(req.parent.mobile)) studentUpdate.phone = req.parent.mobile;
                if (!string.IsNullOrWhiteSpace(req.address)) studentUpdate.address = req.address;

                var sres = studentsBal.saveStudent(studentUpdate);
                if (sres.executionStatus != "TRUE")
                    return Return.returnHttp("201", sres.message);

                return Return.returnHttp("200", new { message = "Parent step saved.", student_id = draft.student_id, admission_no = draft.admission_no });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage saveAcademicStep(SaveAcademicStepRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.resume_token))
                    return Return.returnHttp("201", "Invalid request.");

                var draftBal = new BALAdmissionDrafts();
                var draft = draftBal.getDraftByToken(req.resume_token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                if (!draft.student_id.HasValue || draft.student_id.Value <= 0)
                    return Return.returnHttp("201", "Please complete Student Details step first.");

                // If transfer, save previous school
                if (string.Equals(req.student_type, "transfer", StringComparison.OrdinalIgnoreCase))
                {
                    if (req.previous_school == null)
                        return Return.returnHttp("201", "Previous school details required.");

                    TimeZoneInfo INDIA_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA_ZONE);
                    req.previous_school.creationTimestamp = datetime.ToString("MM/dd/yyyy HH:mm:ss");
                    req.previous_school.studentId = draft.student_id;

                    var prevBal = new BALPreviousSchool();
                    var res = prevBal.saveStudentPreviousSchool(req.previous_school);
                    if (res.executionStatus != "TRUE")
                        return Return.returnHttp("201", res.message);
                }

                return Return.returnHttp("200", new { message = "Academic step saved.", student_id = draft.student_id, admission_no = draft.admission_no });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage uploadDocument(UploadDraftDocumentRequest req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.resume_token) || req.document == null)
                    return Return.returnHttp("201", "Invalid request.");

                var draftBal = new BALAdmissionDrafts();
                var draft = draftBal.getDraftByToken(req.resume_token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                if (!draft.student_id.HasValue || draft.student_id.Value <= 0)
                    return Return.returnHttp("201", "Please complete Student Details step first.");

                // Ensure student linkage
                req.document.studentId = draft.student_id;

                // Delegate to existing StudentDocumentsController logic by using BAL directly.
                TimeZoneInfo INDIA = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                req.document.creationTimestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIA).ToString("MM/dd/yyyy HH:mm:ss");

                // If filePath contains base64, decode and save to disk same as StudentDocumentsController
                if (!string.IsNullOrEmpty(req.document.filePath) && req.document.filePath.StartsWith("data:"))
                {
                    try
                    {
                        var idx = req.document.filePath.IndexOf("base64,");
                        string base64 = req.document.filePath;
                        if (idx >= 0) base64 = req.document.filePath.Substring(idx + 7);

                        byte[] bytes = Convert.FromBase64String(base64);
                        string folder = System.Web.Hosting.HostingEnvironment.MapPath("~/uploads/documents/" + req.document.studentId + "/");
                        if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

                        string ext = ".bin";
                        int a = req.document.filePath.IndexOf(":");
                        int b = req.document.filePath.IndexOf(";");
                        if (a >= 0 && b > a)
                        {
                            var mime = req.document.filePath.Substring(a + 1, b - a - 1);
                            if (mime.Contains("/"))
                            {
                                var parts = mime.Split('/');
                                ext = "." + parts[1];
                            }
                        }

                        string fileName = Guid.NewGuid().ToString() + ext;
                        string fullPath = System.IO.Path.Combine(folder, fileName);
                        System.IO.File.WriteAllBytes(fullPath, bytes);
                        req.document.filePath = "/uploads/documents/" + req.document.studentId + "/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        return Return.returnHttp("201", "File decode/save failed: " + ex.Message);
                    }
                }

                var balDoc = new BALStudentDocuments();
                var resp = balDoc.saveStudentDocument(req.document);
                if (resp.executionStatus != "TRUE")
                    return Return.returnHttp("201", resp.message);

                return Return.returnHttp("200", new { message = resp.message, file_path = req.document.filePath });
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage getDraftPayload(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return Return.returnHttp("201", "Token is required.");

                var draftBal = new BALAdmissionDrafts();
                var draft = draftBal.getDraftByToken(token);
                if (draft == null)
                    return Return.returnHttp("201", "Invalid or expired token.");

                GetStudents student = null;
                System.Collections.Generic.List<GetStudentParents> parents = null;
                System.Collections.Generic.List<GetStudentPreviousSchoolDetails> prev = null;
                System.Collections.Generic.List<StudentDocuments> docs = null;

                if (draft.student_id.HasValue && draft.student_id.Value > 0)
                {
                    var studentsBal = new BALStudents();
                    student = studentsBal.getStudentDetails(new StudentFilter { id = draft.student_id });

                    var parentsBal = new BALStudentParents();
                    parents = parentsBal.getStudentParents(new StudentParentFilter { studentId = draft.student_id });

                    var prevBal = new BALPreviousSchool();
                    prev = prevBal.getStudentPreviousSchool(new StudentPreviousSchoolFilter { studentId = draft.student_id });

                    try
                    {
                        var docsBal = new BALStudentDocuments();
                        docs = docsBal.getStudentDocuments(new StudentDocuments { studentId = draft.student_id });
                    }
                    catch
                    {
                        docs = null;
                    }
                }

                var payload = new AdmissionDraftPayload
                {
                    draft = draft,
                    student = student,
                    parents = parents,
                    previous_schools = prev,
                    documents = docs ?? new System.Collections.Generic.List<StudentDocuments>()
                };

                return Return.returnHttp("200", payload);
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. " + ex.Message);
            }
        }
    }
}
