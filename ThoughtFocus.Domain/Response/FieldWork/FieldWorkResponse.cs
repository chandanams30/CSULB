using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkResponse
    {
        public int FieldWorkId { get; set; }
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public string CSULBCourseID { get; set; }
        public string College { get; set; }
        public string Term { get; set; }
        public bool IsTBTest { get; set; }
        public bool IsCtcDone { get; set; }
    }
}
