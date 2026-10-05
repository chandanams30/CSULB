using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.StudentTeachingMilestone
{
    public class GetStudentTeachingMilestoneResponse
    {

    }

        public class GetLatestFormResponse : BaseResponse
        {
            public GetLatestFormDetails getLatestFormDetails { get; set; }
        }
        public class GetLatestFormDetails
        {
            public int FormId { get; set; }
            public int ProgramID { get; set; }
            public string TermCode { get; set; }
        }
}
