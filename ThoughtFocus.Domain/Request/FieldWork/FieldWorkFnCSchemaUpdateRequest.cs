using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkFnCSchemaUpdateRequest
    {
        public int userID { get; set; }
        public int fieldWorkID { get; set; }
        public int schemaTypeID { get; set; }
        public string schema { get; set; }
    }
}
