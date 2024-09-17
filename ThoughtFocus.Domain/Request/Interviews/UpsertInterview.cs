using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Interviews
{
    public class UpsertInterview
    {
        public string InterviewName { get; set; }
        public string InterviewDescription { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public int ProgramId { get; set; }
        public string TermCode { get; set; }
        public int InterviewFor { get; set; }
        public int InterviewId { get; set; }
        public string Status { get; set; }
    }
    public class UpsertInterviewSlots
    {
        public int InterviewId { get; set; }
        public DateTime InterviewStart { get; set; }
        public DateTime InterviewEnd { get; set; }
        public int CreatedBy { get; set; }
        public int  Interviewers { get; set; }
        public int Students { get; set; }
        public string Status { get; set; }
        public string InterviewComments { get; set; }
        public string InterviewLocation { get; set; }
        public string InterviewLink { get; set; }
        public int InterviewSlotId { get; set; }
    }
    public class UpsertInterviewSlotsList
    {
        public List<UpsertInterviewSlots> interviewSlotsList { get; set; }
    }
}
