using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Travel
{
    public class TravelPrerequisiteAttachments:BaseResponse
    {
        
        public int TravelAttachmentID { get; set; }
        public int BusinessMileageSupervisorUserID { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public byte[] FileContent { get; set; }
        public string RejectReason { get; set; }
        public string Comments { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }

    }
}
