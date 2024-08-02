using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Domain.Response.Guests
{
    public class GetMilestoneSubmittedFormsList : BaseResponse
    {
        public List<GetMilestoneSubmittedForms> milestoneSubmittedFormsList { get; set; }
    }
   
    public class GetMilestoneSubmittedForms
    {
        public int MilestoneFormID { get; set; }
        public string ApproverName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string State { get; set; }
        public int CSULBID { get; set; }
        public string StudentName { get; set; }
        public string ProgramName { get; set; }
        public string MilestoneName { get; set; }
        public int FormID { get; set; }
        public string TermName { get; set; }
        public int MilestonePublishedFormID { get; set; }
        public int MileStoneFormApproverExternalId { get; set; }
    }
}
