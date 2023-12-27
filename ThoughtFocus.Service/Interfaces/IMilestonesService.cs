using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Milestones;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IMilestonesService
    {
        MilestonesListResponse GetMilestoneList();
        BaseResponse UpsertMilestone(UpsertMilestoneRequest input);
        BaseResponse UpsertMilestoneFilledForm(UpsertMilestoneFilledFormRequest input);
        BaseResponse AssignMilestoneProgram(AssignMilestoneToProgram input);
        PublishedMilestonesListResponse GetPublishedMilestoneList(int ProgramID, string TermCode);
        MilestoneApplicationFormsListResponse GetMilestoneApplicationFormsList(int UserID, int FormID, int ProgramID, string TermCode);
        GetMilestoneApplicationFormResponse GetMilestoneApplicationForm(int UserID, int MilestoneFormID, int FormID);
        GetMilestoneResponse GetMilestone(int MilestoneID);
        MilestoneApproverUserListResponse GetMilestoneApproverUserList();
        MilestoneTypesResponse GetMilestoneTypes();
        BaseResponse UpsertMilestoneTemplate(UpsertMilestoneTemplateRequest input);
        MilestoneTemplateListResponse GetMilestoneTemplateList(bool isReadyToPublish);
        MilestoneTemplateByIDResponse GetMilestoneTemplateByID(int MilestoneTemplateID);
        MilestonesApproverTypesResponse GetMilestoneApproverTypes();
        BaseResponse PublishMilestoneForm(PublishMilestoneFormRequest input);
        MilestoneUsersListResponse GetMilestoneUsersList(int RoleID, int ProgramID, string TermCode);
        MilestoneProgramTermListResponse GetMilestoneProgramTermList();
        MilestonePublishedFormsListResponse GetMilestonePublishedFormsList(int UserID);
        GetMilestoneUsersListResponse GetMilestoneUsersList();
        GetMilestoneFilledFormByUserListResponse GetMilestoneFilledFormByUserList(int UserID);
        GetMilestoneFilledFormByPublishedFormListResponse GetMilestoneFilledFormByPublishedFormList(int MilestonePublishedFormID, int UserID);
    }
}
