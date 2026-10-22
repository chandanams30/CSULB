using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Domain.Response.StudentTeachingMilestone
{
    public class GetStudentTeachingMilestoneResponse
    {

    }

        public class GetLatestFormResponse : BaseResponse
        {
            public GetLatestFormDetails getLatestFormDetails { get; set; }
        }
        public class GetLatestFormDetails
        {
            public int FormId { get; set; }
            public int ProgramID { get; set; }
            public string TermCode { get; set; }
        }

    public class StudentTeachingMilestonesResponse : BaseResponse
    {
        public StudentTeachingMilestones studentTeachingMilestones { get; set; }
        public StudentTeachingAcknowledgment studentTeachingAcknowledgment { get; set; }
        public List<AttachmentsInformation> AttachmentsInformation { get; set; }
    }
    public class StudentTeachingAcknowledgment
    {
        public string AcknowledgmentJSON { get; set; }
    }
    public class StudentTeachingMilestones
    {
        public int ID { get; set; }
        public string CSULBID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PreferredName { get; set; }
        public string AlternateName { get; set; }
        public string MailingAddress1 { get; set; }
        public string MailingAddress2 { get; set; }
        public string MailingAddress3 { get; set; }
        public string MailingAddress4 { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingPostal { get; set; }
        public string Phone { get; set; }
        public string csulbemail { get; set; }
        public string AlternateEmail { get; set; }
        public string ConsolidatedAddress { get; set; }
        public int UserID { get; set; }
        public string CredentialPathway { get; set; }
        public string CredentialProgram { get; set; }
        public string TermCode { get; set; }
        public string ProgramStatusDesc { get; set; }
    }

    public class DistrictListResponse : BaseResponse
    {
        public List<Districts> Districts { get; set; }
    }
    
    public class Districts
    {
        public int ID { get; set; }
        public int ProgramID { get; set; }
        public string Name { get; set; }
    }
    public class CredentialAreaListResponse : BaseResponse
    {
        public List<CredentialArea> CredentialArea { get; set; }
    }

    public class CredentialArea
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class AttachmentResponse : BaseResponse
    {
        public int AttachementID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
    }
    public class AttachmentsInformation
    {
        public int ID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public Guid UniqueID { get; set; }
        public string FolderName { get; set; }

    }
    public class DownloadAttachementResponse : BaseResponse
    {
        public int ID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
