using System;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    public class WorkflowDefinition<T> where T : class 
    {
        public T Workflow { get; private set; }
        public Guid Id { get; private set; }
        public bool  IsObsolete { get; private set; }
        public bool IsDeterminingParametersChanged { get; set; }

        public WorkflowDefinition(Guid id, T scheme, bool isObsolete, bool isDeterminingParametersChanged)
        {
            Id = id;
            Workflow = scheme;
            IsObsolete = isObsolete;
            IsDeterminingParametersChanged = isDeterminingParametersChanged;
        }

        public WorkflowDefinition(Guid id, T scheme, bool isObsolete) : this (id,scheme,isObsolete,false)
        {
        }

    }
}
