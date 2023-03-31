using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class AssignUserToRoleRequest
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int createdByUserID { get; set; }
    }
}
