using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkCourseList : BaseResponse
    {
        public List<FieldWorkCourse> Courses { get; set; }
    }
    public class FieldWorkCourse
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
