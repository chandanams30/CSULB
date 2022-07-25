using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkAttachmentsRequest
    {
        public int UserID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
