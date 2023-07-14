using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Milestones
{
    public class UpsertMilestoneTemplateRequest
    {
        public int MilestoneTemplateID { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public string MilestoneForm { get; set; }
        public bool isReadyToPublish { get; set; }
        public int createdByUserID { get; set; }


    }
}
