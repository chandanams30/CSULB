using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class Form
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ApplicationId { get; set; }
        public long SemesterId { get; set; }
        public string Form1 { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public long ModifiedBy { get; set; }
        public int? State { get; set; }

        public virtual Program Application { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual User User { get; set; }
    }
}
