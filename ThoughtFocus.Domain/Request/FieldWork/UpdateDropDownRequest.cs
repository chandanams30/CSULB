using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class UpdateDropDownRequest
    {
        public int DropdownId { get; set; }
        public int CategoryID { get; set; }
        public string DropdownType { get; set; }
        public int Action { get; set; }
        public string ControlLabel { get; set; }
        public string ControlValue { get; set; }
        public int UserID { get; set; }
    }
}
