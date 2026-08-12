using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentProfile
    {
        StudentProfileResponse GetStudentProfileData(string CsuldId, int UserID, int formID);
        StudentProfileSearchResponse GetStudentProfileSearchData(string searchString);
        StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId, string MessageBoardIdentifier);
        BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input);
        BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input);
        List<ApplicationList> GetApplications(int userId, string identifier, int roleID);
        SemesterTermListResponse GetSemesterList(int applicationId);
        ApplicationProgramListResponse GetApplicationProgramList(int userID, int applicationTypeID, string termCode,int roleID);
        StudentAppliedFormsByProgramsResponse GetStudentAppliedFormsByPrograms(int programID, string termCode, string CSULBID,string identifier);
        BaseResponse SaveStudentAggrement(SaveStudentAggrementRequest input);
        BaseResponse DeleteProfileAttachment(DeleteProfileAttachmentRequest input);
        BaseResponse SaveStudentProfilePersonalInfoData(SaveStudentProfilePersonalInfoDataRequest input);
        ProgramPlannerCourseListResponse GetProgramPlannerCourseList(string CSULBID, int ProgramID, string TermCode);
        BaseResponse UpsertCourseDetails(string JSONString);
        ProgramCheckListCourseResponse GetProgramChecklistCoursesTerm(string CSULBID, int ProgramID, string TermCode, int FormID);
        BaseResponse UpsertProgramChecklistData(string JSONString);
        BaseResponse UpsertTeachingEvaluation(TeachingEvaluationRequest input);
        TeachingEvaluationByIDResponse GetTeachingEvaluationByFieldWorkID(int UserID, int FieldWorkID, int ProgramID, string TermCode, int FormID);
        UpsertProfileAttachmentResponse UpsertProfileAttachment(UpsertProfileDocumentRequest input);

        DownloadProfileAttachmentResponse DownloadProfileAttachment(Guid UniqueID);

        ProfileAttachmentDetailsResponse GetProfileAttachmentDetails(string CSULBID);
        BaseResponse UpdateTeachingEvaluationJSON(UpdateTeachingEvaluationJSONRequest input);

        TeachingEvaluationByIdentifierResponse GetTeachingEvaluationByIdentifier(string evaluationIdentifier);
        StudentTeachingObservationResponse GetStudentTeachingObservations(int fieldworkID, int formID, string CSULBID);
        BaseResponse SaveStudentTeachingObservations(StudentTeachingObservationRequest input);
        DownloadStudentTeachingObservationAttachmentsResponse DownloadStudentTeachingObservationAttachments(int formID, int fieldworkID, string csulbid, string observationDocument, Guid UniqueID);
        BaseResponse DeleteStudentTeachingObservationAttachments(DeleteStudentTeachingObservationRequest input);


    }
}
