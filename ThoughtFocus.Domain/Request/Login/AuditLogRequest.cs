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
    public class DistributionList
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SamAccountName { get; set; } = string.Empty;
    }
}
