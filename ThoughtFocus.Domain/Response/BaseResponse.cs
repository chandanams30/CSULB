using System;
using System.Collections.Generic;

namespace ThoughtFocus.Domain.Response
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Messages = new List<Message>();
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<Message> Messages { get; set; }
        public long ID { get; set; }
        public Guid DocumentID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid GroupID { get; set; }
        public bool IsValid { get; set; }
        public List<string> AdditionalMessages { get; set; }
        public string Type { get; set; }
        public List<string> ValidationErrors { get; set; }
        public long DocumentTypeID { get; set; } 
        public string FileName {get; set;}
        public string StorageKey {get; set;}

    }
    public class Message
    {
        public string Key { get; set; }
        public Dictionary<string, string> SubstitutionValues { get; set; }
    }
}
