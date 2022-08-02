using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkValidationRequest
    {
        public int FieldWorkAttachmentId { get; set; }
        public int ApproverUserId { get; set; }
        public bool ApprovalStatus { get; set; }
        public Nullable<DateTime> ValidTill { get; set; }
        public string? RejectedReason { get; set; }
        public string? Comments { get; set; }
    }
}
