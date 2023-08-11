using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.InitialCredentialProgram
{
    public class DeleteAdditionalOfficialDocumentRequest
    {
        public int ProgramID { get; set; }
        public int FormID { get; set; }
        public string TermCode { get; set; }
        public int UserID { get; set; }
        public int AdditionalOfficialDocumentID { get; set; }

    }
}
