using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IFieldWorkService
    {
        FieldWorkListResponse GetFieldWorkList(int userId);
        FieldWorkDataResponse GetFieldWorkDetailsById(int userId,int fieldWorkId);
        BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input);
        BaseResponse UpdateFieldWorkDocumentValidation(FieldWorkUploadDocumentsRequest input);
        FieldWorkProfileAttachments DownloadRequiredDocuments(int userId, int fieldworkAttachmentId);
        FieldWorkActivityLogListResponse GetFieldWorkActivityLog(int userId, int fieldworkId);
        FieldWorkActivityLogByIDResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input);
        BaseResponse UpdateFieldWorkActivityLogStatus(UpdateFieldWorkActivityLogStatusRequest input);
        FieldWorkActivityLogByIDResponse GetFielWorkActivityLogByID(int userID,int activityLogID);
        FieldWorkActivityLogByIDResponse GetFieldWorkActivityLogforAdd(int userID, int fieldWorkID);
        FieldWorkAttachmentsResponse UploadFieldWorkActivityDocuments(FieldWorkAttachmentsRequest input);
        FieldWorkProfileAttachments DownloadActivityAttachments(int userId, int fieldworkAttachmentId, string savedFileName);
        FieldWorkCommunitySitesResponse GetCommunitySites();
        FieldWorkCommunitySiteUsersResponse GetCommunitySiteUsers(int communitySiteId);
        FieldWorkCommunityDistrictResponse GetFieldworkCommunityDistrict();
        GetFieldWorkCoursesCategorySchoolTypesResponse GetFieldWorkCoursesCategorySchoolTypes(int userID, int fieldWorkID);
        GetFieldworkCommunitySchoolSiteUsersByDistrictResponse GetFieldworkCommunitySchoolSiteUsersByDistrict(int CommunityDistrictID);
        FieldWorkStandardsResponse GetStandards(int userID, int fieldWorkID);
        FieldWorkFnCSchemaResponse GetFnCSchema(int userID, int fieldWorkID, int schemaTypeId, int fieldWorkActivityLogID);
        BaseResponse UpdateFnCSchema(FieldWorkFnCSchemaUpdateRequest input);
        BaseResponse UpdateFieldworkCommunityUsersforCreation();
        PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList();
        PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail(int CSSDTID);
        PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail);
        FieldWorkListResponse GetFieldWorkData(string CommunitySiteUserIdentifier);
        PUFieldWorkActivityLogListResponse GetFieldWorkActivityLogList(string CommunitySiteUserIdentifier,int fieldWorkId);
        FieldWorkActivityLogByIDResponse PUNS_GetFieldWorkActivityLogByID(string CommunitySiteUserIdentifier, int ActivityLogID);
        BaseResponse PUNS_UpdateFieldWorkActivityLogStatus(PUUpdateFieldWorkActivityLogStatusRequest input);
        FieldWorkEvaluationByIDResponse GetEvaluationByFieldWorkID(int UserID, int FieldWorkID, int ProgramID, string TermCode);
        BaseResponse UpsertEvaluation(UpsertEvaluationRequest input);
        EvaluationByEvaluationIdentifierResponse GetEvaluationByEvaluationIdentifier(string evaluationIdentifier);
        BaseResponse UpdateEvaluationJSON(UpdateEvaluationJSONRequest input);
        FieldWorkAttachmentsRequest  DownloadAttachment(DownloadAttachment input);
        PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents(int communitySiteUsersID, string communitySiteUserName, string communitySiteUserEmail,int activityLogID);
        FieldWorkAttachmentsRequest DownloadActivityLogs(GetReportDataRequest input);
    }
}
