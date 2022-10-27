using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Login
{
    public class LoginUserRegistrationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
    }
}
