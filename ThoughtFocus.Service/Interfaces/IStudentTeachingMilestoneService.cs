using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.StudentTeachingMilestone;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentTeachingMilestoneService
    {
        GetLatestFormResponse GetLatestForm(string csulbid);
        StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsuldId, int UserID, int formID, int roleID, int programID);
        DistrictListResponse GetDistrictsByProgramID(int programId);
        CredentialAreaListResponse GetCredentialPathwayAreaList();
        AttachmentResponse UpsertAttachement(UpsertAttachments input);
        DownloadAttachementResponse DownloadAttachement(Guid UniqueID);
        BaseResponse DeleteAttachement(DeleteAttachementRequest input);
        BaseResponse AddCredentialArea(AddCredentialAreaRequest input);
        BaseResponse SaveStudentTeachingMilestone(StudentTeachingMilestoneRequest input);
        BaseResponse AssignApproverAssigner(AssignApproverAssignerRequest input);
        ExternalApproversListResponse GetExternalApprovers();
        BaseResponse AddExternalApprover(AddExternalApproverRequest input);
        ApproverAssignmentsResponse GetExternalApproverAssignments(ApproverAssignmentsRequest input);
        BaseResponse UpdateMilestoneFormState(UpdateFormStateRequest input);
        MandateFieldValidationResponse isSubmitEnableST(MandateFieldValidationRequest input);
        ADGroupUsersListResponse GetADGroupListForSTMilestone();
        ADGroupUsersList GetUserIDByCSULBID(string CSULBID);
        ADGroupUsersListResponse SendSTMilestoneNotificationEmail(string CSULBID, int ProgramID);
        BaseResponse UpdateMilestoneWorkflowDetails(UpdateMilestoneWorkflowRequest input);
        MilestoneWorkflowDetailsListResponse GetMilestoneWorkflowDetails(string CSULBID, int FormID);
        GetSTMilestoneWorkflowProcessTransitionHistoryResponse GetSTMilestoneWorkflowProcessTransitionHistory(int FormID);
    }
}
