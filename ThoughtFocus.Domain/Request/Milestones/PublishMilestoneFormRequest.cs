using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Milestones
{
    public class PublishMilestoneFormRequest
    {
        public int MilestoneTemplateID { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public string MilestoneForm { get; set; }
        public int createdByUserID { get; set; }
        public bool isMandatory { get; set; }
        public List<MilestonePublishedFormApprovers> MilestonePublishedFormApprovers { get; set; }
        public List<MilestonePublishedFormUsers> MilestonePublishedFormUsers { get; set; }
        public int MilestoneTypeID { get; set; }
        public string MilestoneRequirement { get; set; }
    }

    public class MilestonePublishedFormApprovers
    {
        public int MilestoneApproverTypeID { get; set; }
        public int? ApproverUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Sequence { get; set; }
    }
    public class MilestonePublishedFormUsers
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
    }
    public class UpsertMilestoneFormAttachment
    {
        public string GUID { get; set; }
        public byte[] FileContent { get; set; }
    }
}
