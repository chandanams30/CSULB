using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IInitialCredentialProgramService
    {
        ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode);
        AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termcode, int formStateID);
        AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID);
        FormStatesResponse GetFormStates(int userID);
        SemesterListResponse GetSemesterList();
        OptionItemListResponse GetIntialCreditialOptionItemsList(int programID);
        FormDispositionsAssessmentResponse GetFormDispositionsAssessment(FormDispositionsAssessmentRequest input);
        BaseResponse UpsertFormDispositionsAssessment(UpsertFormDispositionsAssessmentRequest input);
        FormSubSectionResponse GetFormSubSection(FormSubsectionRequest input);
        BaseResponse UpsertFormSubSection(UpsertFormSubSectionRequest input);
        FormSectionAttachmentResponse GetFormSubSectionAttachmentList(FormSubsectionRequest input);
        BaseResponse SaveFormSubSectionAttachment(SaveFormSubSectionAttachmentRequest input);
        FormSubsectionAttachmentDownloadResponse GetFormSubSectionAttachment(SubsectionAttachmentDownloadRequest input);
        FormPrerequisitesResponse GetFormPrerequisites(int UserId, int FormID);
        UpsertFormEducationInformationAttachmentResponse UpsertFormEducationInformationAttachment(UpsertFormEducationInformationAttachmentRequest input);
        DownloadEducationalInformationalAttachment GetFormEducationInformationAttachment(int FormID, Guid UniqueID);
        BaseResponse UpdateFormSubSectionApproveral(UpdateFormSubSectionApproveralRequest input);
        FormSectionApprovalDetailsResponse GetFormSubSectionApproveralDetails(int FormID, int UserID, int FormSubSectionID, string SubSectionIdentifiers);
        AdditionalOfficialDocumentsResponse GetAdditionalOfficialDocuments(int UserID, int FormID, int ProgramID, string TermCode);
        BaseResponse UpdateAdditionalOfficialDocument(UpdateAdditionalOfficialDocumentRequest input);
        FormAttachments GetAdditionalOfficialDocument(GetAdditionalOfficialDocumentRequest input);
        BaseResponse DeleteAdditionalOfficialDocument(DeleteAdditionalOfficialDocumentRequest input);
        BaseResponse UpdateFormSubSectionSubmitForReview(UpdateFormSubSectionSubmitForReviewRequest input);
        PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail);
        LetterOfRecommendationsByFormIDResponse GetLetterOfRecommendationsByFormID(int UserID, int FormID, int ProgramID, string TermCode);
        BaseResponse UpsertLetterOfRecommendations(UpsertLetterOfRecommendationsRequest input);
        LetterOfRecommendationsByRecommenderIdentifierResponse GetLetterOfRecommendationsByRecommenderIdentifier(string identifier);
        BaseResponse UpdateLetterOfRecommendationsJSON(UpdateLetterOfRecommendationsJSONRequest input);
        FormAttachments DownloadAttachment(string LetterOfRecommendationJSON);

    }
}
