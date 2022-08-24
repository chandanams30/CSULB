using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class ProgramListResponse
    {
        public int ApplicationProgramID { get; set; }
        public string ProgramName { get; set; }
        public string TermCode { get; set; }
        public string TermName { get; set; }
    }
}
