using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class User
    {
        public User()
        {
            Forms = new HashSet<Form>();
            StudentDocuments = new HashSet<StudentDocument>();
            UserRoles = new HashSet<UserRole>();
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public long CreatedByUserId { get; set; }
        public int? AuthenticationTypeId { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Form> Forms { get; set; }
        public virtual ICollection<StudentDocument> StudentDocuments { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
