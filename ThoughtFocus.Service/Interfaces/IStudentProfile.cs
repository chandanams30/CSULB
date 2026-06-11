using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentProfile
    {
        StudentProfileResponse GetStudentProfileData(string CsuldId, int UserID, int formID);
        StudentProfileSearchResponse GetStudentProfileSearchData(string searchString);
        StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId, string MessageBoardIdentifier);
        BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input);
        BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input);
        List<ApplicationList> GetApplications(int userId, string identifier);
        SemesterTermListResponse GetSemesterList(int applicationId);
        ApplicationProgramListResponse GetApplicationProgramList(int userID, int applicationTypeID, string termCode);
        StudentAppliedFormsByProgramsResponse GetStudentAppliedFormsByPrograms(int programID, string termCode, string CSULBID,string identifier);
        BaseResponse SaveStudentAggrement(SaveStudentAggrementRequest input);
        UpsertProfileAttachmentResponse UpsertProfileAttachment(UpsertProfileDocumentRequest input);
        DownloadProfileAttachmentResponse DownloadProfileAttachment(Guid UniqueID);
        ProfileAttachmentDetailsResponse GetProfileAttachmentDetails(string CSULBID);
        BaseResponse DeleteProfileAttachment(DeleteProfileAttachmentRequest input);
        BaseResponse SaveStudentProfilePersonalInfoData(SaveStudentProfilePersonalInfoDataRequest input);
        ProgramPlannerCourseListResponse GetProgramPlannerCourseList(string CSULBID, int ProgramID, string TermCode);
        BaseResponse UpsertCourseDetails(string JSONString);
        ProgramCheckListCourseResponse GetProgramChecklistCoursesTerm(string CSULBID, int ProgramID);

    }
}
