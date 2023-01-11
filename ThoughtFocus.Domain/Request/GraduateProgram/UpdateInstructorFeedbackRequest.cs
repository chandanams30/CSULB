using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class UpdateInstructorFeedbackRequest
    {
        public int InstructionID { get; set; }
        public int InstructorID { get; set; }
        public int FormID { get; set; }
        public int InstructionAttachmentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class UpdateInterviewerFeedbackRequest
    {
        public int InterviewID { get; set; }
        public int InterviewerID { get; set; }
        public int FormID { get; set; }
        public int InterviewAttachmentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class AddInstructorRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int InstructorUserID { get; set; }
    }
    public class AddInterviewerRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int InterviewerUserID { get; set; }
    }

    public class GetInstructorInterviewerListRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }

    }


}
