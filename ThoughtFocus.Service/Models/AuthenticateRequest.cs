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
        public int ApplicationTypeID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<CSULB_COE.Models.RoleATID> roleIDList { get; set; }
    }
   
}
