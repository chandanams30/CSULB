using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class InstructorListResponse:BaseResponse
    {
        public List<InstructorList> InstructorList { get; set; }
    }
    public class InstructorList
    {
        public int InstructorUserID { get; set; }
        public string InstructorName { get; set; }
    }
    public class InterviewerListResponse:BaseResponse
    {
        public List<InterviewerList> InterviewerList { get; set; }
    }
    public class InterviewerList
    {
        public int InterviewerUserID { get; set; }
        public string InterviewerName { get; set; }
    }
}
