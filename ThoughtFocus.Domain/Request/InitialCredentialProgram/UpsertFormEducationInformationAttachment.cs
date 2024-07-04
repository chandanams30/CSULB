using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.InitialCredentialProgram
{
    public class UpsertFormEducationInformationAttachmentRequest
    {
        public Guid UniqueID { get; set; }
        public int FormID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public int UserID { get; set; }
    }
}
