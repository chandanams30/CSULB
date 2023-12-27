using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestoneApplicationFormsListResponse:BaseResponse
    {
        public List<MilestoneApplicationFormsList> MilestoneApplicationFormsList { get; set; }
    }
    public class MilestoneApplicationFormsList
    {
        public int MilestoneFormsID { get; set; }
        public int MilestonePublishedFormID { get; set; }
        public int FormID { get; set; }
        public bool Status { get; set; }
        public string MilestoneName { get; set; }       
    }

    public class GetMilestoneApplicationFormResponse : BaseResponse 
    {
        public MilestoneApplicationForm MilestoneApplicationForm { get; set; }
        public MilestoneFormActivityHandler MilestoneFormActivityHandler { get; set; }
        public MileStoneFilledFormApprovers MileStoneFilledFormApprovers { get; set; }
    }
    public class MilestoneApplicationForm
    {
        public int MilestoneFormID { get; set; }
        public int MilestonePublishedFormID { get; set; }
        public int FormID { get; set; }
        public string MilestoneFilledForm { get; set; }
        public bool Status { get; set; }
        public string MilestoneForm { get; set; }
        public string MilestoneName { get; set; }
        public bool isEditable { get; set; }
    }
    public class MilestoneFormActivityHandler
    {
        public string MilestoneActivityHandler { get; set; }
    }
    public class MileStoneFilledFormApprovers
    {
        public string MileStoneFilledFormApproversDetails { get; set; }
    }
}
