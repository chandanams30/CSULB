using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{

    public abstract class ActorDefinition : BaseDefinition
    {

        public static ActorDefinition CreateRule(string name, string ruleName)
        {
            return new ActorDefinitionExecuteRule { Name = name, RuleName = ruleName, ParametersDictionary = new Dictionary<string, string>() };
        }

        public static ActorDefinition CreateIsIdentity(string name, string identityId)
        {
            return new ActorDefinitionIsIdentity() { Name = name, IdentityId = new Guid(identityId) };
        }

        public static ActorDefinition CreateIsInRole(string name, string roleId)
        {
            return new ActorDefinitionIsInRole() { Name = name, RoleId = roleId };
        }

        public virtual void AddParameter(string key, string value) { }



    }

    [Table("ActorDefinitionExecuteRule", Schema = "WorkFlow")]
    public class ActorDefinitionExecuteRule : ActorDefinition
    {
        public string RuleName { get; internal set; }


        public IDictionary<string, string> Parameters { get { return ParametersDictionary; } }

        internal Dictionary<string, string> ParametersDictionary { get; set; }

        public override void AddParameter(string key, string value)
        {
            ParametersDictionary.Add(key, value);
        }


        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }


        public long WorkflowDefinitionID { get; set; }

    }

    [Table("ActorDefinitionIsIdentity", Schema = "WorkFlow")]
    public class ActorDefinitionIsIdentity : ActorDefinition
    {

        public Guid IdentityId { get; set; }

        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

        public long WorkflowDefinitionID { get; set; }

    }

    [Table("ActorDefinitionIsInRole", Schema = "WorkFlow")]
    public class ActorDefinitionIsInRole : ActorDefinition
    {

        public string RoleId { get; set; }

        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

        public long WorkflowDefinitionID { get; set; }

        public override string Name { get; set; }   

    }
}
