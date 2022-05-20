using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public long LastModifiedByUserId { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
