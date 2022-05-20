using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Form
{
    public class FormAddRequest
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
        public int ProgramId { get; set; }
        public int SemesterId { get; set; }
        public object Form { get; set; }
        public int State { get; set; }
    }
}
