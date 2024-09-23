using Microsoft.VisualBasic;
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
        public DateTime InterviewDate { get; set; }
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
    //public class UpsertInterviewSlotsList
    //{
    //    public int InterviewId { get; set; }
    //    public List<UpsertInterviewSlots> interviewSlotsList { get; set; }
    //}
}
