using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Request.StudentTeachingMilestone;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class StudentTeachingMilestoneController : ControllerBase
    {
        private ILogger<StudentTeachingMilestoneController> _logger;
        private IStudentTeachingMilestoneService _studentTeachingMilestoneService;
        private readonly IConfiguration _configuration;
        public StudentTeachingMilestoneController(IStudentTeachingMilestoneService studentTeachingMilestoneService,
              ILogger<StudentTeachingMilestoneController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _studentTeachingMilestoneService = studentTeachingMilestoneService;
        }
        [HttpGet("GetLatestForm")]
        public GetLatestFormResponse GetLatestForm(string csulbid)
        {
            try
            {
                GetLatestFormResponse response = _studentTeachingMilestoneService.GetLatestForm(csulbid);
                return response;
            }
            catch (Exception ex)
            {
                GetLatestFormResponse response = new GetLatestFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                return response;
            }
        }
        [HttpGet("GetStudentTeachingMilestoneDetails")]
        public StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsulbId, int UserID, int formID, int roleID, int programID)
        {
            try
            {
                StudentTeachingMilestonesResponse response = _studentTeachingMilestoneService.GetStudentTeachingMilestoneDetails(CsulbId, UserID, formID, roleID, programID);
                return response;
            }
            catch (Exception ex)
            {
                StudentTeachingMilestonesResponse response = new StudentTeachingMilestonesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to fetch student profile data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetDistrictsByProgramID")]
        public DistrictListResponse GetDistrictsByProgramID(int programId)
        {
            try
            {
                DistrictListResponse response = _studentTeachingMilestoneService.GetDistrictsByProgramID(programId);
                return response;
            }
            catch (Exception ex)
            {
                DistrictListResponse response = new DistrictListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetCredentialPathwayAreaList")]
        public CredentialAreaListResponse GetCredentialPathwayAreaList()
        {
            try
            {
                CredentialAreaListResponse response = _studentTeachingMilestoneService.GetCredentialPathwayAreaList();
                return response;
            }
            catch (Exception ex)
            {
                CredentialAreaListResponse response = new CredentialAreaListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertAttachement")]
        public AttachmentResponse UpsertAttachement(UpsertAttachments input)
        {
            try
            {
                AttachmentResponse response = new AttachmentResponse();

                //#region to get the file content from local
                //byte[] fileContent = null;
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\test3.pdf";
                //string filepath = "D:\\ExcelDoc\\Mileage-Report-1775048340024.pdf";
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

                response = _studentTeachingMilestoneService.UpsertAttachement(input);
                return response;
            }
            catch (Exception ex)
            {
                AttachmentResponse response = new AttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("DownloadAttachement")]
        public IActionResult DownloadAttachement(Guid UniqueID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;
                DownloadAttachementResponse obj = _studentTeachingMilestoneService.DownloadAttachement(UniqueID);
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
        [HttpPost("DeleteAttachement")]
        public BaseResponse DeleteAttachement(DeleteAttachementRequest input)
        {
            try
            {
                BaseResponse response = _studentTeachingMilestoneService.DeleteAttachement(input);
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
        [HttpPost("AddCredentialArea")]
        public BaseResponse AddCredentialArea(AddCredentialAreaRequest input)
        {
            try
            {
                BaseResponse response = _studentTeachingMilestoneService.AddCredentialArea(input);
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

        [HttpPost("SaveStudentTeachingMilestone")]
        public BaseResponse SaveStudentTeachingMilestone(StudentTeachingMilestoneRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _studentTeachingMilestoneService.SaveStudentTeachingMilestone(input);
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
    }

}
