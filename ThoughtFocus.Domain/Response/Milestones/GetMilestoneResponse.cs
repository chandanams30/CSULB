using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.StudentProfile;

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
        public int MilestoneTypeID { get; set; }
        public string MilestoneTypeName { get; set; }
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

    public class GetMilestoneUsersListResponse : BaseResponse
    {
        public string MilestoneUsersList { get; set; }
    }
    
    public class GetMilestoneFilledFormByUserListResponse : BaseResponse
    {
        public string MilestoneFilledFormByUserList { get; set; }
    }
    public class GetMilestoneFilledFormByPublishedFormListResponse : BaseResponse
    {
        public string MilestonePublishedFormsList { get; set; }
    }
    public class GetMilestoneRequirementListResponse : BaseResponse
    {
        public string MilestoneRequirementList { get; set; }
    }
    public class GetMilestoneSubmittedFormsListResponse : BaseResponse
    {
        public List<GetMilestoneSubmittedFormsResponse> milestoneSubmittedFormsList { get; set; }
    }
    public class GetMilestoneSubmittedFormsResponse
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
    }
    public class GetMilestoneWorkflowProcessTransitionHistoryResponse : BaseResponse
    {
        public string WorkflowTransitionHistory { get; set; }
    }
    public class ApplicationProgramsResponse : BaseResponse
    {
        public List<ApplicationProgram> ProgramsList { get; set; }
    }
    public class ApplicationProgram
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
    }
}
