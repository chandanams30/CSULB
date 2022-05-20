using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class Program
    {
        public Program()
        {
            Forms = new HashSet<Form>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long ApplicationTypesId { get; set; }
        public string FormTemplate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }

        public virtual ApplicationType ApplicationTypes { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}
