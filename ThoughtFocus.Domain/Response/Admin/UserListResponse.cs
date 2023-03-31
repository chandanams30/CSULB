using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Admin
{
    public class UserListResponse:BaseResponse
    {
       public List<UserList> userList { get; set; }
    }
    public class UserList
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMAIL { get; set; }
        public string CSULBID { get; set; }
        public string UserRoles { get; set; }
    }
    public class UserDataOptionListResponse : BaseResponse
    {
        public UserDataOptions UserDataOptionList { get; set; }
    }
    public class UserDataOptions
    {
        public string UserDataOptionList { get; set; }
    }

    public class UserDetailResponse:BaseResponse
    {
        public UserDetails UserDetail { get; set; }
    }
    public class UserDetails
    {
        public string UserDetail { get; set; }
    }

    public class DownloadMessageBoardAttachmentResponse : BaseResponse
    {
        public DownloadMessageBoardAttachment DownloadAttachment { get; set; }
    }

    public class DownloadMessageBoardAttachment
    {
        public byte[] FileContent { get; set; }
    }
    public class GetUsersByProgramRoleResponse:BaseResponse
    {
        public GetUsersByProgramRole Details { get; set; }
    }
    public class GetUsersByProgramRole
    {
        public string UsersByProgramRole { get; set; }
    }
    public class CommunityDistrictListResponse:BaseResponse
    {
        public CommunityDistrictList Districts { get; set; }
    }

    public class CommunityDistrictList
    {
        public string Districts { get; set; }
    }

    public class CommunitySchoolListResponse : BaseResponse
    {
        public CommunitySchoolList Schools { get; set; }
    }

    public class CommunitySchoolList
    {
        public string Schools { get; set; }
    }

}
