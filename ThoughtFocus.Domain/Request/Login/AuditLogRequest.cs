using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Login
{
    public class AuditLogRequest
    {
        public int UserID { get; set; }
        public string Type { get; set; }
        public string IPAddress { get; set; }
        public string AuthenticationType { get; set; }
    }
}
