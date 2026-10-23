using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.StudentTeachingMilestone;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentTeachingMilestoneService
    {
        GetLatestFormResponse GetLatestForm(string csulbid);
        StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsuldId, int UserID, int formID, int roleID);
        DistrictListResponse GetDistrictsByProgramID(int programId);
        CredentialAreaListResponse GetCredentialPathwayAreaList();
        AttachmentResponse UpsertAttachement(UpsertAttachments input);
        DownloadAttachementResponse DownloadAttachement(Guid UniqueID);
        BaseResponse DeleteAttachement(DeleteAttachementRequest input);
        BaseResponse AddCredentialArea(AddCredentialAreaRequest input);
    }
}
