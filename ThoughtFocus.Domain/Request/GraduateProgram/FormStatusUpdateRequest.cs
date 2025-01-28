using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormStatusUpdateRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int FormStateID { get; set; }
        public int  WaitlistNumber { get; set; }
        public string WaitlistComments { get; set; }
        public int FinalDecision { get; set; }
    }
    public class BulkNotOfferFormStatusUpdateRequest
    {
        public int UserID { get; set; }
        public List<int> FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int FormStateID { get; set; }
    }
}
