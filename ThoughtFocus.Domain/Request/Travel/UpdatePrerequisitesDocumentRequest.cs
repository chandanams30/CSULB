using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Travel
{
    public class UpdatePrerequisitesDocumentRequest
    {
        public int TravelPrerequisiteID { get; set; }
        public int BusinessMileageSupervisorUserID { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime ValidTill { get; set; }
        public string Comments { get; set; }
        public Boolean IsApproved { get; set; }
        public int ApprovedBy { get; set; }
        public string RejectedReason { get; set; }


    }
    public class ApproveBusinessMileageLog
    {
        public int BusinessMileageLogID { get; set; }
        public Boolean IsApproved { get; set; }
    }
 }
