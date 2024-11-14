using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class StandardsAndSchoolTypeDropdownList : BaseResponse
    {
        public List<StandardsAndSchoolTypeDropdown> DropdownList { get; set; }
    }
    public class StandardsAndSchoolTypeDropdown
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


}
