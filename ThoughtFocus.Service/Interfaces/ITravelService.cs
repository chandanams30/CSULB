using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Travel;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Travel;

namespace ThoughtFocus.Service.Interfaces
{
    public interface ITravelService
    {
        FacultySupervisorListResponse GetFacultySupervisorList(int UserID);
        BaseResponse UpsertBusinessMileageSupervisorList(UpsertFacultySupervisorRequest input);
        BaseResponse UpsertBusinessMileageLog(UpsertBusinessMileageLogRequest input);
        MileageLogListResponse GetBusinessMileageLogList(int BusinessMileageSupervisorID, int BusinessMileageSupervisorUserID);
        MileageLogResponse GetBusinessMileageLog(int BusinessMileageLogID);
        BaseResponse UpdatePrerequisitesDocument(UpdatePrerequisitesDocumentRequest input,Boolean isValidation);
        TravelPrerequisiteAttachments DownloadPrerequisitesDocument(int TravelPrerequisiteID, int BusinessMileageSupervisorUserID);
        ReportDataResponse GetReportData(ReportDataRequest input);
        BaseResponse ApproveBusinessMileageLog(ApproveBusinessMileageLog input);
    }
}
