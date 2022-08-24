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
    public class FieldWorkCommunitySiteUsersResponse:BaseResponse
    {
        public List<FieldWorkCommunitySiteUsers> users { get; set; }
    }
}
