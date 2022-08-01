using System;
using System.Collections.Generic;
using System.Text;

namespace CSULB_COE.Models
{
    public class AuthenticateResponse 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string JWTToken { get; set; }
        //public List<long> RoleID { get; set; }
        //public List<string> RoleName { get; set; }

        public List<Roles> Roles { get; set; }
        
        public string message { get; set; }
    }

    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
