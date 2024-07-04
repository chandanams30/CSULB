using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class ProgramApplicationDates : BaseResponse
    {
        public DateTime ApplicationOpens { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public DateTime ApplicationCloseDate { get; set; }
        public bool Status { get; set; }
    }

  
}
