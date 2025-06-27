using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentProfileController : ControllerBase
    {
        public ILogger<StudentProfileController> _logger;
        public IStudentProfile _studentProfileService;
        private readonly IConfiguration _configuration;
        public StudentProfileController(IStudentProfile studentProfileService,
              ILogger<StudentProfileController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _studentProfileService = studentProfileService;
            _configuration = configuration;
        }
        //feches student profile data
        [HttpGet("GetStudentProfileData")]
        public StudentProfileResponse GetStudentProfileData(string CsulbId)
        {
            try
            {
                StudentProfileResponse response = _studentProfileService.GetStudentProfileData(CsulbId);
                return response;
            }
            catch (Exception ex)
            {
                StudentProfileResponse response = new StudentProfileResponse();
                response.IsSuccess = false;
                response.Message = "Failed to fetch student profile data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        //Student profile search
        [HttpGet("GetStudentProfileSearchData")]
        public StudentProfileSearchResponse GetStudentProfileSearchData(string searchString)
        {
            try
            {
                StudentProfileSearchResponse response = _studentProfileService.GetStudentProfileSearchData(searchString);
                return response;
            }
            catch (Exception ex)
            {
                StudentProfileSearchResponse response = new StudentProfileSearchResponse();
                response.IsSuccess = false;
                response.Message = "Failed to search profile data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetStudentProfileMessageBoard")]
        public StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId,string MessageBoardIdentifier)
        {
            try
            {
                StudentProfileMessageBoardResponse response = _studentProfileService.GetStudentProfileMessageBoard(CsulbId, MessageBoardIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                StudentProfileMessageBoardResponse response = new StudentProfileMessageBoardResponse();
                response.IsSuccess = false;
                response.Message = "Failed to fetch student profile message board data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateStudentProfileMessageBoard")]
        public BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _studentProfileService.UpdateStudentProfileMessageBoard(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("SaveStudentProfileData")]
        public BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _studentProfileService.SaveStudentProfileData(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetApplicationList")]
        public IActionResult GetApplicationList(int userId,string identifier)
        {
            try
            {
                List<ApplicationList> response = _studentProfileService.GetApplications(userId,identifier);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        [HttpGet("GetSemesterTermList")]
        public SemesterTermListResponse GetSemesterTermList(int applicationId)
        {
            try
            {
                SemesterTermListResponse response = _studentProfileService.GetSemesterList(applicationId);
                return response;
            }
            catch (Exception ex)
            {
                SemesterTermListResponse response = new SemesterTermListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetApplicationProgramList")]
        public ApplicationProgramListResponse GetApplicationProgramList(int userID, int applicationTypeID, string termCode)
        {
            try
            {
                ApplicationProgramListResponse response = _studentProfileService.GetApplicationProgramList(userID, applicationTypeID, termCode);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationProgramListResponse response = new ApplicationProgramListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetStudentAppliedFormsByPrograms")]
        public StudentAppliedFormsByProgramsResponse GetStudentAppliedFormsByPrograms( int programID, string termCode,string CSULBID )
        {
            try
            {
                StudentAppliedFormsByProgramsResponse response = _studentProfileService.GetStudentAppliedFormsByPrograms( programID, termCode,CSULBID);
                return response;
            }
            catch (Exception ex)
            {
                StudentAppliedFormsByProgramsResponse response = new StudentAppliedFormsByProgramsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertProfileAttachment")]
        public UpsertProfileAttachmentResponse UpsertProfileAttachment(UpsertProfileDocumentRequest input)
        {
            try
            {
                UpsertProfileAttachmentResponse response = new UpsertProfileAttachmentResponse();

                //#region to get the file content from local
                //byte[] fileContent = null;
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\test3.pdf";
                //string filepath = "D:\\ExcelDoc\\Screenshot 2024-10-15 123100.png";
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                //Byte[] InputStream = null;
                //input.FileContent = fileContent;
                //#endregion

                response = _studentProfileService.UpsertProfileAttachment(input);
                return response;
            }
            catch (Exception ex)
            {
                UpsertProfileAttachmentResponse response = new UpsertProfileAttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("DownloadProfileAttachment")]
        public IActionResult DownloadProfileAttachment(Guid UniqueID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;
                DownloadProfileAttachmentResponse obj = _studentProfileService.DownloadProfileAttachment(UniqueID);
                fileName = obj.FileName;
                inputStream = obj.FileContent;
                string[] fileSplit = fileName.Split('.');
                string fileextension = fileName.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetProfileAttachmentDetails")]
        public ProfileAttachmentDetailsResponse GetProfileAttachmentDetails(string CSULBID)
        {
            try
            {
                ProfileAttachmentDetailsResponse response = _studentProfileService.GetProfileAttachmentDetails(CSULBID);
                return response;
            }
            catch (Exception ex)
            {
                ProfileAttachmentDetailsResponse response = new ProfileAttachmentDetailsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("DeleteProfileAttachment")]
        public BaseResponse DeleteProfileAttachment(Guid UniqueID)
        {
            try
            {
                BaseResponse response = _studentProfileService.DeleteProfileAttachment(UniqueID);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to delete data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        private string GetFileType(string fileExt)
        {
            string contentType = string.Empty;
            switch (fileExt.ToUpper())
            {
                case "PDF":
                    contentType = "application/pdf";
                    break;
                case "DOCX":
                    contentType = "Application/msword";
                    break;
                case "DOC":
                    contentType = "Application/msword";
                    break;
                case "XLSX":
                    contentType = "Application/x-msexcel";
                    break;
                case "XLS":
                    contentType = "Application/x-msexcel";
                    break;
                case "JPG":
                    contentType = "image/jpeg";
                    break;
                case "JPEG":
                    contentType = "image/jpeg";
                    break;

            }
            return contentType;
        }

    }
}
