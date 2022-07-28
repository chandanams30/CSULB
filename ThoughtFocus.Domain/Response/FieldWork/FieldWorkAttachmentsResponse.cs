using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkAttachmentsResponse : BaseResponse
    {
        public string AttachmentID { get; set; }
        public string FileSavedName { get; set; }
        public string FileDisplayName { get; set; }
    }
}
