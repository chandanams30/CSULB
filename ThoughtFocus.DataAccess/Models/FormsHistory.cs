using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class FormsHistory
    {
        public int Id { get; set; }
        public int FormsId { get; set; }
        public string Form { get; set; }
        public string VersionNumber { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public long ModifiedBy { get; set; }
    }
}
