using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ActionDefinition", Schema = "WorkFlow")]
    public class ActionDefinition : BaseDefinition
    {


        public string TypeAsString { get; set; }
        public virtual string FullTypeName { get; set; }
        public virtual string MethodName { get; set; }
        public long WorkflowDefinitionID { get; set; }


        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

        [NotMapped]
        public virtual Type Type { get { return Type.GetType(TypeAsString); } internal set { } }
        public ICollection<ParameterDefinitionForAction> InputParametersLazyLoading;
        public virtual ICollection<ParameterDefinitionForAction> ParameterDefinitionForActions { get; set; }

        [BackingField(nameof(InputParametersLazyLoading))]
        public virtual ICollection<ParameterDefinitionForAction> ParameterDefinitionForActionsLazy { get; set; }

        protected List<ParameterDefinitionForAction> OutputParametersList;
        
        protected List<ParameterDefinitionForAction> InputParametersList;

        public virtual IEnumerable<ParameterDefinitionForAction> InputParameters
        {
            get
            {
                if (ParameterDefinitionForActions != null)
                {
                    return ParameterDefinitionForActions.Where(a => a.IsInputParameter == true).ToList();
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
       
        public virtual IEnumerable<ParameterDefinitionForAction> OutputParameters
        {
            get
            {
                if (ParameterDefinitionForActions != null)
                {
                    return ParameterDefinitionForActions.Where(a => a.IsInputParameter == false).ToList();
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
        


        public static ActionDefinition Create(string name, string type, string metodName)
        {
            Type t = null;
            try
            {
                t = Type.GetType(type);
            }
            catch (Exception ex) { }

            return new ActionDefinition
            {
                Name = name,
                InputParametersList = new List<ParameterDefinitionForAction>(),
                OutputParametersList = new List<ParameterDefinitionForAction>(),
                Type = t,
                FullTypeName = type,
                MethodName = metodName
            };
        }

        public static ActionDefinitionForActivity Create(ActionDefinition actionDefinition, string order)
        {
            var parsedOrder = int.Parse(order);
            return new ActionDefinitionForActivity { ActionDefinition = actionDefinition, Order = parsedOrder };
        }
 
        public static ActionDefinitionForActivity Create(ActionDefinition actionDefinition, int order)
        {
            return new ActionDefinitionForActivity { ActionDefinition = actionDefinition, Order = order };
        }

        public void AddInputParameterRef(ParameterDefinitionForAction parameter)
        {
            InputParametersList.Add(parameter);
        }

        public void AddOutputParameterRef(ParameterDefinitionForAction parameter)
        {
            OutputParametersList.Add(parameter);
        }

        [NotMapped]
        public static ActionDefinition NoAction = new ActionDefinition();
    }

}
