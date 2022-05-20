using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class Document
    {
        public Document()
        {
            StudentDocuments = new HashSet<StudentDocument>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }

        public virtual ICollection<StudentDocument> StudentDocuments { get; set; }
    }
}
