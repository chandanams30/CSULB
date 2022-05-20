using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class Semester
    {
        public Semester()
        {
            Forms = new HashSet<Form>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public virtual ICollection<Form> Forms { get; set; }
    }
}
