using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Request.FieldWork;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IGraduateProgramService
    {
        ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode);
        AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID);
        AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termcode, int formStateID);
        FormStatesResponse GetFormStates(int userID);
        SemesterListResponse GetSemesterList();
        GraduateProgramFormResponse GetForm(int userID, int formID, int programID, string termCode, bool showMilestone);
        BaseResponse UpdatePersonalInfoSchema(FormPersonalInfoSchemaRequest input);
        BaseResponse UpdateMessageBoardSchema(FormMessageBoardSchema input);
        BaseResponse UpsertFormAttachment(FormUpsertAttachmentRequest input);
        BaseResponse DeleteFormAttachment(DeleteFormAttachmentRequest input);
        BaseResponse DeleteInsructorAttachment(DeleteInsructorAttachmentRequest input);
        BaseResponse SaveForm(FormSaveRequest input);
        BaseResponse UpdateFormState(FormStatusUpdateRequest input);
        FormAttachments DownloadFormAttachments(int userID, int formattachmentID);
        InstructorAttachments GetInstructionAttachment(int UserID, int InstructionAttachmentID);
        InterviewerAttachments GetInterviewAttachments(int UserID, int InterviewAttachmentID);
        RecommendationAttachments GetFormRecommendations(int userID, int recommendationAttachmentID);
        BaseResponse AddRecommender(FormAddRecommenderRequest input);
        BaseResponse AddRecommendation(FormAddRecommendationRequest input);
        BaseResponse AddRecommendationForm(FormAddRecommendation input);
        AuthorizeRecommenderResponse GetDetailsForRecommendation(string recommenderIdentifier);
        BaseResponse SendReminderToRecommender(int recommendationID);
        BaseResponse UpdateReviwerReview(FormReviewerReviewRequest input);
        BaseResponse AssignFormToReviewers(int programID);
        byte[] GetMergedDocument(int formID);
        BaseResponse UpdateInstructorFeedback(UpdateInstructorFeedbackRequest input);
        BaseResponse UpdateInterviewerFeedback(UpdateInterviewerFeedbackRequest input);
        BaseResponse AddInstructorToForm(AddInstructorRequest input);
        BaseResponse AddInterviewerToForm(AddInterviewerRequest input);
        BaseResponse AddReviewerToForm(AddReviewerRequest input);
        BaseResponse RemoveReviewerFromForm(AddReviewerRequest input);
        InstructorListResponse GetInstructorList(GetInstructorInterviewerListRequest input);
        InterviewerListResponse GetInterviewerList(GetInstructorInterviewerListRequest input);
        ReviewerListResponse GetReviewerList(GetReviewerListRequest input);
        StudentMessageBoardResponse GetFormStudentMessageBoard(int UserID, int FormID, int ProgramID, string TermCode);
        BaseResponse UpdateFormStudentMessageBoard(StudentMessageBoardRequest input);
        ProgramConfigurationHandlerResponse GetProgramConfigurationHandler(int UserID, int FormID, int ProgramID, string TermCode);
        BaseResponse SaveFormGridNotes(FormSaveGridNotesRequest input);
        LatestWaitlistNumberResponse GetLatestWaitlistNumber(string TermCode, int ProgramID, int FormID);
        BaseResponse BulkOfferNotOfferUpdateFormState(BulkNotOfferFormStatusUpdateRequest input);
        BaseResponse UpdateRecommendation(UpdateRecommendation input);
        BaseResponse DeleteRecommendation(DeleteRecommendations input);
        AdhocMailLogResponse SendNotificationforPendingRecommendations(PendingRecommendationsRequest input);
        BaseResponse UpdateProgramApplicationDates(UpdateProgramApplicationDatesRequest input);
        ProgramApplicationDates GetProgramApplicationDates(int programID, string termCode);
        ApplicationProgramResponse GetApplicationProgramsforDates(int userID, int applicationTypeID, string termCode);
        BaseResponse RevertBacktoPreviousState(int formID);
        BaseResponse MoveApplicationToSemester(MoveApplicationToSemesterRequest input);
        ApplicationProgramsListResponse GetApplicationProgramList(int applicationTypeID);
        RecommenderMailBodyResponse GetRecommenderMailBody(int applicationId, int programID);
        BaseResponse UpdateRecommenderMailBody(RecommenderBody input);
        BaseResponse UpsertDecisionLetters(DecisionLettersRequest input);
        DecisionLettersResponse GetDecisionLetters(string programIdentifier, string offeredCategories, string decisionType);
        DropDownListResponse GetDropDownList(int programId, string controlLabel);
        BaseResponse UpsertDropDown(DropDownRequest input);
        ControlLabelListResponse GetControlLabelList(int programId);
    }
}
