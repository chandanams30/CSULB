using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Fault;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ThoughtFocus.Common.Workflow.Core.Model
{

    [Table("WorkflowDefinition", Schema = "WorkFlow")]
    public  class WorkflowDefinition : BaseDefinition
    {
        public string DesignerModel { get; set; }
        public virtual ICollection<ActorDefinitionExecuteRule> ActorDefinitionExecuteRules { get; set; }
        public virtual ICollection<ActorDefinitionIsIdentity> ActorDefinitionIsIdentities { get; set; }
        public virtual ICollection<ActorDefinitionIsInRole> ActorDefinitionIsInRoles { get; set; }        
        public virtual ICollection<ParameterDefinition> Parameters { get; set; }
        public virtual ICollection<CommandDefinition> Commands { get; set; }
        public virtual ICollection<ActionDefinition> Actions { get; set; }
        public virtual ICollection<ActivityDefinition> Activities { get; set; }
        public virtual ICollection<TransitionDefinition> Transitions { get; set; }
        public virtual ICollection<LocalizeDefinition> Localization { get; set; }
        

        public WorkflowDefinition()
        {
            ActorDefinitionExecuteRules = new List<ActorDefinitionExecuteRule>();
            ActorDefinitionIsIdentities = new List<ActorDefinitionIsIdentity>();
            ActorDefinitionIsInRoles = new List<ActorDefinitionIsInRole>();
          
            Parameters = new List<ParameterDefinition>();
            Commands = new List<CommandDefinition>();
            Actions = new List<ActionDefinition>();
            Activities = new List<ActivityDefinition>();
            Transitions = new List<TransitionDefinition>();
            Localization = new List<LocalizeDefinition>();
            DesignerModel = string.Empty;
        }

        public ActivityDefinition InitialActivity
        {
            get
            {
                try
                {
                    var initialActivity = Activities.SingleOrDefault(a => a.IsInitial);
                    if (initialActivity == null)
                        throw new InitialActivityNotFoundException();
                    return initialActivity;
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            }
        }
      [NotMapped]
        public ParameterDefinition[] ParametersForSerialized
        {
            get
            {
                if (Parameters == null)
                    return null;

                return Parameters.Where(p => DefaultDefinitions.DefaultParameters.Count(p1 => p1.Name == p.Name) == 0).ToArray();
            }
        }            

        public ActivityDefinition FindActivity (string name)
        {
            var activity = Activities.SingleOrDefault(a => a.Name == name);
            if (activity == null)
                throw new ActivityNotFoundException();
            return activity;
        }

        public TransitionDefinition FindTransition(string name)
        {
            var transition = Transitions.SingleOrDefault(a => a.Name == name);
            if (transition == null)
                throw new TransitionNotFoundException();
            return transition;
        }

        public IEnumerable<TransitionDefinition> GetPossibleTransitionsForActivity (ActivityDefinition activity)
        {
            return Transitions.Where(t => t.From == activity);
        }

        public IEnumerable<TransitionDefinition> GetCommandTransitions(ActivityDefinition activity)
        {
            return Transitions.Where(t => t.From == activity && t.Trigger.Type == TriggerTypeEnumeration.Command);
        }

        public IEnumerable<TransitionDefinition> GetAutoTransitionForActivity(ActivityDefinition activity)
        {
            return Transitions.Where(t => t.From == activity && t.Trigger.Type == TriggerTypeEnumeration.Auto);
        }

        public IEnumerable<TransitionDefinition> GetCommandTransitionForActivity(ActivityDefinition activity, string commandName)
        {
            return Transitions.Where(t => t.From == activity && t.Trigger.Type == TriggerTypeEnumeration.Command && t.Trigger.Command.Name == commandName);
        }

        public IEnumerable<TransitionDefinition> GetTimerTransitionForActivity(ActivityDefinition activity)
        {
            return Transitions.Where(t => t.From == activity && t.Trigger.Type == TriggerTypeEnumeration.Timer);
        }


        public static WorkflowDefinition Create(string name, List<ActorDefinition> actors, List<ParameterDefinition> parameters, List<CommandDefinition> commands,
           List<ActionDefinition> actions, List<ActivityDefinition> activities, List<TransitionDefinition> transitions, List<LocalizeDefinition> localization, string designerModel)
        {
            return new WorkflowDefinition
                       {
                           Actions = actions,
                           Activities = activities,
                           //Actors = actors,
                           Commands = commands,
                           Name = name,
                           Parameters = parameters,
                           Transitions = transitions,
                           Localization = localization,
                           DesignerModel = designerModel
                       };
        }

        public ParameterDefinition GetParameterDefinition(string name)
        {
            return Parameters.Single(p => p.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase));
        }

        public ParameterDefinition GetNullableParameterDefinition(string name)
        {
            return Parameters.SingleOrDefault(p => p.Name == name);
        }
        [NotMapped]
        public IEnumerable<ParameterDefinition> PersistenceParameters
        {
            get { return Parameters.Where(p => p.Purpose == ParameterPurposeEnumeration.Persistence);}
        }

        #region Localized
        public string GetLocalizedStateName(string stateName, CultureInfo culture)
        {
            return GetLocalizedName(stateName, culture, LocalizeTypeEnumeration.State);
        }

        public string GetLocalizedCommandName(string commandName, CultureInfo culture)
        {
            return GetLocalizedName(commandName, culture, LocalizeTypeEnumeration.Command);
        }


        protected string GetLocalizedName(string name, CultureInfo culture, LocalizeTypeEnumeration localizeType)
        {
            if (Localization == null)
                return name;                     

            var localize =
                Localization.FirstOrDefault(
                    l =>
                    l.LocalizeType.Id == (int)localizeType && string.Compare(l.Culture, culture.Name, true) == 0 &&
                    l.ObjectName == name);

            if (localize != null)
                return localize.Value;

            localize =
                Localization.FirstOrDefault(
                    l =>
                    l.LocalizeType == localizeType && l.IsDefault &&
                    l.ObjectName == name);

            if (localize != null)
                return localize.Value;

            return name;
        }
        #endregion
       
    }
}
