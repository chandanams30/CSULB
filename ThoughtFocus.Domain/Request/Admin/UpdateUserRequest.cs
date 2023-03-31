using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UpdateUserRequest
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AuthenticationTypeId { get; set; }
        public bool Status { get; set; }
        public string CSULBID { get; set; }
        public string FirstNamePref { get; set; }
        public string LastNamePref { get; set; }
        public string DisplayName { get; set; }
        public int createdByUserID { get; set; }
        public List<UserRoles> Roles { get; set; }
    }
    public class UserRoles
    {
        public int? RoleID { get; set; }
        public string RoleName { get; set; }
        public int? ProgramID { get; set; }
        public string ProgramName { get; set; }
        public int? CommunityDistrictID { get; set; }
        public string CommunityDistrictName { get; set; }
    }
}
