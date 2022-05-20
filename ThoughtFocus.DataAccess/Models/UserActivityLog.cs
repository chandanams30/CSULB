using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class UserActivityLog
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public DateTime LoginDateTime { get; set; }
        public string Ipaddress { get; set; }
    }
}
