using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class AddUserRequest
    {
        public string CSULBID { get; set; }
        public string displayName { get; set; }
        public string mail { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int RoleID { get; set; }
        public int AuthenticationTypeId { get; set; }
        public int createdByUserID { get; set; }
    }
}
