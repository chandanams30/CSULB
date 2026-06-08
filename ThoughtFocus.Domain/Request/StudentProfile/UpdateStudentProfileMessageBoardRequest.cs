using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.StudentProfile
{
    public class UpdateStudentProfileMessageBoardRequest
    {
        public string CSULBID { get; set; }
        public string MessageBoardIdentifier { get; set; }
        public string MessageBoard { get; set; }

    }
    public class SaveStudentProfileDataRequest
    {
        public string CSULBID { get; set; }
        public DateTime ? DateOfBirth { get; set; }
        public string SSNNumber { get; set; }
        public string AcademicIntegrityStatement { get; set; }
        public DateTime ? SubmittedDate { get; set; }

    }
    public class SaveStudentAggrementRequest
    {
        public string CSULBID { get; set; }
        public bool IsAgreed { get; set; }
        public DateTime AgreedDate { get; set; }
    }
    public class UpsertProfileDocumentRequest
    {
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string CSULBID { get; set; }
        public int CreatedBy { get; set; }
        //public string ProfileAttachmentComments { get; set; }
    }
    public class SaveStudentProfilePersonalInfoDataRequest
    {
        public string CSULBID { get; set; }
        public string BachelorDegreeMajorSP { get; set; }
        public string CredentialProgram { get; set; }
        public string CredentialPathway { get; set; }
        public string Certificate { get; set; }

    }
    public class SaveStudentProfilePersonalInfoDoc
    {
        public int CSULBID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
    }
    public class DeleteProfileAttachmentRequest
    {
        public Guid UniqueID { get; set; }
    }
    public class ProgramPlannerInput
    {
        public object ProgramPlannerCourseJSON { get; set; }
    }

}
