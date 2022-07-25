using System;

namespace ThoughtFocus.Common.Exceptions
{
    public class DocumentRepositoryException: Exception
    {
        public DocumentRepositoryException() : base() { }

        public DocumentRepositoryException(string message) : base(message) { }

        public DocumentRepositoryException(string message, Exception innerException) : base(message, innerException) { }
        
    }
}
