using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class FormStatesResponse:BaseResponse
    {
        public List<FormStates> FormStates { get; set; }
    }
    public class FormStates
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
