using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkStandardsResponse:BaseResponse
    {
        public List<FieldWorkStandards> standards { get; set; }
    }
    public class FieldWorkStandards
    {
        public int StandardID { get; set; }
        public string standard { get; set; }
    }
}
