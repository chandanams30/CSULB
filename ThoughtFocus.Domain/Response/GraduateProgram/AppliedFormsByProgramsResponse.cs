using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class AppliedFormsByProgramsResponse : BaseResponse
    {
        public List<AppliedFormsByPrograms> AppliedFormsByPrograms { get; set; }
        public HeaderDetails HeaderDetails { get; set; }

    }
    public class AppliedFormsByPrograms
    {
        public int FormID { get; set; }
        public string StudentName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string Email { get; set; }
        public string AlternateEmail { get; set; }
        public int FormStateID { get; set; }
        public string FormState { get; set; }
        public DateTime AppliedDate { get; set; }
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string Semester { get; set; }
        public string TermCode { get; set; }
        public string CSULBID { get; set; }
        public string ReviewersName { get; set; }
        public string BSRStatus { get; set; }
        public string CTCStatus { get; set; }
        public string GPAStatus { get; set; }
        public string SMCStatus { get; set; }
        public string TBTestStatus { get; set; }
        public string CredentialPathway { get; set; }
        public string IsInterviewRatingSheetSubmitted { get; set; }
        public string RecommendationsSubmittedCount { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string EDEL200380FinalFieldworkEvaluation_Status { get; set; }
        public string InstructorEvaluationForm_Status { get; set; }
        public string AdvisementConfirmationForm_Status { get; set; }
        public string GridNotes { get; set; }
        public string ReviewerRecommendation { get; set; }
        public string FinalDecision { get; set; }
        public int WaitlistNumber { get; set; }
        public bool ShowBulkCheckBox { get; set; }
        public string ResumeUploadStatus { get; set; }
        public string SOPUploadStatus {  get; set; }
        public string PersonalStatementDoc { get; set; }
        public string UniversityApplicationDetails { get; set; }
        public string DocumentNumber { get; set; }
    }


}
