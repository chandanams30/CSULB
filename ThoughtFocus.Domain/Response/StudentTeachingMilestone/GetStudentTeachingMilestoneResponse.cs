using System;
using System.Collections.Generic;
using System.Text;
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
}
