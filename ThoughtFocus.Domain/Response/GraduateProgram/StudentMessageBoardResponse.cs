using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class StudentMessageBoardResponse:BaseResponse
    {
        public int FormID { get; set; }
        public int UserID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string StudentMessageBoard { get; set; }
    }
}
