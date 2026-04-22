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
        public string EducationalLeaveTerm { get; set; }
        public string Credential { get; set; }
        public string Certificate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string SSNNumber { get; set; }
        public string AcademicIntegrityStatement { get; set; }
        public DateTime SubmittedDate { get; set; }
        public bool IsAgreed { get; set; }
        public DateTime ? AgreedDate { get; set; }
        public string BachelorDegreeMajorSP { get; set; }
        public string ConsolidatedAddress { get; set; }
        public bool isStaff { get; set; }

    }
    public class StudentProfileSearchResponse : BaseResponse
    {
        public List<StudentProfileSearch> studentProfileSearch { get; set; }
    }
    public class StudentProfileSearch
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMAIL { get; set; }
        public string CSULBID { get; set; }
    }
    public class StudentProfileMessageBoardResponse :BaseResponse
    {
        public StudentProfileMessageBoard studentProfileMessageBoards { get; set; }
    }
    public class SemesterTermListResponse : BaseResponse
    {
        public List<SemesterTerm> SemesterTerms { get; set; }
    }
    public class ApplicationProgramListResponse: BaseResponse
    {
        public List<ApplicationProgramList> ApplicationProgramList { get; set; }
    }
    public class StudentAppliedFormsByProgramsResponse : BaseResponse
    {
        public List<StudentAppliedFormsByPrograms> StudentAppliedFormsByPrograms { get; set; }
        public int SSNSessionTimeOut {  get; set; }
    }

    public class StudentAppliedFormsByPrograms
    {
        public int FormID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
        public string ApplicationTypeName { get; set; }
        public int UserID { get; set; }
        public string TermCode { get; set; }
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public int ApplicantTypeID { get; set; }
        public string Semester { get; set; }
        public string Status { get; set; }
        public DateTime DOB { get; set; }
        public string SSN { get; set; }
    }
    public class StudentProfileMessageBoard
    {
        public string CSULBID { get; set; }
        public string MessageBoard { get; set; }
    }
    public class ApplicationList
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
    }
    public class SemesterTerm
    {
        public string TermCode { get; set; }
        public string TermName { get; set; }
        //public int ApplicationId { get; set; }
    }
    public class ApplicationProgramList
    {
        public int programID { get; set; }
        public string programName { get; set; }
        public string semester { get; set; }
        public string TermCode { get; set; }
    }

}
