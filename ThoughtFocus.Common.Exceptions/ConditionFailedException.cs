using System;

namespace ThoughtFocus.Common.Exceptions
{
    public class ConditionFailedException : Exception 
    {
        public ConditionFailedException(string Message): base(Message) {}  
        public ConditionFailedException() {}
        public ConditionFailedException(string Message, Exception inner) : base(Message, inner) { }
    }
}
