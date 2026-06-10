using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormUpsertAttachmentRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FormSchema {  get; set; }
    }

    public class DeleteFormAttachmentRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public int DocumentID { get; set; }
        public string TermCode { get; set; }
        public int FormAttachmentID { get; set; }
    }

    public class DeleteInsructorAttachmentRequest
    {
        public int InstructionID { get; set; }
        public int InstructorUserID { get; set; }
        public int FormID { get; set; }
        public int InstructionAttachmentID { get; set; }
    }
    public class FormDocumentRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime ValidTill { get; set; }
        public string Comments { get; set; }
        public DateTime UploadedDate { get; set; }
    }
    public class FormDocumentValidationRequest
    {
        public int FormID { get; set; }
        public bool ApprovalStatus { get; set; }
        public int ApproverUserId { get; set; }
        public Nullable<DateTime> ValidTill { get; set; }
        public string? RejectedReason { get; set; }
        public string? Comments { get; set; }
    }
}
