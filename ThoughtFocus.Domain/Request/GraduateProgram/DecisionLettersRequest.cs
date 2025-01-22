using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class DecisionLettersRequest
    {
        public int DecisionLettersID { get; set; }
        public string ProgramIdentifier { get; set; }
        public string OfferedCategories { get; set; }
        public string DecisionType { get; set; }
        public string MailBody { get; set; }
        public int UserID { get; set; }
    }
}
