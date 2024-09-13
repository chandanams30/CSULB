using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Interviews
{
    public class InterviewDetails : BaseResponse
    {
        public InterviewDetailsResponse interviewDetailsResponse { get; set; }
    }
    public class InterviewDetailsResponse
    {
        public int InterviewId { get; set; }
        public string InterviewName { get; set; }
        public string InterviewDescription { get; set; }
        public string ProgramName { get; set; }
        public string Semester { get; set; }
        public DateTime InterviewStart { get; set; }
        public DateTime InterviewEnd { get; set; }
        public string InterviewerName { get; set; }
        public string StudentName { get; set; }
        public string Status { get; set; }
        public string InterviewerComments { get; set; }
        public string InterviewLocation { get; set; }
        public string InterviewLink { get; set; }
    }
    public class InterviewList : BaseResponse
    {
        public List<InterviewListResponse> interviewListResponse { get; set; }
    }
    public class InterviewListResponse
    {
        public int InterviewId { get; set; }
        public string InterviewName { get; set; }
        public string InterviewDescription { get; set; }
        public string ProgramName { get; set; }
        public string Semester { get; set; }
    }
    public class InterviewersList : BaseResponse
    {
        public List<InterviewersListResponse> interviewersList { get; set; }
    }
    public class InterviewersListResponse
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
    }
}
