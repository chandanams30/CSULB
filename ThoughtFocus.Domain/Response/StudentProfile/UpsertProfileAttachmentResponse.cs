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
    public class ProgramEvaluationDetails
    {
        public string LetterOfRecommendationJSON { get; set; }
        public string FileLink { get; set; }
        public string ApplicationType { get; set; }
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
    public class TeachingEvaluationByIDResponse : BaseResponse
    {
        public List<TeachingEvaluationByID> teachingEvaluationByID { get; set; }
        public MidTermUniversityMentorEvaluationDetails MUMED {  get; set; }
        public FinalTermUniversityMentorEvaluationDetails FUMED { get; set; }
    }
    public class TeachingEvaluationByID
    {
        public int? EvaluationID { get; set; }
        public int? FieldWorkId { get; set; }
        public string CooperatingTeacherName { get; set; }
        public string CooperatingTeacherEmail { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CooperatingTeacherURL { get; set; }
        public string CooperatingTeacherIdentifier { get; set; }
        public bool isFinalMailSent { get; set; }
        public bool isMidtermMailSent { get; set; }
        public string CooperatingTeacherJSON { get; set; }
        public bool CanView { get; set; }
        public string FileLink { get; set; }
        public string ApplicationType { get; set; }
        public string EvaluationType { get; set; }
        public int ProgramID { get; set; }
        public string UniversityMentorName {  get; set; }
        public string UniversityMentorEmail { get; set; }
        public string UniversityMentorJSON { get; set; }

    }
    public class MidTermUniversityMentorEvaluationDetails
    {
        public string UniversityMentorName { get; set; }
        public string UniversityMentorEmail { get; set; }
        public string UniversityMentorJSON { get; set; }
        public string EvaluationType { get; set; }
    }
    public class FinalTermUniversityMentorEvaluationDetails
    {
        public string UniversityMentorName { get; set; }
        public string UniversityMentorEmail { get; set; }
        public string UniversityMentorJSON { get; set; }
        public string EvaluationType { get; set; }
    }
    public class DownloadStudentTeachingObservationAttachments : BaseResponse
    {
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class StudentTeachingObservationAttachmentsResponse : BaseResponse
    {
        public List<StudentTeachingObservationAttachments> studentTeachingObservationAttachments { get; set; }
    }
    public class DownloadStudentTeachingObservationAttachmentsResponse : BaseResponse
    {
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class StudentTeachingObservationAttachments
    {
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CanView { get; set; }
        public string CreatedByUsername { get; set; }
        public byte[] FileContent { get; set; }

    }

    public class ObservationRoot
    {
        public List<ObservationItem> Observations { get; set; }
    }

    public class ObservationItem
    {
        public string ObservationDate1 { get; set; }
        public string ObservationNotes1 { get; set; }
        public ObservationDocument ObservationDocumentName1 { get; set; }

        public string ObservationDate2 { get; set; }
        public string ObservationNotes2 { get; set; }
        public ObservationDocument ObservationDocumentName2 { get; set; }

        public string ObservationDate3 { get; set; }
        public string ObservationNotes3 { get; set; }
        public ObservationDocument ObservationDocumentName3 { get; set; }

        public string ObservationDate4 { get; set; }
        public string ObservationNotes4 { get; set; }
        public ObservationDocument ObservationDocumentName4 { get; set; }

        public string ObservationDate5 { get; set; }
        public string ObservationNotes5 { get; set; }
        public ObservationDocument ObservationDocumentName5 { get; set; }

        public string ObservationDate6 { get; set; }
        public string ObservationNotes6 { get; set; }
        public ObservationDocument ObservationDocumentName6 { get; set; }
    }

    public class ObservationDocument
    {
        public int CreatedBy { get; set; }
        public string CSULBID { get; set; }
        public string FileContent { get; set; }
        public string FileName { get; set; }
        public Guid UniqueID { get; set; }
        public string FileExt { get; set; }
        public string FileNameStatic { get; set; }
    }
    public class ExitSurveyAttachmentResponse : BaseResponse
    {
        public int SurveyAttachementID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
    }
    public class ExitSurveyAttachementDetailsResponse : BaseResponse
    {
        public List<ExitSurveyAttachementDetails> ExitSurveyAttachementDetails { get; set; }
    }
    public class ExitSurveyAttachementDetails
    {
        public int SurveyAttachementID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CanView { get; set; }
        public string CSUExitSurveyMet {  get; set; }
        public string CEDExitSurveyMet { get; set; }
    }
    public class DownloadExitSurveyAttachementResponse : BaseResponse
    {
        public int SurveyAttachementID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
