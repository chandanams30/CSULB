using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UploadMessageBoardAttachmentRequest
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
