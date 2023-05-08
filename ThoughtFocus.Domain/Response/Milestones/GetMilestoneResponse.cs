using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class GetMilestoneResponse : BaseResponse
    {
        public MilestoneDetails MilestoneDetails { get; set; }
        public List<ApproverDetails> ApproverDetails { get; set; }
    }
    public class MilestoneDetails
    {
        public int MilestoneID { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public string MilestoneForm { get; set; }
        public bool isPublished { get; set; }
        public bool isMandatory { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByName { get; set; }
    }
    public class ApproverDetails
    {
        public int ApproverUserID { get; set; }
        public int Sequence { get; set; }
        public string ApproverUserName { get; set; }
    }
    public class MilestoneApproverUserListResponse : BaseResponse
    {
        public List<MilestoneApproverUserList> MilestoneApproverUserList { get; set; }
    }
    public class MilestoneApproverUserList
    {
    public int ApproverUserID { get; set; }
    public string ApproverUserName { get; set; }
}
}
