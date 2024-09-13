using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Interviews;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Interviews;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IInterviewService
    {
        BaseResponse UpsertInterview(UpsertInterview input);
        InterviewDetails GetInterviewDetails(int InterviewId);
        BaseResponse UpsertInterviewSlots(UpsertInterviewSlots input);
        InterviewList GetInterviewList(int UserId);
        InterviewersList GetInterviewerList();
    }
}
