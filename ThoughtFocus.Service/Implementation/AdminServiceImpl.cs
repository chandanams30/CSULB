using CSULB_COE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.Admin;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Service.Interfaces;
using CSULB_COE.ViewModels;


namespace ThoughtFocus.Service.Implementation
{
    public class AdminServiceImpl :IAdminService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<AdminServiceImpl> _logger;
        private readonly ICommonUtils _utils;

        public AdminServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<AdminServiceImpl> logger
                                         , ICommonUtils utils
                                        )
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;

        }

        public BaseResponse AddUser(AddUserRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 255) { Value = input.CSULBID },
                                          new SqlParameter("@displayName", SqlDbType.NVarChar, 255) { Value = input.displayName },
                                          new SqlParameter("@mail", SqlDbType.NVarChar, 255) { Value = input.mail },
                                          new SqlParameter("@LastName", SqlDbType.NVarChar, 255) { Value = input.LastName },
                                          new SqlParameter("@FirstName", SqlDbType.NVarChar, 255) { Value = input.FirstName },
                                          new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = input.RoleID },
                                          new SqlParameter("@AuthenticationTypeId", SqlDbType.Int) { Value = input.AuthenticationTypeId },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }

                                        };

            DataTable dtResponse = _helper.GetDataTable("[User].[AddUser]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                int status = Convert.ToInt32(dtResponse.Rows[0]["Status"]);
                if (status == 1)
                {
                    response.Message = "Data Updated Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    string message = Convert.ToString(dtResponse.Rows[0]["Message"]);
                    response.Message = message;
                    response.IsSuccess = false;
                }


            }
            return response;
        }

        public BaseResponse AssignUserToRole(AssignUserToRoleRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = input.RoleID },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID },
                                        };

            int ID = _helper.InsertTable("[User].[AssignUserToRole]", parameters);
            response.Message = "Role Assigned Successfully";
            response.IsSuccess = true;
            return response;
        }

        public RolesListResponse GetRolesList()
        {
            RolesListResponse obj = new RolesListResponse();
            SqlParameter[] parameters = {
                                        };

            DataTable dtRoles = _helper.GetDataTable("[User].[GetRolesList]", parameters);
            try
            {
                if (dtRoles.Rows.Count > 0)
                {


                    obj.roles = dtRoles.AsEnumerable().Select(row =>
                                              new RolesList
                                              {
                                                  RoleID = Convert.ToInt32(row["RoleID"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        //public UserDetailResponse GetUser(int UserID)
        //{
        //    UserDetailResponse obj = new UserDetailResponse();

        //    SqlParameter[] parameters = {
        //                                    new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID }
        //                                };

        //    DataTable dtOptionsList = _helper.GetDataTable("[User].[GetUser]", parameters);
        //    try
        //    {
        //        if (dtOptionsList.Rows.Count > 0)
        //        {


        //            obj.UserDetail = dtOptionsList.AsEnumerable().Select(row =>
        //                                      new UserDetails
        //                                      {
        //                                          UserDetail = Convert.ToString(row["UserDetail"])
        //                                      }).FirstOrDefault();


        //            obj.IsSuccess = true;
        //            obj.Message = "Data Retrieved Successfully";

        //        }
        //        else
        //        {
        //            obj.IsSuccess = false;
        //            obj.Message = "No Data Present";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        obj.IsSuccess = false;
        //        obj.Message = "Data Retrieval Failed , Please contact site admin ";
        //        obj.StackTrace = ex.Message;
        //    }
        //    return obj;
        //}


        public UserDetailResponse GetUser(int UserID)
        {
            UserDetailResponse obj = new UserDetailResponse();
            var getADRoles = GetIntegratedUserRolesByUserID(UserID);

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@RoleIDList", SqlDbType.NVarChar,255) { Value = getADRoles.RolesList }
                                        };

            DataTable dtOptionsList = _helper.GetDataTable("[User].[GetUser_AD]", parameters);

            try
            {
                if (dtOptionsList.Rows.Count > 0)
                {


                    obj.UserDetail = dtOptionsList.AsEnumerable().Select(row =>
                                              new UserDetails
                                              {
                                                  UserDetail = Convert.ToString(row["UserDetail"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public CommunityDistrictListResponse GetCommunityDistrictList()
        {
            CommunityDistrictListResponse obj = new CommunityDistrictListResponse();
            SqlParameter[] parameters = {
                                            
                                        };

            DataTable dtDistrictList = _helper.GetDataTable("[FieldWork].[GetCommunityDistrictList]", parameters);
            try
            {
                if (dtDistrictList.Rows.Count > 0)
                {


                    obj.Districts = dtDistrictList.AsEnumerable().Select(row =>
                                              new CommunityDistrictList
                                              {
                                                  Districts = Convert.ToString(row["CommnunityDistrictList"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public CommunitySchoolListResponse GetCommunitySchoolList()
        {
            CommunitySchoolListResponse obj = new CommunitySchoolListResponse();
            SqlParameter[] parameters = {
                                           
                                        };

            DataTable dtSchoolList = _helper.GetDataTable("[FieldWork].[GetCommunitySchoolList]", parameters);
            try
            {
                if (dtSchoolList.Rows.Count > 0)
                {


                    obj.Schools = dtSchoolList.AsEnumerable().Select(row =>
                                              new CommunitySchoolList
                                              {
                                                  Schools = Convert.ToString(row["CommunitySchoolList"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public DownloadMessageBoardAttachment DownloadMessageBoardAttachment(string fileName)
        {
            DownloadMessageBoardAttachment obj = new DownloadMessageBoardAttachment();
            byte[] content= _utils.GetFileContent(_configuration["ApplicationKeys:MessageBoardFileRepository"].ToString(), "MessageBoardAttachments", fileName);
            obj.FileContent = content;
            return obj;
        }

        public UserDataOptionListResponse GetUserOptionList(int UserID)
        {
            UserDataOptionListResponse obj = new UserDataOptionListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                        };

            DataTable dtOptionsList = _helper.GetDataTable("[User].[GetUserDataOptionList]", parameters);
            try
            {
                if (dtOptionsList.Rows.Count > 0)
                {


                    obj.UserDataOptionList = dtOptionsList.AsEnumerable().Select(row =>
                                              new UserDataOptions
                                              {
                                                  UserDataOptionList = Convert.ToString(row["UserDataOptionList"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public UserListResponse GetUsersList(string searchString,int RoleID)
        {
            UserListResponse obj = new UserListResponse();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@searchString", SqlDbType.NVarChar, 100) { Value = searchString },
            };
            //SqlParameter[] parameters = {
            //                                new SqlParameter("@searchString", SqlDbType.NVarChar, 100) { Value = searchString },
            //                                new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = RoleID }
            //                            };
            if (RoleID > 0)
            {
                parameters.Add(new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = RoleID });
            }

            DataTable dtUsers = _helper.GetDataTable("[User].[GetUsersList]", parameters.ToArray());
            try
            {
                if (dtUsers.Rows.Count > 0)
                {


                    obj.userList = dtUsers.AsEnumerable().Select(row =>
                                              new UserList
                                              {
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  EMAIL = Convert.ToString(row["EMAIL"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  UserRoles = Convert.ToString(row["UserRoles"])
                                                  //ProgramName = Convert.ToString(row["ProgramName"] == DBNull.Value ? null : row["ProgramName"])
                                              }).ToList();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public BaseResponse UpdateUser(UpdateUserRequest input)
        {
            DataTable Roles = _utils.ToDataTable(input.Roles);
            string CSULBID = string.Empty;
            CSULBID = String.IsNullOrEmpty(input.CSULBID) ? "" : input.CSULBID; 
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FirstName", SqlDbType.NVarChar, 255) { Value = input.FirstName },
                                          new SqlParameter("@LastName", SqlDbType.NVarChar, 255) { Value = input.LastName },
                                          new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = input.Email },
                                          new SqlParameter("@AuthenticationTypeId", SqlDbType.Int) { Value = input.AuthenticationTypeId },
                                          new SqlParameter("@Status", SqlDbType.Bit) { Value = input.Status },
                                          new SqlParameter("@CSULBID", SqlDbType.VarChar, 15) { Value = CSULBID },
                                          new SqlParameter("@FirstNamePref", SqlDbType.NVarChar, 255) { Value = input.FirstNamePref },
                                          new SqlParameter("@LastNamePref", SqlDbType.NVarChar, 255) { Value = input.LastNamePref },
                                          new SqlParameter("@DisplayName", SqlDbType.NVarChar, 255) { Value = input.DisplayName },
                                          new SqlParameter("@Roles", SqlDbType.Structured) { Value = Roles },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }

                                        };

            DataTable dtResponse = _helper.GetDataTable("[User].[UpdateUser]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                int status = Convert.ToInt32(dtResponse.Rows[0]["Status"]);
                if (status == 1)
                {
                    response.Message = "Data Updated Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    string message= Convert.ToString(dtResponse.Rows[0]["Message"]);
                    response.Message = message;
                    response.IsSuccess = false;
                }
               
                
            }

            return response;

        }

        public BaseResponse AssignUsersToProgram(AssignUsersToProgramRequest input)
        {
            DataTable UserPrograms = _utils.ToDataTable(input.UserPrograms);
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = input.RoleID },
                                          new SqlParameter("@UserPrograms", SqlDbType.Structured) { Value = UserPrograms },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }

                                        };

            DataTable dtResponse = _helper.GetDataTable("[User].[AssignUserToProgram]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                int status = Convert.ToInt32(dtResponse.Rows[0]["Status"]);
                if (status == 1)
                {
                    response.Message = "Users Updated Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    string message = Convert.ToString(dtResponse.Rows[0]["Message"]);
                    response.Message = message;
                    response.IsSuccess = false;
                }


            }

            return response;

        }

        public BaseResponse UploadMessageBoardAttachment(UploadMessageBoardAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string attachmentFolderName = string.Empty;
                string fileSavePath = string.Empty;
                var fileRepoPath = _configuration["ApplicationKeys:MessageBoardFileRepository"];
                if (input.FileName != string.Empty)
                {
                    ThoughtFocus.Common.Utilities.Implementation.AttachmentFileDetails fileDetails = _utils.GetAttachedFileSplitValues(input.FileName);
                    fileExtension = fileDetails.FileExtension;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // logic to convert png to pdf 
                        byte[] imageContent = null;
                        imageContent = _utils.GetImageFilecontent(input.FileContent);
                        input.FileContent = null;
                        input.FileContent = imageContent;
                        fileExtension = "pdf";
                    }
                    fileSavePath= Path.Combine(fileRepoPath, "MessageBoardAttachments");
                    if (Directory.Exists(fileSavePath))
                    {
                        File.WriteAllBytes(Path.Combine(fileSavePath, fileDetails.FileName + "." + fileExtension), input.FileContent);
                    }
                    else
                    {
                        Directory.CreateDirectory(fileSavePath);
                        File.WriteAllBytes(Path.Combine(fileSavePath, fileDetails.FileName + "." + fileExtension), input.FileContent);
                    }
                    response.IsSuccess = true;
                    response.Message = "Attachment Added Successfully";
                }
             }
            return response;
        }

        public GetUsersByProgramRoleResponse GetUsersByProgramRole(int ProgramID, int RoleID)
        {
            GetUsersByProgramRoleResponse obj = new GetUsersByProgramRoleResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                            new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = RoleID }
                                        };

            DataTable dtProgramDetails = _helper.GetDataTable("[User].[GetUsersByProgramRole]", parameters);
            try
            {
                if (dtProgramDetails.Rows.Count > 0)
                {


                    obj.Details = dtProgramDetails.AsEnumerable().Select(row =>
                                              new GetUsersByProgramRole
                                              {
                                                  UsersByProgramRole = Convert.ToString(row["UsersByProgramRole"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }
        public BaseResponse UpsertCommunityDistrict(UpsertCommunityDistrictRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CommunityDistrictID", SqlDbType.BigInt) { Value = input.CommunityDistrictID },
                                          new SqlParameter("@CommunityDistrictName", SqlDbType.VarChar,100) { Value = input.CommunityDistrictName },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }

                                        };

            int id = _helper.InsertTable("[FieldWork].[UpsertCommunityDistrict]", parameters);
            response.Message = "District Updated Successfully";
            response.IsSuccess = true;

            return response;

        }
        public BaseResponse UpsertCommunitySchool(UpsertCommunitySchoolRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CommunitySchoolID", SqlDbType.BigInt) { Value = input.CommunitySchoolID },
                                          new SqlParameter("@CommunitySchoolName", SqlDbType.VarChar,100) { Value = input.CommunitySchoolName },
                                          new SqlParameter("@CommunityDistrictID", SqlDbType.BigInt) { Value = input.CommunityDistrictID },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }

                                        };

            int id = _helper.InsertTable("[FieldWork].[UpsertCommunitySchool]", parameters);
            response.Message = "School Updated Successfully";
            response.IsSuccess = true;

            return response;

        }


        public UpcomingSemesterListResponse GetFutureSemesterList()
        {
            UpcomingSemesterListResponse obj = new UpcomingSemesterListResponse();


            SqlParameter[] parameters = { };

            DataTable dtSemesters = _helper.GetDataTable("[dbo].[GetFutureSemesterList]", parameters);
            try
            {
                if (dtSemesters.Rows.Count > 0)
                {


                    obj.SemesterTerms = dtSemesters.AsEnumerable().Select(row =>
                                              new Domain.Response.Admin.SemesterTerm
                                              {
                                                  TermName = Convert.ToString(row["Name"]),
                                                  TermCode = Convert.ToString(row["TermCode"])
                                              }).ToList();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public ApplicationProgramListResponse GetAllApplicationProgramsList(int applicationTypeID, string termCode)
        {
            ApplicationProgramListResponse obj = new ApplicationProgramListResponse();

            SqlParameter[] parameters ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationTypeID },
                                            new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = termCode },

                                       };
            DataSet dtApplicationPrograms = _helper.GetDataSet("[dbo].[GetProgramsByApplication]", parameters);

            try
            {
                if (dtApplicationPrograms.Tables.Count > 0)
                {
                    obj.ApplicationProgramList = dtApplicationPrograms.Tables[0].AsEnumerable().Select(row =>
                                                 new ApplicationProgramsList
                                                 {
                                                     ProgramID = Convert.ToInt32(row["ID"]),
                                                     ProgramName = Convert.ToString(row["Name"]),
                                                 }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }
        public ApplicationDatesResponse GetApplicationDates(string termCode)
        {
            ApplicationDatesResponse obj = new ApplicationDatesResponse();

            SqlParameter[] parameters ={
                                            new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = termCode }

                                       };
            DataSet dtApplicationPrograms = _helper.GetDataSet("[dbo].[GetApplicationDates]", parameters);

            try
            {
                if (dtApplicationPrograms.Tables.Count > 0)
                {
                    obj = dtApplicationPrograms.Tables[0].AsEnumerable().Select(row =>
                                                 new ApplicationDatesResponse
                                                 {
                                                     ApplicationOpenDate = Convert.ToDateTime(row["ApplicationOpenDate"]),
                                                     ApplicationCloseDate = Convert.ToDateTime(row["ApplicationCloseDate"]),
                                                     ApplicationDeadlineDate = Convert.ToDateTime(row["ApplicationDeadlineDate"]),
                                                 }).FirstOrDefault();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public BaseResponse UpdateProgramApplicationDates(UpdateProgramApplicationDates input)
        {
            BaseResponse response = new BaseResponse();

            foreach (var programID in input.ProgramID)
            {
                SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ProgramID", SqlDbType.Int) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = input.TermCode },
                                          new SqlParameter("@ApplicationOpens", SqlDbType.DateTime) {Value = input.ApplicationOpens},
                                          new SqlParameter("@ApplicationDeadline", SqlDbType.DateTime) {Value = input.ApplicationDeadline},
                                          new SqlParameter("@ApplicationCloseDate", SqlDbType.DateTime) {Value = input.ApplicationCloseDate},
                                          new SqlParameter("@Status", SqlDbType.Bit) {Value = input.Status}
                                        };
                int ID = _helper.InsertTable("[dbo].[SaveProgramApplicationDates]", parameters);

            }
            try
            {
                response.Message = "Program application dates saved successfully";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseResponse RemoveReviewerFromForms(ReviewerRequest input)
        {
            DataTable UserPrograms = _utils.ToDataTable(input.UserPrograms);
            BaseResponse response = new BaseResponse();

            for (int i = 0; i < UserPrograms.Rows.Count; i++)
            {
                SqlParameter[] parameters =
                                  {
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@ReviewerID", SqlDbType.BigInt) { Value = UserPrograms.Rows[i]["UserID"] }
                                        };
                int ID = _helper.InsertTable("[dbo].[RemoveReviewer]", parameters);
            }
            response.Message = "Reviewer Removed Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse UpsertInternCourseConfig(UpsertCSULBIDForIntern input)
        {
            BaseResponse response = new BaseResponse();
            string filePath = Path.Combine("SupportFiles/MycedConfigurations/CSULBCEDConfig.json");
            JObject jsonObj;

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                jsonObj = JObject.Parse(File.ReadAllText(filePath)); 

                // Get current value and split into list
                string currentValue = jsonObj["EnableIntern2"]?.ToString();
                var existingIds = string.IsNullOrEmpty(currentValue)
                    ? new List<string>()
                    : currentValue.Split(',')
                                  .Select(id => id.Trim())
                                  .Where(id => !string.IsNullOrEmpty(id))
                                  .ToList();

                // Split input into multiple IDs
                var newIds = input.CSULBIDs.Split(',')
                                  .Select(id => id.Trim())
                                  .Where(id => !string.IsNullOrEmpty(id))
                                  .ToList();

                // Add new IDs if not already present
                foreach (var id in newIds)
                {
                    if (!existingIds.Contains(id))
                    {
                        existingIds.Add(id);
                    }
                }

                jsonObj["EnableIntern2"] = string.Join(",", existingIds);

                response.Message = "ID(s) Added Successfully";
                response.IsSuccess = true;

                File.WriteAllText(filePath, jsonObj.ToString());
            }
            else
            {
                response.Message = "Config file not found.";
                response.IsSuccess = false;
            }

            return response;
        }

        public RoleADResponse GetIntegratedUserRolesByUserID(int UserID)
        {
            RoleADResponse obj = new RoleADResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.NVarChar, 100) { Value = UserID }
                                        };

            DataTable dtRolesList = _helper.GetDataTable("[dbo].[GetIntegratedUserRolesByUserID]", parameters);
            try
            {
                if (dtRolesList.Rows.Count > 0)
                {


                    obj.RolesList = dtRolesList.AsEnumerable().Select(row =>
                                              new Roles
                                              {
                                                  RoleId = Convert.ToInt16(row["RoleID"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();
                    obj.RoleATIDList = dtRolesList.AsEnumerable().Select(row =>
                                              new RoleATID
                                              {
                                                  RoleId = Convert.ToInt16(row["RoleID"]),
                                                  ApplicationTypeId = Convert.ToInt16(row["ApplicationTypeId"])
                                              }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }



    }
}
