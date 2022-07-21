using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkActivityLogRequest
    {
        public int UserId { get; set; }
        public int FieldWorkId { get; set; }
        public string ResponseSchema { get; set; }
    }
}
