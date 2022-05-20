using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class StudentDocument
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long DocumentId { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }

        public virtual Document Document { get; set; }
        public virtual User User { get; set; }
    }
}
