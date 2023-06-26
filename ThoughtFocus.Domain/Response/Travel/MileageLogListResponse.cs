using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Travel
{
    public class MileageLogListResponse:BaseResponse
    {
        public SupervisorPersonalInfo PersonalInfo { get; set; }
        public List<TravelPrerequisites> TravelPrerequisites { get; set; }
        public List<MileageLogDetailsList> MileageLogDetailsList { get; set; }
    }
    public class SupervisorPersonalInfo
    {
        public int BusinessMileageSupervisorUserID { get; set; }
        public string BusinessMileageSupervisorName { get; set; }

        public string Email { get; set; }
        public string CSULBID { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int BusinessMileageSupervisorID { get; set; }
        public bool isPersonalInfoCompleted { get; set; }
        public bool IsTravelPrerequisitesCompleted { get; set; }

    }
    public class TravelPrerequisites
    {
        public int TravelAttachmentID { get; set; }
        public int BusinessMileageSupervisorUserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string DocumentInfo { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public string RejectReason { get; set; }
        public string Comments { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string PrerequisiteStatus { get; set; }
        public Boolean ShowUpload { get; set; }
        public Boolean ShowReview { get; set; }
    }
    public class MileageLogDetailsList
    {
        public int BusinessMileageLogID { get; set; }
        public int BusinessMileageSupervisorID { get; set; }
        public DateTime TravelDateTime { get; set; }
        public string StartingLocation { get; set; }
        public string DestinationAddress { get; set; }
        public string BusinessPurpose { get; set; }
        public decimal Miles { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public decimal Rate { get; set; }
    }
}
