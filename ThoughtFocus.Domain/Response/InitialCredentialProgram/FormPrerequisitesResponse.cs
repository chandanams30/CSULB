using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class FormPrerequisitesResponse : BaseResponse
    {
        public List<FormPrerequisites> FormPrerequisites { get; set; }
    }
    public class FormPrerequisites
    {
        public int fieldWorkAttachmentID { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RejectReason { get; set; }
        public string Comments { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentInfo { get; set; }
        public bool CanUpload { get; set; }
        public bool CanValidate { get; set; }
    }
}
