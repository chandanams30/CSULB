using System;
using System.Collections.Generic;
using System.Text;

namespace CSULB_COE.ViewModels
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class AuthenticateRequestRoles
    {
        public long ApplicationTypeID { get; set; }
        public long RoleID { get; set; }

        public string Email { get; set; }
        public string CSULBID { get; set; }
        public List<CSULB_COE.Models.RoleATID> roleIDList { get; set; }
    }
    public class UserInfoRequest
    {
        public string Email { get; set; }
        public string CSULBID { get; set; }
    }

}
