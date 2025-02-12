using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UpdateProgramApplicationDates
    {
        public List<int> ProgramID { get; set; }
        public string TermCode { get; set; }
        public DateTime ApplicationOpens { get; set; }
        public DateTime ApplicationDeadline { get; set;}
        public DateTime ApplicationCloseDate { get; set; }
        public bool Status { get; set; }
    }
    
}
