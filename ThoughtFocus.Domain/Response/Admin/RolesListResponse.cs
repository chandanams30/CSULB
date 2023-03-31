using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Admin
{
    public class RolesListResponse:BaseResponse
    {
      public List<RolesList> roles { get; set; }
    }
    public class RolesList
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
