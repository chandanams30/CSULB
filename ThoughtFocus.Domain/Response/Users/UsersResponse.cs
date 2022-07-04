using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Users
{
    public class UsersResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
    }
}
