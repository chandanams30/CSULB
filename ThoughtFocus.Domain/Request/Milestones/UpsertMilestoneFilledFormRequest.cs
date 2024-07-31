using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Milestones
{
    public class UpsertMilestoneFilledFormRequest
    {
        public int MilestoneFormID { get; set; }
        public int MilestoneID { get; set; }
        public int FormID { get; set; }
        public string MilestoneFilledForm { get; set; }
        public int createdByUserID { get; set; }
        public int? ApproverID { get; set; }
        public string ApproverComments { get; set; }
        public int ActivityDefinitionID { get; set; }
        public string ActivityDefinitionState { get; set; }
        public string ActivityControlLabel { get; set; }
        public string ExternalApprovers { get; set; }
    }
}
