using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

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
    public class ProgramChecklistInput
    {
        public object programChecklistCourseJSON { get; set; }
    }
    public class TeachingEvaluationRequest
    {
        public int evaluationID { get; set; }
        public int FieldWorkID { get; set; }
        public int FormID { get; set; }
        public string TermCode { get; set; }
        public int ProgramID { get; set; }
        public int UserID { get; set; }
        public string CooperatingTeacherName { get; set; }
        public string CooperatingTeacherEmail { get; set; }
        public string EvaluationType { get; set; }
        public string ApplicationType { get; set; }

    }
    public class UpdateTeachingEvaluationJSONRequest
    {
        public int EvaluationID { get; set; }
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string EvaluationJSON { get; set; }
        public string ApplicationType { get; set; }
        public string EvaluationType { get; set; }
        public string UniversityMentorJSON { get; set; }

    }
    public class UpsertTeachingEvaluationRequest
    {
        public int EvaluationID { get; set; }
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorEmail { get; set; }
        public string ApplicationType { get; set; }

    }
    public class TeachingEvaluationByIdentifierResponse : BaseResponse
    {
        public TeachingEvaluationByIdentifier evaluationByEvaluationIdentifier { get; set; }
        public ProgramDetails programDetails { get; set; }

    }
    public class ProgramDetails
    {
        public int ProgramID { get; set; }

    }
    public class TeachingEvaluationByIdentifier
    {
        public int EvaluationID { get; set; }
        public int FieldWorkID { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluationJSON { get; set; }
        public string StudentName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string CSULBID { get; set; }
        public string StudentEmail { get; set; }
        public string CourseTitle { get; set; }
        public string TermName { get; set; }
        public string ApplicationType { get; set; }
        public string CourseNameandNumber { get; set; }
        public int ProgramID { get; set; }
        public bool isYellowFlagEnabled { get; set; }
        public string UniversityMentorJSON { get; set; }

    }

    public class StudentTeachingObservationResponse : BaseResponse
    {
        public StudentTeachingObservation studentTeachingObservation { get; set; }
    }
    public class StudentTeachingObservation
    {
        public int ID { get; set; }
        public int FieldWorkID { get; set; }
        public int FormID { get; set; }
        public string Observations { get; set; }
        public string CSULBID { get; set; }

    }

    public class StudentTeachingObservationRequest
    {
        public int FieldWorkID { get; set; }
        public int FormID { get; set; }
        public string Observations { get; set; }
        public string CSULBID { get; set; }

    }


}
