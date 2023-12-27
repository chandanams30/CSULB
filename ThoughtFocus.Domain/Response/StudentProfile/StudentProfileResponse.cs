using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.StudentProfile
{
    public class StudentProfileResponse : BaseResponse
    {
        public StudentProfile studentProfile { get; set; }
    }

    public class StudentProfile
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
        public string AcademicPlan { get; set; }
        public string AcademicSubPlan { get; set; }
        public string AdditionalPlan { get; set; }
        public string AdmitTerm { get; set; }
        public string ProgramStatusDesc { get; set; }
        public string ActiveTerm { get; set; }
        public string BachelorDegreeMajor { get; set; }
        public string GraduationFillingStatusDesc { get; set; }
        public string GraduationFillingTerm { get; set; }
        public string AcademicStanding { get; set; }
        public string CurrentCsulbGpa { get; set; }
        public string CumulativeGpa { get; set; }
        public string MajorGpa { get; set; }
    }
}
