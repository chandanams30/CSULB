using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ThoughtFocus.Common.Workflow.Core.Model
{

    [Table("ActivityDefinition", Schema = "WorkFlow")]
    public class ActivityDefinition : BaseDefinition
    {
        public string State { get;  set; }
        public bool IsInitial { get;  set; }
        public bool IsFinal { get;  set; }
        public bool IsForSetState { get;  set; }
        public bool IsAutoSchemeUpdate { get;  set; }
        public long WorkflowDefinitionID { get; set; }

        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
       
        public virtual ICollection<ActionDefinitionForActivity> ActionDefinitionForActivities { get; set; }

      
    
        public bool HaveImplementation
        {

            get
            {

                if (Implementation != null)
                {
                    return Implementation.Count > 0;
                }
                else
                {
                    return false;
                }
            }
           
        }

        public bool HavePreExecutionImplementation
        {
            get
            {
                if (PreExecutionImplementation != null)
                {
                    return PreExecutionImplementation.Count > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        [NotMapped]
        public List<ActionDefinitionForActivity> Implementation
        {
            get
            {
                if(ActionDefinitionForActivities!=null)
                {
                    return ActionDefinitionForActivities.Where(a => a.IsPostExecution == true).ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }
        [NotMapped]
        public List<ActionDefinitionForActivity> PreExecutionImplementation
        {
            get
            {
                if (ActionDefinitionForActivities != null)
                {
                    return ActionDefinitionForActivities.Where(a => a.IsPostExecution == false).ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }

        public bool IsState
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public static ActivityDefinition Create(string name, string stateName, string isInitial, string isFinal, string isForSetState, string isAutoSchemeUpdate)
        {
            return new ActivityDefinition()
                       {
                           IsFinal = !string.IsNullOrEmpty(isFinal) && bool.Parse(isFinal),
                           IsInitial = !string.IsNullOrEmpty(isInitial) && bool.Parse(isInitial),
                           IsForSetState = !string.IsNullOrEmpty(isForSetState) && bool.Parse(isForSetState),
                           IsAutoSchemeUpdate = !string.IsNullOrEmpty(isAutoSchemeUpdate) && bool.Parse(isAutoSchemeUpdate),
                           Name = name,
                           State = stateName,
                           Implementation = new List<ActionDefinitionForActivity>(),
                           PreExecutionImplementation = new List<ActionDefinitionForActivity>()
                       };
        }

        public void AddAction(ActionDefinitionForActivity action)
        {
            Implementation.Add(action);
        }

        public void AddPreExecutionAction(ActionDefinitionForActivity action)
        {
            PreExecutionImplementation.Add(action);
        }
    }
}
