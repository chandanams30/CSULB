using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FieldWorkController : ControllerBase
    {
        public ILogger<FieldWorkController> _logger;
        public IFieldWorkService _fieldWorkService;
        private readonly IConfiguration _configuration;
        public FieldWorkController(IFieldWorkService fieldWorkService
            , ILogger<FieldWorkController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _fieldWorkService = fieldWorkService;
            _configuration = configuration;

        }
        [HttpGet("GetFieldWorkList")]
        public IActionResult GetFieldWorkList(int userId)
        {
            try
            {
                FieldWorkListResponse lstFieldWork = _fieldWorkService.GetFieldWorkList(userId);
                return Ok(lstFieldWork);
            }
            catch (Exception ex)
            {
                FieldWorkListResponse response = new FieldWorkListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return BadRequest(response);
            }
        }
        [HttpGet("GetFieldWorkById")]
        public FieldWorkDataResponse GetFieldWorkById(int userId,int fieldWorkId)
        {
            try
            {
                FieldWorkDataResponse fieldWorkDataResponse = _fieldWorkService.GetFieldWorkDetailsById(userId, fieldWorkId);
                return fieldWorkDataResponse;
            }
            catch (Exception ex)
            {
                FieldWorkDataResponse response = new FieldWorkDataResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpdateFieldWorkValidation")]
        public BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            try
            {
                BaseResponse response = _fieldWorkService.UpdateFieldWorkValidation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UploadFieldWorkRequiredDocuments")]
        public BaseResponse UploadFieldWorkRequiredDocuments(FieldWorkUploadDocumentsRequest input)
        {
            #region testing with manual file , actual file will come as byte array 
            //-------------just for testing - comment it after testing
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
            //string filepath = "D:\\CSULB\\Document\\TBCTC_Approval.pdf";
            //byte[] fileContent = null;
            //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            //long byteLength = new System.IO.FileInfo(filepath).Length;
            //fileContent = binaryReader.ReadBytes((Int32)byteLength);
            //input.FileContent = fileContent;
            //fs.Close();
            //fs.Dispose();
            //binaryReader.Close();
            //string fc = fileContent.ToString();
            //----end comment----------------------------------------
            #endregion

            try
            {
                BaseResponse response = _fieldWorkService.UpdateFieldWorkDocumentValidation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldWorkMetaData")]
        public BaseResponse UpdateFieldWorkMetaData(FieldWorkUploadDocumentsRequest input)
        {
            BaseResponse response = new BaseResponse();

            return response;
        }
        [HttpGet("DownloadRequiredDocuments")]
        public IActionResult DownloadRequiredDocuments(int userId,int fieldworkAttachmentId)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            FieldWorkProfileAttachments obj = _fieldWorkService.DownloadRequiredDocuments(userId,fieldworkAttachmentId);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');
            string fileextension = obj.FileName.Split('.').Last();
            fileType = GetFileType(fileextension);
            //fileType = GetFileType(fileSplit[1]);
            //Response.Headers.Add("Content-Disposition", "inline");
            //return File(inputStream, fileType);
            return File(inputStream, fileType, fileName);
        }
        [HttpGet("GetFieldWorkActivityLog")]
        public FieldWorkActivityLogListResponse GetFieldWorkActivityLog(int userId, int fieldWorkId)
        {
            try
            {
                FieldWorkActivityLogListResponse response = new FieldWorkActivityLogListResponse();
                response = _fieldWorkService.GetFieldWorkActivityLog(userId, fieldWorkId);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogListResponse response = new FieldWorkActivityLogListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldWorkActivityLog")]
        public FieldWorkActivityLogByIDResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input)
        {
            try
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response = _fieldWorkService.UpdateFieldWorkActivityLog(input);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save Activity Log, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldWorkActivityLogStatus")]
        public BaseResponse UpdateFieldWorkActivityLogStatus(UpdateFieldWorkActivityLogStatusRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _fieldWorkService.UpdateFieldWorkActivityLogStatus(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Update Activity Log Status, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldWorkActivityLogStatusList")]
        public BaseResponse UpdateFieldWorkActivityLogStatusList(UpdateFieldWorkActivityLogStatusListRequest inputs)
        {
            try
            {
                foreach (UpdateFieldWorkActivityLogStatusRequest input in inputs.logStatusList)
                {
                     _fieldWorkService.UpdateFieldWorkActivityLogStatus(input);
                }
                BaseResponse obj = new BaseResponse();
                obj.IsSuccess = true;
                obj.Message = "Activity Log Status Updated Successfully";
                return obj;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Update Activity Log Status, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFieldWorkActivityLogByID")]
        public FieldWorkActivityLogByIDResponse GetFielWorkActivityLogByID(int userID, int activityLogID)
        {
            try
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response = _fieldWorkService.GetFielWorkActivityLogByID(userID,activityLogID);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save Activity Log, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFieldWorkActivityLogforAdd")]
        public FieldWorkActivityLogByIDResponse GetFieldWorkActivityLogforAdd(int userID, int fieldWorkID)
        {
            try
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response = _fieldWorkService.GetFieldWorkActivityLogforAdd(userID, fieldWorkID);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save Activity Log, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UploadFieldWorkActivityDocuments")]
        public FieldWorkAttachmentsResponse UploadFieldWorkActivityDocuments(FieldWorkAttachmentsRequest input)
        {
            try
            {
                #region testing with manual file , actual file will come as byte array 
                // comment the below after testing 


                //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
                ////// string filepath = "D:\\CSULB\\GitHub\\Documents\\TB-TEST.docx";
                //////string filepath = "D:\\CSULB\\GitHub\\Documents\\Student Clearance Form Sample.pdf";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();

                // end comment
                #endregion

                FieldWorkAttachmentsResponse response = new FieldWorkAttachmentsResponse();
                response = _fieldWorkService.UploadFieldWorkActivityDocuments(input);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkAttachmentsResponse response = new FieldWorkAttachmentsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to upload activity attachments, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("DownloadActivityAttachments")]
        public IActionResult DownloadActivityAttachments(int userId, int fieldworkId,string savedFileName)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            // FieldWorkProfileAttachments obj = _fieldWorkService.DownloadActivityAttachments(userId, fieldworkId);
            string filefolderName = GetFolderName(userId, fieldworkId);
            string filepath = Path.Combine(filefolderName, savedFileName);
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            //input.FileContent = fileContent;

            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            fileName = savedFileName;
            inputStream = fileContent;
            string[] fileSplit = savedFileName.Split('.');

            fileType = GetFileType(fileSplit[1]);

            return File(inputStream, fileType, fileName);
        }
        [HttpGet("GetCommunitySites")]
        public FieldWorkCommunitySitesResponse GetCommunitySites()
        {           
            try
            {
                FieldWorkCommunitySitesResponse response = new FieldWorkCommunitySitesResponse();
                response = _fieldWorkService.GetCommunitySites();
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkCommunitySitesResponse response = new FieldWorkCommunitySitesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetCommunitySiteUsers")]
        public FieldWorkCommunitySiteUsersResponse GetCommunitySiteUsers(int communitySiteId)
        {
            try
            {
                FieldWorkCommunitySiteUsersResponse response = new FieldWorkCommunitySiteUsersResponse();
                response = _fieldWorkService.GetCommunitySiteUsers(communitySiteId);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkCommunitySiteUsersResponse response = new FieldWorkCommunitySiteUsersResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFieldworkCommunityDistrict")]
        public FieldWorkCommunityDistrictResponse GetFieldworkCommunityDistrict()
        {
            try
            {
                FieldWorkCommunityDistrictResponse response = new FieldWorkCommunityDistrictResponse();
                response = _fieldWorkService.GetFieldworkCommunityDistrict();
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkCommunityDistrictResponse response = new FieldWorkCommunityDistrictResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetFieldWorkCoursesCategorySchoolTypes")]
        public GetFieldWorkCoursesCategorySchoolTypesResponse GetFieldWorkCoursesCategorySchoolTypes(int userID,int fieldWorkID)
        {
            try
            {
                GetFieldWorkCoursesCategorySchoolTypesResponse response = new GetFieldWorkCoursesCategorySchoolTypesResponse();
                response = _fieldWorkService.GetFieldWorkCoursesCategorySchoolTypes(userID,fieldWorkID);
                return response;
            }
            catch (Exception ex)
            {
                GetFieldWorkCoursesCategorySchoolTypesResponse response = new GetFieldWorkCoursesCategorySchoolTypesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetFieldworkCommunitySchoolSiteUsersByDistrict")]
        public GetFieldworkCommunitySchoolSiteUsersByDistrictResponse GetFieldworkCommunitySchoolSiteUsersByDistrict(int CommunityDistrictID)
        {
            try
            {
                GetFieldworkCommunitySchoolSiteUsersByDistrictResponse response = new GetFieldworkCommunitySchoolSiteUsersByDistrictResponse();
                response = _fieldWorkService.GetFieldworkCommunitySchoolSiteUsersByDistrict(CommunityDistrictID);
                return response;
            }
            catch (Exception ex)
            {
                GetFieldworkCommunitySchoolSiteUsersByDistrictResponse response = new GetFieldworkCommunitySchoolSiteUsersByDistrictResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetStandards")]
        public FieldWorkStandardsResponse GetStandards(int userID,int fieldWorkID)
        {
            try
            {
                FieldWorkStandardsResponse response = new FieldWorkStandardsResponse();
                response = _fieldWorkService.GetStandards(userID,fieldWorkID);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkStandardsResponse response = new FieldWorkStandardsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFnCSchema")]
        public FieldWorkFnCSchemaResponse GetFnCSchema(int userID, int fieldWorkID,int schemaType, int fieldWorkActivityLogID)
        {
            try
            {
                FieldWorkFnCSchemaResponse response = new FieldWorkFnCSchemaResponse();
                response = _fieldWorkService.GetFnCSchema(userID, fieldWorkID,schemaType,fieldWorkActivityLogID);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkFnCSchemaResponse response = new FieldWorkFnCSchemaResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFnCSchema")]
        public BaseResponse UpdateFnCSchema(FieldWorkFnCSchemaUpdateRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _fieldWorkService.UpdateFnCSchema(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save the schema , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldworkCommunityUsersforCreation")]
        public BaseResponse UpdateFieldworkCommunityUsersforCreation()
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _fieldWorkService.UpdateFieldworkCommunityUsersforCreation();
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save the schema , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList")]
        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList()
        {
            try
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse();
                response = _fieldWorkService.PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList();
                return response;
            }
            catch (Exception ex)
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail")]
        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail(int CSSDTID)
        {
            try
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();
                response = _fieldWorkService.PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail(CSSDTID);
                return response;
            }
            catch (Exception ex)
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetEvaluationByFieldWorkID")]
        public FieldWorkEvaluationByIDResponse GetEvaluationByFieldWorkID(int UserID, int FieldWorkID, int ProgramID, string TermCode)
        {
            try
            {
                FieldWorkEvaluationByIDResponse response = _fieldWorkService.GetEvaluationByFieldWorkID(UserID, FieldWorkID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkEvaluationByIDResponse response = new FieldWorkEvaluationByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertEvaluation")]
        public BaseResponse UpsertEvaluation(UpsertEvaluationRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _fieldWorkService.UpsertEvaluation(input);
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
        [AllowAnonymous]
        [HttpGet("GetEvaluationByEvaluationIdentifier")]
        public EvaluationByEvaluationIdentifierResponse GetEvaluationByEvaluationIdentifier(string evaluationIdentifier)
        {
            try
            {

                EvaluationByEvaluationIdentifierResponse response = _fieldWorkService.GetEvaluationByEvaluationIdentifier(evaluationIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                EvaluationByEvaluationIdentifierResponse response = new EvaluationByEvaluationIdentifierResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [AllowAnonymous]
        [HttpPost("UpdateEvaluationJSON")]
        public BaseResponse UpdateEvaluationJSON(UpdateEvaluationJSONRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _fieldWorkService.UpdateEvaluationJSON(input);
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
        [HttpPost("DownloadAttachment")]
        public IActionResult DownloadAttachment(DownloadAttachment input)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                FieldWorkAttachmentsRequest obj = _fieldWorkService.DownloadAttachment(input);
                fileName = obj.FileName;
                inputStream = obj.FileContent;
                string[] fileSplit = obj.FileName.Split('.');
                string fileextension = obj.FileName.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents")]
        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents(int communitySiteUsersID, string? communitySiteUserName, string? communitySiteUserEmail,int activityLogID)
        {
            try
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents();
                response = _fieldWorkService.PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents(communitySiteUsersID,communitySiteUserName,communitySiteUserEmail,activityLogID);
                return response;
            }
            catch (Exception ex)
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("DownloadActivityLogs")]
        public IActionResult DownloadActivityLogs(FieldWorkActivityLogsAttachmentRequest input)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            FieldWorkActivityLogsAttachmentResponse obj = _fieldWorkService.DownloadActivityLogs(input);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');
            string fileextension = obj.FileName.Split('.').Last();
            fileType = GetFileType(fileextension);
            return File(inputStream, fileType, fileName);
        }
        [HttpPost("DeleteActivityLog")]
        public BaseResponse DeleteFieldWorkActivityLog(DeleteActivityLogRequest input)
        {
            try
            {
                BaseResponse response = _fieldWorkService.DeleteFieldWorkActivityLog(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to delete activity log , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateCommunitySiteSupervisorDemonstrationTeacherList")]
        public UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse PUNS_UpdateCommunitySiteSupervisorDemonstrationTeacherList(UpdateCommunitySiteSupervisorDemonstrationTeacherListRequest input)
        {
            try
            {
                UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse response = new UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse();
                response = _fieldWorkService.PUNS_UpdateCommunitySiteSupervisorDemonstrationTeacherList(input);
                return response;
            }
            catch (Exception ex)
            {
                UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse response = new UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Update Community User List, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("PrerequisiteExpiredMail")]
        public AdhocMailLogResponse PrerequisiteExpired_Sendmail_To_Students(PrerequisiteExpiredRequest input)
        {
            try
            {
                AdhocMailLogResponse response = new AdhocMailLogResponse();
                response = _fieldWorkService.PrerequisiteExpired_Sendmail_To_Students(input);
                return response;
            }
            catch (Exception ex)
            {
                AdhocMailLogResponse response = new AdhocMailLogResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Send mail , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("PrerequisiteApprovedMail")]
        public AdhocMailLogResponse StudentsEnrolled_ApprovedDocuments_BulkEmail(PrerequisiteApprovedRequest input)
        {
            try
            {
                AdhocMailLogResponse response = _fieldWorkService.StudentsEnrolled_ApprovedDocuments_BulkEmail(input);
                return response;
            }
            catch (Exception ex)
            {
                AdhocMailLogResponse response = new AdhocMailLogResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        private string GetFolderName(int userId, int fieldWorkID)
        {
            string folderName = string.Empty;
            string completePath = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            folderName = Path.Combine(userId.ToString() + "//Fieldwork", fieldWorkID.ToString());
            completePath= Path.Combine(fileRepoPath,folderName);
            return completePath;
        }

        private string GetFileType(string fileExt)
        {
            string contentType = string.Empty;
            switch(fileExt.ToUpper())
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
