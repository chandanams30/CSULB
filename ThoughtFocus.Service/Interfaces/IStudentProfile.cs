using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentProfile
    {
        StudentProfileResponse GetStudentProfileData(string CsuldId);
        StudentProfileSearchResponse GetStudentProfileSearchData(string searchString);
        StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId, string MessageBoardIdentifier);
        BaseResponse UpdateFormStudentMessageBoard(UpdateFormStudentMessageBoard input);
    }
}
