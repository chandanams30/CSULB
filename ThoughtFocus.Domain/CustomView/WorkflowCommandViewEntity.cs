namespace ThoughtFocus.Domain.CustomView
{
    public class WorkflowCommandViewEntity
    {
        public long WorkflowCommandID { get; set; }
        public string CommandIconClass { get; set; }
        public string LocalizedName { get; set; }
        public string Name { get; set; }
        public string TransitionType { get; set; }
        public string CommandTransitionType { get; set; }

        public WorkFlowCommandValidationEntity WorkFlowCommandValidationEntity { get; set; }

    }
}
