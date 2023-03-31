using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class AssignUsersToProgramRequest
    {
        public int ProgramID { get; set; }
        public int RoleID { get; set; }
        public int createdByUserID { get; set; }
        public List<UserPrograms> UserPrograms { get; set; }
    }
    public class UserPrograms   
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CSULBID { get; set; }
        public string Email { get; set; }
    }
}
