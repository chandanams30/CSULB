using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.StudentProfile
{
    public class UpsertProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
    }
    public class DownloadProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class ProfileAttachmentDetailsResponse : BaseResponse
    {
        public List<ProfileAttachmentDetails> ProfileAttachmentDetails { get; set; }
    }
    public class ProfileAttachmentDetails
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CanView { get; set; }
        public string CreatedByUsername { get; set; }
    }
    public class PrerequisitesResponse : BaseResponse
    {
        public List<PrerequisiteDetails> FormPrerequisites { get; set; }
    }
    public class PrerequisiteDetails
    {
        public int fieldWorkAttachmentID { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public string DocumentStatus { get; set; }
    }
    public class ProgramPlannerCourseListResponse:BaseResponse
    {
        public List<ProgramPlannerCourseList> ProgramPlannerCourseList { get; set; }
        public ProgramPlannerStateHandler StateHandler { get; set; }

    }

    public class ProgramPlannerCourseList
    {

        public int MasterID { get; set; }
        public int DetailID { get; set; }
        public string CourseName { get; set; }
        public string TermCode { get; set; }
        public string Term { get; set; }
        public string Year { get; set; }
        public string Notes { get; set; }
        public string CSULBID { get; set; }
    }
    public class ProgramPlannerStateHandler
    {
        public int StateID { get; set; }
    }
    public class ProgramCheckListCourseResponse : BaseResponse
    {
        public List<ProgramCheckListCourse> ProgramCheckListCourse { get; set; }
        public List<ProgramEvaluationDetails> programEvaluationDetails { get; set; }
        public BILARequirementDetails BILARequirementDetails { get; set; }
        public CPRRequirementDetails CPR { get; set; }
        public CandidateStatusDetails candidateStatusDetails { get; set; }
        public List<TPADetails> TPADetails { get; set; }

    }

    public class ProgramCheckListCourse
    {
        public string CourseName { get; set; }
        public string TermCode { get; set; }
        public string Grade { get; set; }
        public int SPCMID { get; set; }
        public string CourseNotes { get; set; }
        public string fieldWorkEvaluation { get; set; }

    }
    public class BILARequirementDetails
    {
        public string CSETSubset { get; set; }
        public string CultureRequirement { get; set; }
        public string MethodologyRequirement { get; set; }
        public DateTime DatePassed { get; set; }

    }
    public class CPRRequirementDetails
    {
        public string Notes { get; set; }
        public string MetBy { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
    public class CandidateStatusDetails
    {
        public string ProgramStatus { get; set; }
        public string StudentTeachingStatus { get; set; }
        public string CertificationStatus { get; set; }
        public string ConstitutionRequirementOption { get; set; }
        public string ConstitutionRequirementCourseExam { get; set; }
        public DateTime? TPAFinalPassDate { get; set; }

    }
    public class TPADetails
    {
        public string CycleID { get; set; }
        public string TPARecordID { get; set; }
        public string TPAType { get; set; }
        public string Score { get; set; }
        public string TaskName { get; set; }
        public DateTime? ReleaseDate { get; set; }

    }
    public class ProgramEvaluationDetails
    {
        public string LetterOfRecommendationJSON { get; set; }
        public string FileLink { get; set; }
        public string ApplicationType { get; set; }
    }

}
