using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkActivityLogsAttachmentResponse : BaseResponse
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
