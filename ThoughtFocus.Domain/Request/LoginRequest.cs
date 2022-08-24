using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CSULBID { get; set; }
    }
    public class LoginSSORequest
    {
        public string Token { get; set; }

    }

}
