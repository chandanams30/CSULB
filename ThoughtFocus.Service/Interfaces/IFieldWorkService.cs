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
    }
}
