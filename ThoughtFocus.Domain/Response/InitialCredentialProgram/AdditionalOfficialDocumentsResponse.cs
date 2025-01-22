using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class AdditionalOfficialDocumentsResponse:BaseResponse
    {
        public List<AdditionalOfficialDocuments> AdditionalOfficialDocuments { get; set; }
    }
    public class AdditionalOfficialDocuments
    {
        public int AdditionalOfficialDocumentID { get; set; }
        public int FormID { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public string UploadedByName { get; set; }
        public string CanView { get; set; }
    }
    public class FormExperienceAttachmentResponse : BaseResponse
    {
        public string fileName { get; set; }
    }
}
