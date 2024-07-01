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

    public class UpdateFormSubSectionSubmitForReviewRequest
    {
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public bool IsSubmitForReview { get; set; }

    }
    public class DeleteSubSectionAttachmentRequest
    {
        public int FormID { get; set; }
        public string FileName { get; set; }

    }
}
