using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class DropDownRequest
    {
        public int ProgramId { get; set; }
        public string ControlLabel { get; set; }
        public string ControlValue { get; set; }
        public bool Active { get; set; }
        public int Action { get; set; }
        public int DropdownId { get; set; }
    }
}
