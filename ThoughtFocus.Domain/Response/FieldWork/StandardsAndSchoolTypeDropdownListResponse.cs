using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class StandardsAndSchoolTypeDropdownList : BaseResponse
    {
        public List<StandardsAndSchoolTypeDropdown> dropDowns { get; set; }
    }
    public class StandardsAndSchoolTypeDropdown
    {
        public int DropdownId { get; set; }
        public string ControlLabel { get; set; }
        public string ControlValue { get; set; }
        public bool Active { get; set; }
    }


}
