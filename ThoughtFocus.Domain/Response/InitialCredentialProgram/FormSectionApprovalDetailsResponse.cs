using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class FormSectionApprovalDetailsResponse:BaseResponse
    {
        public int ID { get; set; }
        public int FormID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public int? ApproverUserID { get; set; }
        public string ApprovedBy { get; set; }
        public bool? isApproved { get; set; }
        public string ApproverComments { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool showSubSectionApproveral { get; set; }
        public bool canUpdateSubSectionApproveral { get; set; }
        public string ReviewedByText { get; set; }

        public string Status { get; set; }
    }
}
