using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkActivityLogResponse:BaseResponse
    {
        public int FieldWorkId { get; set; }
        public string BaseSchema { get; set; }
        public string ResponseSchema { get; set; }
    }
}
