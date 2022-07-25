using System;

namespace ThoughtFocus.Common.Exceptions
{
    public class ObjectNullException : Exception 
    {
        public ObjectNullException(string Message): base(Message) {}  
        public ObjectNullException() {}
        public ObjectNullException(string Message, Exception inner) : base(Message, inner) { }
    }
}
