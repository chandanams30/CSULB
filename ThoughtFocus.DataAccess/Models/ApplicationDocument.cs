using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ApplicationDocument
    {
        public long Id { get; set; }
        public long ProgramId { get; set; }
        public long DocumentId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }
    }
}
