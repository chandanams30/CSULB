using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Form
{
    public class FormResponse
    {
        public int FormId { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public object Form { get; set; }
    }
}
