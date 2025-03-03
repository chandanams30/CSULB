using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FinalDecisionComment
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string FDComment { get; set; }
    }
}
