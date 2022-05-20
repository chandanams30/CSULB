using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class FormUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FormId { get; set; }
        public int RoleId { get; set; }
    }
}
