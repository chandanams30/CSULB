using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormPersonalInfoSchemaRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string FormSchema { get; set; }
        public bool isYellowFlagEnabled { get; set; }

    }
    public class MoveApplicationToSemesterRequest
    {
        public int FormID { get; set; }
        public string TermCode { get; set; }
        public string FormSchema { get; set; }

    }
}
