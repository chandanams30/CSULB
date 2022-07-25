using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("TransitionDefinition", Schema = "WorkFlow")]
    public partial class TransitionDefinition : BaseDefinition
    {
        public long ConditionID { get; set; }
        public int TransitionClassifierID { get; set; }
        public long TriggerID { get; set; }
        public long WorkflowDefinitionID { get; set; }
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }  
         [NotMapped]
        public long From_ID { get; set; }
         [NotMapped]
        public long To_ID { get; set; }
        public virtual ActivityDefinition From { get; set; }
        public virtual ActivityDefinition To { get; set; }
        
        public virtual TransitionClassifier TransitionClassifier { get; set; } // navigation property
         
        public virtual ICollection<TransitionValidationDefination> TransitionValidationDefinations { get; set; }
        public ICollection<RestrictionDefinition> RestrictionLazyLoading;
        
        
        public virtual IEnumerable<RestrictionDefinition> Restrictions 
        {
           get{ return RestrictionLazyLoading;
           
           }
        }
       [BackingField(nameof(RestrictionLazyLoading))]
       public virtual ICollection<RestrictionDefinition> RestrictionsList { 
           get{ return RestrictionLazyLoading = new Collection<RestrictionDefinition>();}
       
       set{}
       }
       

       [ForeignKey("TriggerID")]
       public virtual TriggerDefinition Trigger { get; set; }

        [ForeignKey("ConditionID")]
        public virtual ConditionDefinition Condition { get; set; }
        

        [NotMapped]
        public IEnumerable<OnErrorDefinition> OnErrors
        {
            get { return OnErrorsList; }
        }
        
        [NotMapped]
        protected List<OnErrorDefinition> OnErrorsList;

        public static TransitionDefinition Create(string name, string clasifier, ActivityDefinition from, ActivityDefinition to, TriggerDefinition trigger, ConditionDefinition condition)
        {
            TransitionClassifierEnumeration parsedClassifier;
            Enum.TryParse(clasifier, true, out parsedClassifier);
            return new TransitionDefinition()
            {
                Name = name,
                To = to,
                From = from,
                TransitionClassifier = parsedClassifier,
                Condition = condition ?? ConditionDefinition.Always,
                Trigger = trigger ?? TriggerDefinition.Auto,
                RestrictionsList = new List<RestrictionDefinition>(),
                OnErrorsList = new List<OnErrorDefinition>()
            };
        }

        public static TransitionDefinition Create(ActivityDefinition from, ActivityDefinition to)
        {
            return new TransitionDefinition()
            {
                Name = "Undefined",
                To = to,
                From = from,
                TransitionClassifier = TransitionClassifierEnumeration.NotSpecified,
                Condition = ConditionDefinition.Always,
                Trigger = TriggerDefinition.Auto,
                RestrictionsList = new List<RestrictionDefinition>(),
                OnErrorsList = new List<OnErrorDefinition>()
            };
        }

        public void AddRestriction(RestrictionDefinition restriction)
        {
            RestrictionsList.Add(restriction);
        }

        public void AddOnError(OnErrorDefinition onError)
        {
            OnErrorsList.Add(onError);
        }
    }
}
