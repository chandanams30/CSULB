using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{

    [Serializable]
    [DataContract]
    public sealed class WorkflowCommand //: IEqualityComparer<WorkflowCommand>
    {
        [Serializable]
        [DataContract]
        public class CommandParameter
        {
            [DataMember]
            public string Name { get;  set; }

            [DataMember]
            public object Value { get; set; }

            [DataMember]
            public Type Type { get { return Type.GetType(TypeAsString); } internal set { } }

            [DataMember]
            public string TypeAsString { get; set; }
        }

        [DataMember]
        public long ProcessId { get;  set; }
        [DataMember]
        public long WorkFlowId { get; set; }

        [DataMember]
        public string ValidForActivityName { get; private set; }

        [DataMember]
        public string ValidForStateName { get; private set; }

        [DataMember]
        public long CommandID { get; private set; }

        [DataMember]
        public string CommandIconClass { get; private set; }

        [DataMember]
        public string TransitionType { get; private set; }

        [DataMember]
        public List<TransitionValidationDefination> TransitionValidationDefinations { get; private set; }

        //public IEnumerable<Guid> Identities
        //{
        //    get { return IdentitiesList; }
        //}

        // private  List<Guid> IdentitiesList = new List<Guid>();

        public IEnumerable<long> Identities
        {
            get { return IdentitiesList; }
        }

        private List<long> IdentitiesList = new List<long>();


        [DataMember]
        public string CommandName { get;  set; }

        [DataMember]
        public string LocalizedName { get; set; }

        [DataMember] public IEnumerable<CommandParameter> Parameters = new List<CommandParameter>();

        public object GetParameter(string name)
        {
            var parameter = Parameters.SingleOrDefault(p => p.Name == name);
            if (parameter == null)
                return null;
            return parameter.Value;
        }

        public void SetParameter(string name, object value)
        {
            var parameter = Parameters.SingleOrDefault(p => p.Name == name);
            if (parameter == null)
                throw new InvalidOperationException();
            if (parameter.Type != value.GetType())
                throw new InvalidOperationException();
            parameter.Value = value;
        }

        public static WorkflowCommand Create(long processId,long workflowId ,TransitionDefinition transitionDefinition)
        {
            if (transitionDefinition.Trigger.Type != TriggerTypeEnumeration.Command || transitionDefinition.Trigger.Command == null)
                throw new InvalidOperationException();

            var parametrs = new List<CommandParameter>();
            //var parametrs = new List<CommandParameter>(transitionDefinition.Trigger.Command.InputParameters.Count);
            if (transitionDefinition.Trigger.Command.InputParameters != null)
            {
                parametrs.AddRange(
                    transitionDefinition.Trigger.Command.InputParameters.Select(
                        p => new CommandParameter { Name = p.Key, Type = p.Value.Type, Value = null }));
            }

            return new WorkflowCommand
                       {
                           CommandName = transitionDefinition.Trigger.Command.Name,
                           Parameters = parametrs,
                           ProcessId = processId,
                           WorkFlowId=workflowId,
                           ValidForActivityName = transitionDefinition.From.Name,
                           ValidForStateName = transitionDefinition.From.State,
                           CommandIconClass=transitionDefinition.Trigger.Command.CommandIconClass,
                           CommandID = transitionDefinition.Trigger.Command.ID,
                           TransitionType=transitionDefinition.TransitionClassifier.Name,
                           TransitionValidationDefinations = transitionDefinition.TransitionValidationDefinations == null ? null : transitionDefinition.TransitionValidationDefinations.ToList()
                       };
        }

        public bool Validate ()
        {
            return Parameters.All(parameter => parameter.Value != null);
        }

        public void AddIdentity (long identityId)
        {
            if (!IdentitiesList.Contains(identityId))
                IdentitiesList.Add(identityId);
        }

        //public bool Equals(WorkflowCommand x, WorkflowCommand y)
        //{
        //    if (x == null && y == null)
        //        return true;

        //    if (x == null || y == null)
        //        return false;

        //    return x.CommandName == y.CommandName;
        //}

        //public int GetHashCode(WorkflowCommand obj)
        //{
        //    int hCode = bx.Height ^ bx.Length ^ bx.Width;
        //    return hCode.GetHashCode();
        //}
    }
}
