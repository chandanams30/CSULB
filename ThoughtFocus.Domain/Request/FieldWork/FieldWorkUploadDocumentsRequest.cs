using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkUploadDocumentsRequest
    {
        public int UserID { get; set; }
        public int FieldWorkAttachmentId { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime ValidTill { get; set; }
        public string Comments { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
