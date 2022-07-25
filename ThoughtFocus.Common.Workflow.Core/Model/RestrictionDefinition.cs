using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("RestrictionDefinition", Schema = "WorkFlow")]
    public class RestrictionDefinition  //: IEnumerable 
    {
        [Key]
        public virtual long ID { get; set; }
        [NotMapped]       
        public long Actor_ID { get; set; }
        public int? RestrictionType_Id { get; set; }

        [ForeignKey("RestrictionType_Id")]
        public virtual RestrictionType RestrictionType{ get;  set; }  
       
        [NotMapped]
        [ForeignKey("Actor_ID")]
        public ActorDefinition Actor
        {
            get
            {
                if (ActorDefinitionExecuteRule != null)
                {
                    return ActorDefinitionExecuteRule as ActorDefinition;
                }
                else if (ActorDefinitionIsIdentity != null)
                {
                    return ActorDefinitionIsIdentity as ActorDefinition;
                }
                else
                {
                    return ActorDefinitionIsInRole as ActorDefinition;
                }
            }
            set
            {
            }
        }
    
         [ForeignKey("Transition_ID")]
        public virtual TransitionDefinition Transition { get; set; }
         public long? Transition_ID { get; set; }
       
        [NotMapped]
        public long TransitionDefinition_ID { get; set; }

        [ForeignKey("ActorDefinitionExecuteRule_ID")]
        public virtual ActorDefinitionExecuteRule ActorDefinitionExecuteRule { get; set; }
        [ForeignKey("ActorDefinitionIsIdentity_ID")]
        public virtual ActorDefinitionIsIdentity ActorDefinitionIsIdentity { get; set; }
        [ForeignKey("ActorDefinitionIsInRole_ID")]
        public virtual ActorDefinitionIsInRole ActorDefinitionIsInRole { get; set; }       

       
        public long? ActorDefinitionExecuteRule_ID { get; set; }

        public long? ActorDefinitionIsIdentity_ID { get; set; }

        public long? ActorDefinitionIsInRole_ID { get; set; }
          
        public static RestrictionDefinition Create (string type, ActorDefinition actor)
        {
            RestrictionTypeEnumeration parsedType;
            Enum.TryParse(type, true, out parsedType);

            return new RestrictionDefinition() { Actor = actor, RestrictionType = parsedType };
        }
    }

    public enum RestrictionTypeEnumeration
    {
        Allow=1, Restrict=2
    }
}
