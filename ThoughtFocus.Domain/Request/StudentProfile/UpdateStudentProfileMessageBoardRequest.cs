using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.StudentProfile
{
    public class UpdateStudentProfileMessageBoardRequest
    {
        public string CSULBID { get; set; }
        public string MessageBoardIdentifier { get; set; }
        public string MessageBoard { get; set; }

    }

}
