using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ApplicationType
    {
        public ApplicationType()
        {
            Programs = new HashSet<Program>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }

        public virtual ICollection<Program> Programs { get; set; }
    }
}
