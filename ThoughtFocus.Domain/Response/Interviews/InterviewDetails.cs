using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Interviews;
using ThoughtFocus.Domain.Response.GraduateProgram;

namespace ThoughtFocus.Domain.Response.Interviews
{
    public class InterviewDetails : BaseResponse
    {
        public BasicDetails basicDetails { get; set; }
        public List<SlotsList> slotsList {  get; set; } 
        public InterviewSateHandler interviewSateHandler {  get; set; } 
    }
    public class BasicDetails
    {
        public int InterviewId { get; set; }
        public string InterviewName { get; set; }
        public string InterviewDescription { get; set; }
        public string ProgramName { get; set; }
        public string Semester { get; set; }
        public int ApplicationProgramID { get; set; }
        public string SemesterTerm { get; set; }
        public int ApplicationTypeID { get; set; }

    }
    public class SlotsList
    {
        public int InterviewSlotId { get; set; }
        public string InterviewDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string InterviewerName { get; set; }
        public string StudentName { get; set; }
        public string Status { get; set; }
        public string InterviewComments { get; set; }
        public string InterviewLocation { get; set; }
        public string InterviewLink { get; set; }
        public int Interviewer {  get; set; }
        public int Student { get; set; }
        public string ReasonforReschedule { get; set; }
        public bool ShowAcceptInterview { get; set; }
        public bool ShowRejectInterview { get; set; }
        public bool ShowRescheduleInterview { get; set; }
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
    public class InterviewBasicDetails
    {
        public int InterviewId { get; set; }
        public string InterviewName { get; set; }
        public string InterviewDescription { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int ApplicationProgramID { get; set; }
        public string SemesterTerm { get; set; }
        public int InterviewFor { get; set; }
        public string Status { get; set; }
    }
    public class InterviewBasicDetailsResponse : BaseResponse
    {
        public InterviewBasicDetails interviewBasicDetails { get; set; }
    }
    public class InterviewSateHandler
    {
        public string StateHandler { get; set; }
    }
    public class InterviewSlots 
    {
        //public int InterviewId { get; set; }
        public string InterviewDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int CreatedBy { get; set; }
        public int  Interviewer { get; set; }
        public int Student { get; set; }
        public string Status { get; set; }
        public string InterviewComments { get; set; }
        public string InterviewLocation { get; set; }
        public string InterviewLink { get; set; }
        public int InterviewSlotId { get; set; }
    }
    public class InterviewSlotsList : BaseResponse
    {
        public List<InterviewSlots> interviewSlotsList { get; set; }
        public string AlertMessage { get; set; }
    }
}
