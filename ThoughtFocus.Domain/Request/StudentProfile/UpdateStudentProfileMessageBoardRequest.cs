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
    public class SaveStudentProfileDataRequest
    {
        public string CSULBID { get; set; }
        public DateTime ? DateOfBirth { get; set; }
        public string SSNNumber { get; set; }
        public string AcademicIntegrityStatement { get; set; }
        public DateTime ? SubmittedDate { get; set; }

    }

}
