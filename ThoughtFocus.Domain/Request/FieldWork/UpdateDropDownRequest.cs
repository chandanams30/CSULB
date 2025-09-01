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
    public class UpsertInternCourseForTermRequest
    {
        public string CSULBID { get; set; }
        public string Course { get; set; }
        public string TermCode { get; set; }
        public string SecID {  get; set; }
        public string CourseNumber { get; set; }
    }
}
