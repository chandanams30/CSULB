using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSULB_COE.Models
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
