using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class AppliedFormsResponse : BaseResponse
    {
       public List<AppliedForms> appliedForms { get; set; }
    }
    public class AppliedForms
    {
        public int formID { get; set; }
        public int formStateID { get; set; }
        public string status { get; set; }
        public int ProgramID { get; set; }
        public string programName { get; set; }
        public string TermCode { get; set; }
        public string semester { get; set; }
        public DateTime appliedDate { get; set; }
    }
}
