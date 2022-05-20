using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Program;

namespace ThoughtFocus.Domain.Response.Program
{
    public class ProgramResponse
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int ApplicationTypeId { get; set; }
        public string ApplicationName { get; set; }
        public string FormTemplate { get; set; }
        public string FormNotes { get; set; }

        public int ApplicationCount { get; set; }
        public int OfferedCount { get; set; }
        public object Template { get; set; }
    }
}
