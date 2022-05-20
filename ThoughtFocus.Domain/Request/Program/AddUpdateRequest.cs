using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Program
{
    public class AddUpdateRequest
    {
        public int ProgramID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApplicationTypeId { get; set; }
        public string Form { get; set; }
        public string Notes { get; set; }
        public int userId { get; set; }
    }
}
