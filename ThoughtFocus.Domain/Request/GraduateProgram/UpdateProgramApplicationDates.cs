using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class UpdateProgramApplicationDatesRequest
    {
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public DateTime ApplicationOpens { get; set; }
        public DateTime ApplicationDeadline { get; set;}
        public DateTime ApplicationCloseDate { get; set; }
        public bool Status { get; set; }
    }
    
}
