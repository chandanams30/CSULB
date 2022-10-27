using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkCommunitySiteUsers
    {
        public int CommunitySiteUserID { get; set; }
        public string CommunitySiteUser { get; set; }
    }
    public class FieldWorkCommunitySiteUsersResponse : BaseResponse
    {
        public List<FieldWorkCommunitySiteUsers> users { get; set; }
    }
    public class FieldWorkCommunityDistrictResponse : BaseResponse
    {
        public List<FieldWorkCommunityDistrict> districts { get; set; }
    }

    public class FieldWorkCommunityDistrict
    {
        public int CommunityDistrictID { get; set; }
        public string CommunityDistrictName { get; set; }
    }

    public class GetFieldworkCommunitySchoolSiteUsersByDistrictResponse : BaseResponse
    {
        public List<GetFieldworkCommunitySchool> schools { get; set; }
        public List<GetFieldworkCommunitySiteUsers> users { get; set; }
    }
    public class GetFieldworkCommunitySchool
    {
        public int CommunitySchoolID { get; set; }
        public string CommunitySchoolName { get; set; }
    }
    public class GetFieldworkCommunitySiteUsers
    {
        public int CommunitySiteUserID { get; set; }
        public string CommunitySiteUser { get; set; }
    }

    public class GetFieldWorkCoursesCategorySchoolTypes
    {
        public int SchoolTypeID { get; set; }
        public string SchoolType { get; set; }
    }
    public class GetFieldWorkCoursesCategorySchoolTypesResponse : BaseResponse
    {
        public List<GetFieldWorkCoursesCategorySchoolTypes> schoolTypes { get; set; }
    }
}
