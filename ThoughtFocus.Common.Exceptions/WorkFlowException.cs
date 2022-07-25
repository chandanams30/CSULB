using System;

namespace ThoughtFocus.Common.Exceptions
{
    public class WorkFlowException: Exception
    {
        public WorkFlowException() : base() { }

        public WorkFlowException(string message) : base(message) { }

        public WorkFlowException(string message, Exception innerException) : base(message, innerException) { }
    }
}
