using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.StudentProfile
{
    public class UpsertProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
    }
    public class DownloadProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class ProfileAttachmentDetailsResponse : BaseResponse
    {
        public List<ProfileAttachmentDetails> ProfileAttachmentDetails { get; set; }
    }
    public class ProfileAttachmentDetails
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CanView { get; set; }
    }
}
