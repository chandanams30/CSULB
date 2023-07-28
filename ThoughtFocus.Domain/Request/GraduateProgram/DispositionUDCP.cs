using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class DispositionUDCP
    {
        public class BasicCredentials
        {
            public string label { get; set; }
            public string value { get; set; }
            public string comment { get; set; }
        }

        public IList<BasicCredentials> basicCredentials { get; set; }
    }
}
