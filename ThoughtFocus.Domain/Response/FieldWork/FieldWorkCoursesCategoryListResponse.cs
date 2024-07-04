using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkCoursesCategoryList : BaseResponse
    {
        public List<FieldWorkCoursesCategory> Categories { get; set; }
    }
    public class FieldWorkCoursesCategory
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }
}
