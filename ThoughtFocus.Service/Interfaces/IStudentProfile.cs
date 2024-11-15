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
        BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input);
        BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input);
        List<ApplicationList> GetApplications(int userId, string identifier);
        SemesterTermListResponse GetSemesterList(int applicationId);
        ApplicationProgramListResponse GetApplicationProgramList(int userID, int applicationTypeID, string termCode);
        StudentAppliedFormsByProgramsResponse GetStudentAppliedFormsByPrograms(int programID, string termCode, string CSULBID);
    }
}
