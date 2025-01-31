using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Admin;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IAdminService
    {
        RolesListResponse GetRolesList();
        UserListResponse GetUsersList(string searchString,int RoleID);
        UserDataOptionListResponse GetUserOptionList(int UserID);
        UserDetailResponse GetUser(int UserID);
        CommunityDistrictListResponse GetCommunityDistrictList();
        CommunitySchoolListResponse GetCommunitySchoolList();
        DownloadMessageBoardAttachment DownloadMessageBoardAttachment(string fileName);
        GetUsersByProgramRoleResponse GetUsersByProgramRole(int ProgramID, int RoleID);
        BaseResponse AssignUserToRole(AssignUserToRoleRequest input);
        BaseResponse AddUser(AddUserRequest input);
        BaseResponse UpdateUser(UpdateUserRequest input);
        BaseResponse AssignUsersToProgram(AssignUsersToProgramRequest input);
        BaseResponse UploadMessageBoardAttachment(UploadMessageBoardAttachmentRequest input);
        BaseResponse UpsertCommunityDistrict(UpsertCommunityDistrictRequest input);
        BaseResponse UpsertCommunitySchool(UpsertCommunitySchoolRequest input);
        UpcomingSemesterListResponse GetFutureSemesterList();
        ApplicationProgramListResponse GetAllApplicationProgramsList();
        BaseResponse UpdateProgramApplicationDates(UpdateProgramApplicationDates input);


    }
}
