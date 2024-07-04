using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.InitialCredentialProgram
{
    public class UpdateAdditionalOfficialDocumentRequest
    {
        public int AdditionalOfficialDocumentID { get; set; }
        public int FormID { get; set; }
        public string FileName { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public byte[] FileContent { get; set; }

    }
}
