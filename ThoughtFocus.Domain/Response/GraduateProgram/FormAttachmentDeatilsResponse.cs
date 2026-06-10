using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class FormAttachmentDeatilsResponse : BaseResponse
    {
        public FormAttachmentDeatils formAttachmentDeatils { get; set; }
    }
    public class FormDocumentResponse : BaseResponse
    {
        public List<FormDocumentDeatils> formdocumentDeatils { get; set; }
    }
    public class FormAttachmentDeatils
    {
        public int formID { get; set; }
        public int formAttachmentID { get; set; }
        public int documentID { get; set; }
        public int programID { get; set; }
        public string attachmentTitle { get; set; }
        public string fileName { get; set; }
        public string fileExtn { get; set; }
        public bool IsOptional { get; set; }
    }
    public class FormDocumentDeatils
    {
        public int formAttachmentID { get; set; }
        public int DocumentID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string AttachmentTitle {  get; set; }
        public bool IsOptional { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public string RejectReason { get; set; }
        public string Comments { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentInfo { get; set; }
        public bool CanUpload { get; set; }
        public bool CanValidate { get; set; }
    }
}
