using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class DropDownListResponse : BaseResponse
    {
       public List<DropDowns> DropDowns { get; set; }
    }
    public class ControlLabelListResponse : BaseResponse
    {
        public List<ConrolLabel> ConrolLabels { get; set; }
    }
    public class DropDowns
    {
        public int DropdownId { get; set; }
        public int ProgramID { get; set; }
        public string ControlLabel { get; set; }
        public string ControlValue { get; set; }
        public bool Active { get; set; }
    }
    public class ConrolLabel
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
