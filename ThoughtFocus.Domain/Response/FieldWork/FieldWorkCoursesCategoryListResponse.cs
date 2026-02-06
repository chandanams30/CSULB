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
        public string Value { get; set; }
        public string Label { get; set; }
    }
    public class FieldWorkSubjectList : BaseResponse
    {
        public List<FieldWorkSubjects> SubjectList { get; set; }
    }
    public class FieldWorkSubjects
    {
        public string Subject { get; set; }
    }
}
