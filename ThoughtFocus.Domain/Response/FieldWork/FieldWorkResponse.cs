using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkResponse
    {
        public int FieldWorkId { get; set; }
        public string StudentName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CSULBID { get; set; }
        public string CourseTitle { get; set; }
        public string CSULBCourseID { get; set; }
        public string College { get; set; }
        public string Section { get; set; }
        public string Term { get; set; }
        public int FieldWorkPrerequisiteStatus { get; set; }
        public string PrerequisiteStatus { get; set; }
        public string UIHandler { get; set; }
    }

    public class FieldWorkListResponse:BaseResponse
    {
        public List<FieldWorkResponse> FieldWorkResponse { get; set; }
    }
}
