using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.Guests;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        public ILogger<GraduateProgramController> _logger;
        public IGraduateProgramService _graduateProgramService;
        public IInitialCredentialProgramService _initialCredentialProgramService;
        public IFieldWorkService _fieldWorkService;
        public IMilestonesService _milestonesService;
        private readonly IApplicationService _applicationService;
        private readonly IConfiguration _configuration;
        private readonly ISqlDBUtility _helper;
        public IGuestsService _guestsService;
        public GuestsController(IGraduateProgramService graduateProgramService ,
              ILogger<GraduateProgramController> logger, IApplicationService applicationService
            , IConfiguration configuration, ISqlDBUtility helper 
            ,IInitialCredentialProgramService initialCredentialProgramService
            ,IFieldWorkService fieldWorkService
            ,IMilestonesService milestonesService
            ,IGuestsService guestsService)
        {
            _logger = logger;
            _graduateProgramService = graduateProgramService;
            _applicationService = applicationService;
            _configuration = configuration;
            _helper = helper;
            _initialCredentialProgramService = initialCredentialProgramService;
            _fieldWorkService = fieldWorkService;
            _milestonesService = milestonesService;
            _guestsService = guestsService;
        }


        [HttpGet("GetAppliedFormsByPrograms")]
        public AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID,string termCode,int formStateID)
        {
            try
            {
                AppliedFormsByProgramsResponse response = _graduateProgramService.GetAppliedFormsByPrograms(userID, programID,termCode,formStateID);
                return response;
            }
            catch (Exception ex)
            {
                AppliedFormsByProgramsResponse response = new AppliedFormsByProgramsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetFormStates")]
        public FormStatesResponse GetFormStates(int userID)
        {
            try
            {
                FormStatesResponse response = _graduateProgramService.GetFormStates(userID);
                return response;
            }
            catch (Exception ex)
            {
                FormStatesResponse response = new FormStatesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetSemesterList")]
        public SemesterListResponse GetSemesterList()
        {
            try
            {
                SemesterListResponse response = _graduateProgramService.GetSemesterList();
                return response;
            }
            catch (Exception ex)
            {
                SemesterListResponse response = new SemesterListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetForm")]
        public GraduateProgramFormResponse GetForm(int userID,int formID,int programID,string termCode)
        {
            try
            {
                GraduateProgramFormResponse response = _graduateProgramService.GetForm(userID,formID,programID,termCode,false);
                return response;
            }
            catch (Exception ex)
            {
                GraduateProgramFormResponse response = new GraduateProgramFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        
        [HttpGet("GetFormStudentMessageBoard")]
        public StudentMessageBoardResponse GetFormStudentMessageBoard(int UserID, int FormID, int ProgramID, string TermCode)
        {
            try
            {
                StudentMessageBoardResponse response = new StudentMessageBoardResponse();
                response = _graduateProgramService.GetFormStudentMessageBoard(UserID, FormID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                StudentMessageBoardResponse response = new StudentMessageBoardResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetApplicationList")]
        public IActionResult GetApplicationList(int userId,int roleId)
        {
            try
            {
                #region Below code is to pull the claims from token , currently pulling just the UserID -------------------
                //var user = User as ClaimsPrincipal;
                //string userIdFromToken = user.Claims.Where(c => c.Type == "UserID")
                //    .Select(x => x.Value).FirstOrDefault();
                // gets the application list  
                #endregion

                List<ApplicationListResponse> response = _applicationService.GetApplications(userId,roleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        [HttpPost("SaveForm")]
        public BaseResponse SaveForm(FormSaveRequest input)
        {
            try
            {

                BaseResponse response = _graduateProgramService.SaveForm(input);
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
        [HttpPost("UpsertFormInterviewDateFacultyAttachment")]
        public BaseResponse UpsertFormAttachment(FormUpsertAttachmentRequest input)
        {
            try
            {
                #region commented area to pull the file content 
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\MYDOCS.png";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\pic2.jpg";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpeg";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\MyDOC.docx";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                #endregion
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.UpsertInterviewDate(input);
                if ((input.FileName != string.Empty) && (input.FileContent != null))
                {
                    response = _graduateProgramService.UpsertFormAttachment(input);
                    response.Message = "Updated Interview Date with Faculty successfully";
                }
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
        [HttpGet("DownloadFormAttachments")]
        public IActionResult DownloadFormAttachments(int userID, int formattachmentID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                ThoughtFocus.Domain.Request.GraduateProgram.FormAttachments obj = _graduateProgramService.DownloadFormAttachments(userID, formattachmentID);
                fileName = obj.Filename;
                inputStream = obj.FileContent;
                string[] fileSplit = obj.Filename.Split('.');
                string fileextension = obj.Filename.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ValidateForm")]
        public GraduateProgramFormResponse ValidateForm(string part1, string part2, string part3)
        {
            try
            {
                var secretKey = _configuration["ApplicationKeys:SecretKey"];
                byte[] Key = new byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(secretKey), Key, Math.Min(Key.Length, Encoding.UTF8.GetBytes(secretKey).Length));
                string part3Decrypt = DecodeBase64AndDecrypt(part3, Key);
                string part2Decrypt = string.Empty;
                if (part3Decrypt == part1.ToString())
                {
                    part2Decrypt = DecodeBase64AndDecrypt(part2, Key);

                    string[] keyValuePairs = part2Decrypt.Split('&');
                    var keyValueDictionary = new System.Collections.Generic.Dictionary<string, string>();

                    foreach (string keyValue in keyValuePairs)
                    {
                        // Split each key-value pair by '='
                        string[] parts = keyValue.Split('=');

                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            string value = parts[1];
                            keyValueDictionary[key] = value;
                        }
                    }
                    int userID = Convert.ToInt32(keyValueDictionary["userID"]);
                    int programID = Convert.ToInt32(keyValueDictionary["programID"]);
                    string termCode = keyValueDictionary["termCode"];
                    bool showMileStone = true;
                    SqlParameter[] parameters =
                                             {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.BigInt) { Value = termCode }
                                        };
                    DataTable dtAppliedForms = _helper.GetDataTable("[Application].[GetFormID]", parameters);
                    int formID = 0;
                    GraduateProgramFormResponse response = new GraduateProgramFormResponse();
                    if (dtAppliedForms.Rows.Count > 0)
                    {
                        formID = Convert.ToInt32(dtAppliedForms.Rows[0]["FormID"]);
                        response = _graduateProgramService.GetForm(userID, formID, programID, termCode,showMileStone);
                    }
                    else
                    {
                        formID = 0;
                        response = _graduateProgramService.GetForm(userID, formID, programID, termCode,showMileStone);
                    }
                    return response;
                }
                else
                {
                    GraduateProgramFormResponse response = new GraduateProgramFormResponse();
                    response.IsSuccess = false;
                    response.Message = "Invalid link";
                    return response;
                }
            }
            catch (Exception ex)
            {
                GraduateProgramFormResponse response = new GraduateProgramFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }


        }
        private string DecodeBase64AndDecrypt(string base64CipherText, byte[] Key)
        {

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Mode = CipherMode.ECB; // Use CFB mode for educational purposes (not recommended for security)
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor();

                byte[] encryptedBytes = Convert.FromBase64String(base64CipherText);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        [HttpPost("DeleteFormAttachment")]
        public BaseResponse DeleteFormAttachment(DeleteFormAttachmentRequest input)
        {
            try
            {
                #region commented area to pull the file content 
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\MYDOCS.png";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\pic2.jpg";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpeg";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\MyDOC.docx";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                #endregion
                BaseResponse response = _graduateProgramService.DeleteFormAttachment(input);
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
        [HttpPost("UpdateFormState")]
        public BaseResponse UpdateFormState(FormStatusUpdateRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpdateFormState(input);
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
        [HttpPost("UpsertFormEducationInformationAttachment")]
        public UpsertFormEducationInformationAttachmentResponse UpsertFormEducationInformationAttachment(UpsertFormEducationInformationAttachmentRequest input)
        {
            try
            {
                UpsertFormEducationInformationAttachmentResponse response = new UpsertFormEducationInformationAttachmentResponse();

                #region to get the file content from local
                //byte[] fileContent = null;
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\test3.pdf";
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                //Byte[] InputStream = null;
                //input.FileContent = fileContent;
                #endregion

                response = _initialCredentialProgramService.UpsertFormEducationInformationAttachment(input);
                return response;
            }
            catch (Exception ex)
            {
                UpsertFormEducationInformationAttachmentResponse response = new UpsertFormEducationInformationAttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetFormEducationInformationAttachment")]
        public IActionResult GetFormEducationInformationAttachment(int FormID, Guid UniqueID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;
                DownloadEducationalInformationalAttachment obj = _initialCredentialProgramService.GetFormEducationInformationAttachment(FormID, UniqueID);
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
        [HttpGet("GetIntialCreditialOptionItemsList")]
        public OptionItemListResponse GetIntialCreditialOptionItemsList(int programID)
        {
            try
            {
                OptionItemListResponse response = _initialCredentialProgramService.GetIntialCreditialOptionItemsList(programID);
                return response;
            }
            catch (Exception ex)
            {
                OptionItemListResponse response = new OptionItemListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetProgramConfigurationHandler")]
        public ProgramConfigurationHandlerResponse GetProgramConfigurationHandler(int UserID, int FormID, int ProgramID, string TermCode)
        {
            try
            {
                ProgramConfigurationHandlerResponse response = new ProgramConfigurationHandlerResponse();
                response = _graduateProgramService.GetProgramConfigurationHandler(UserID, FormID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                ProgramConfigurationHandlerResponse response = new ProgramConfigurationHandlerResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("GetFormSubSection")]
        public FormSubSectionResponse GetFormSubSection(FormSubsectionRequest input)
        {
            try
            {
                FormSubSectionResponse response = _initialCredentialProgramService.GetFormSubSection(input);
                return response;
            }
            catch (Exception ex)
            {
                FormSubSectionResponse response = new FormSubSectionResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("GetFormSubSectionAttachmentList")]
        public FormSectionAttachmentResponse GetFormSubSectionAttachmentList(FormSubsectionRequest input)
        {
            try
            {
                FormSectionAttachmentResponse response = _initialCredentialProgramService.GetFormSubSectionAttachmentList(input);
                return response;
            }
            catch (Exception ex)
            {
                FormSectionAttachmentResponse response = new FormSectionAttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFormSubSectionApproversDetails")]
        public FormSectionApprovalDetailsResponse GetFormSubSectionApproveralDetails(int FormID, int UserID, int FormSubSectionID, string SubSectionIdentifiers)
        {
            try
            {
                FormSectionApprovalDetailsResponse response = _initialCredentialProgramService.GetFormSubSectionApproveralDetails(FormID, UserID, FormSubSectionID, SubSectionIdentifiers);
                return response;
            }
            catch (Exception ex)
            {
                FormSectionApprovalDetailsResponse response = new FormSectionApprovalDetailsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetAdditionalOfficialDocuments")]
        public AdditionalOfficialDocumentsResponse GetAdditionalOfficialDocuments(int UserID, int FormID, int ProgramID, string TermCode)
        {
            try
            {
                AdditionalOfficialDocumentsResponse response = _initialCredentialProgramService.GetAdditionalOfficialDocuments(UserID, FormID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                AdditionalOfficialDocumentsResponse response = new AdditionalOfficialDocumentsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("GetFormSubSectionAttachment")]
        public IActionResult GetFormSubSectionAttachment(SubsectionAttachmentDownloadRequest input)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;
                FormSubsectionAttachmentDownloadResponse obj = _initialCredentialProgramService.GetFormSubSectionAttachment(input);
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
        [HttpPost("SaveFormSubSectionAttachment")]
        public BaseResponse SaveFormSubSectionAttachment(SaveFormSubSectionAttachmentRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                #region to get the file content from local
                //byte[] fileContent = null;
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\test3.pdf";
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                //Byte[] InputStream = null;
                //input.FileContent = fileContent;
                #endregion

                response = _initialCredentialProgramService.SaveFormSubSectionAttachment(input);
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
        [HttpPost("UpsertFormSubSection")]
        public BaseResponse UpsertFormSubSection(UpsertFormSubSectionRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _initialCredentialProgramService.UpsertFormSubSection(input);
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
        [HttpGet("PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher")]
        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail)
        {
            try
            {
                PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail response = _fieldWorkService.PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(CommunitySiteUserIdentifier,CommunitySiteUserEmail);
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

        [HttpGet("GetFieldWorkData")]
        public FieldWorkListResponse GetFieldWorkData(string CommunitySiteUserIdentifier)
        {
            try
            {
                FieldWorkListResponse response = _fieldWorkService.GetFieldWorkData(CommunitySiteUserIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkListResponse response = new FieldWorkListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }

        }

        [HttpGet("GetFieldWorkActivityLogList")]
        public PUFieldWorkActivityLogListResponse GetFieldWorkActivityLogList(string CommunitySiteUserIdentifier,int fieldWorkId)
        {
            try
            {
                PUFieldWorkActivityLogListResponse response = _fieldWorkService.GetFieldWorkActivityLogList(CommunitySiteUserIdentifier, fieldWorkId);
                return response;
            }
            catch (Exception ex)
            {
                PUFieldWorkActivityLogListResponse response = new PUFieldWorkActivityLogListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }

        }

        [HttpGet("GetFieldWorkActivityLogByID")]
        public FieldWorkActivityLogByIDResponse PUNS_GetFieldWorkActivityLogByID(string CommunitySiteUserIdentifier, int activityLogId)
        {
            try
            {
                FieldWorkActivityLogByIDResponse response = _fieldWorkService.PUNS_GetFieldWorkActivityLogByID(CommunitySiteUserIdentifier, activityLogId);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogByIDResponse response = new FieldWorkActivityLogByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }

        }

        [HttpPost("UpdateFieldWorkActivityLogStatus")]
        public BaseResponse PUNS_UpdateFieldWorkActivityLogStatus(PUUpdateFieldWorkActivityLogStatusRequest input)
        {
            try
            {
                BaseResponse response = _fieldWorkService.PUNS_UpdateFieldWorkActivityLogStatus(input);
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
        [HttpPost("UpdateFieldWorkActivityLogStatusList")]
        public BaseResponse PUNS_UpdateFieldWorkActivityLogStatusList(PUUpdateFieldWorkActivityLogStatusListRequest inputs)
        {
            try
            {
                foreach (PUUpdateFieldWorkActivityLogStatusRequest input in inputs.logStatusList)
                {
                    _fieldWorkService.PUNS_UpdateFieldWorkActivityLogStatus(input);
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
        [HttpGet("GetFnCSchema")]
        public FieldWorkFnCSchemaResponse GetFnCSchema(int userID, int fieldWorkID, int schemaType, int fieldWorkActivityLogID)
        {
            try
            {
                FieldWorkFnCSchemaResponse response = new FieldWorkFnCSchemaResponse();
                response = _fieldWorkService.GetFnCSchema(userID, fieldWorkID, schemaType, fieldWorkActivityLogID);
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
        [HttpGet("SendReminderToRecommender")]
        public BaseResponse SendReminderToRecommender(int recommendationID)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.SendReminderToRecommender(recommendationID);
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
        [HttpPost("GetFormDispositionsAssessment")]
        public FormDispositionsAssessmentResponse GetFormDispositionsAssessment(FormDispositionsAssessmentRequest input)
        {
            try
            {
                FormDispositionsAssessmentResponse response = _initialCredentialProgramService.GetFormDispositionsAssessment(input);
                return response;
            }
            catch (Exception ex)
            {
                FormDispositionsAssessmentResponse response = new FormDispositionsAssessmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFormPrerequisites")]
        public FormPrerequisitesResponse GetFormPrerequisites(int UserId, int FormID)
        {
            try
            {
                FormPrerequisitesResponse response = _initialCredentialProgramService.GetFormPrerequisites(UserId, FormID);
                return response;
            }
            catch (Exception ex)
            {
                FormPrerequisitesResponse response = new FormPrerequisitesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertFormDispositionsAssessment")]
        public BaseResponse UpsertFormDispositionsAssessment(UpsertFormDispositionsAssessmentRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _initialCredentialProgramService.UpsertFormDispositionsAssessment(input);
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
        [HttpPost("UpdateFormSubSectionSubmitForReview")]
        public BaseResponse UpdateFormSubSectionSubmitForReview(UpdateFormSubSectionSubmitForReviewRequest input)
        {
            try
            {

                BaseResponse response = _initialCredentialProgramService.UpdateFormSubSectionSubmitForReview(input);
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
        [HttpGet("DownloadRequiredDocuments")]
        public IActionResult DownloadRequiredDocuments(int userId, int fieldworkAttachmentId)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            FieldWorkProfileAttachments obj = _fieldWorkService.DownloadRequiredDocuments(userId, fieldworkAttachmentId);
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
        
        [HttpGet("GetMilestoneApplicationForm")]
        public GetMilestoneApplicationFormResponse GetMilestoneApplicationForm(int UserID, int MilestoneFormID, int FormID, int MilestonePublishedFormID, bool IsReApply,bool IsExternalApprover)
        {
            try
            {
                GetMilestoneApplicationFormResponse response = _milestonesService.GetMilestoneApplicationForm(UserID, MilestoneFormID, FormID, MilestonePublishedFormID, IsReApply, IsExternalApprover);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneApplicationFormResponse response = new GetMilestoneApplicationFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetMilestoneWorkflowProcessTransitionHistory")]
        public GetMilestoneWorkflowProcessTransitionHistoryResponse GetMilestoneWorkflowProcessTransitionHistory(int MilestoneFormID, int RoleID)
        {
            try
            {
                GetMilestoneWorkflowProcessTransitionHistoryResponse response = _milestonesService.GetMilestoneWorkflowProcessTransitionHistory(MilestoneFormID, RoleID);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneWorkflowProcessTransitionHistoryResponse response = new GetMilestoneWorkflowProcessTransitionHistoryResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertMilestoneFilledForm")]
        public BaseResponse UpsertMilestoneFilledForm(UpsertMilestoneFilledFormRequest input)
        {
            try
            {
                BaseResponse response = _milestonesService.UpsertMilestoneFilledForm(input);
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
        [HttpGet("ApplyStudentMilestone")]
        public StudentMilestoneListResponse ApplyStudentMilestone(int UserID)
        {
            try
            {
                StudentMilestoneListResponse response = _milestonesService.GetStudentsMilestone(UserID);
                return response;
            }
            catch (Exception ex)
            {
                StudentMilestoneListResponse response = new StudentMilestoneListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetMilestoneSubmittedFormsList")]
        public GetMilestoneSubmittedFormsList GetMilestoneSubmittedFormsList(int UserID, int FormID,string ExternalApprovalIdentifier)
        {
            try
            {
                GetMilestoneSubmittedFormsList response = _guestsService.GetMilestoneSubmittedFormsList(UserID,FormID, ExternalApprovalIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneSubmittedFormsList response = new GetMilestoneSubmittedFormsList();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("SendRemainderToApprover")]
        public BaseResponse SendRemainderToApprover(string ExternalApprovalIdentifier, int MilestoneFormID)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _milestonesService.SendRemainderToApprover(ExternalApprovalIdentifier, MilestoneFormID);
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
        [HttpPost("UpsertMilestoneFormAttachment")]
        public MilestoneFormAttachmentResponse UpsertMilestoneFormAttachment(UpsertMilestoneFormAttachment input)
        {
            try
            {
                #region commented area to pull the file content 
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\Screenshot 2024-06-27 162131.png";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\timelog from S4.pdf";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpeg";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\MyDOC.docx";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                #endregion
                MilestoneFormAttachmentResponse response = _milestonesService.UpsertMilestoneFormAttachment(input);
                return response;
            }
            catch (Exception ex)
            {
                MilestoneFormAttachmentResponse response = new MilestoneFormAttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("DownloadMilestoneFormAttachments")]
        public IActionResult DownloadMilestoneFormAttachments(string FileName)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                ThoughtFocus.Domain.Request.GraduateProgram.FormAttachments obj = _milestonesService.DownloadMilestoneFormAttachments(FileName);
                fileName = obj.Filename;
                inputStream = obj.FileContent;
                string[] fileSplit = obj.Filename.Split('.');
                string fileextension = obj.Filename.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetClinicalPracticeEquivalencyAttachmentList")]
        public ClinicalPracticeEquivalencyAttachmentList GetClinicalPracticeEquivalencyAttachmentList(int UserID, int FormID)
        {
            try
            {
                ClinicalPracticeEquivalencyAttachmentList response = _initialCredentialProgramService.GetClinicalPracticeEquivalencyAttachmentList(UserID, FormID);
                return response;
            }
            catch (Exception ex)
            {
                ClinicalPracticeEquivalencyAttachmentList response = new ClinicalPracticeEquivalencyAttachmentList();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFormAttachmentDeatils")]
        public FormAttachmentDeatilsResponse GetFormAttachmentDeatils(int userID, int programID, int formID)
        {
            try
            {
                FormAttachmentDeatilsResponse response = _graduateProgramService.GetFormAttachmentDeatils(userID, programID, formID);
                return response;
            }
            catch (Exception ex)
            {
                FormAttachmentDeatilsResponse response = new FormAttachmentDeatilsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("SaveClinicalPracticeEquivalencyAttachment")]
        public BaseResponse SaveClinicalPracticeEquivalencyAttachment(SaveClinicalPracticeEquivalencyAttachmentRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                //#region to get the file content from local
                //byte[] fileContent = null;
                //string filepath = "D:\\ExcelDoc\\CTC_10232024162937.pdf";
                ////string filepath = "D:\\ExcelDoc\\PK3_OtherChanges_20250220.pdf";
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

                response = _initialCredentialProgramService.SaveClinicalPracticeEquivalencyAttachment(input);
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
        [HttpPost("GetClinicalPracticeEquivalencyAttachment")]
        public IActionResult GetClinicalPracticeEquivalencyAttachment(ClinicalPracticeEquivalencyAttachmentRequest input)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;
                FormSubsectionAttachmentDownloadResponse obj = _initialCredentialProgramService.GetClinicalPracticeEquivalencyAttachment(input);
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
        [HttpGet("GetLetterOfRecommendationsByFormID")]
        public LetterOfRecommendationsByFormIDResponse GetLetterOfRecommendationsByFormID(int UserID, int FormID, int ProgramID, string TermCode)
        {
            try
            {
                LetterOfRecommendationsByFormIDResponse response = _initialCredentialProgramService.GetLetterOfRecommendationsByFormID(UserID, FormID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                LetterOfRecommendationsByFormIDResponse response = new LetterOfRecommendationsByFormIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("GetInstructorList")]
        public InstructorListResponse GetInstructorList(GetInstructorInterviewerListRequest input)
        {
            try
            {
                InstructorListResponse response = new InstructorListResponse();
                response = _graduateProgramService.GetInstructorList(input);
                return response;
            }
            catch (Exception ex)
            {
                InstructorListResponse response = new InstructorListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertLetterOfRecommendations")]
        public BaseResponse UpsertLetterOfRecommendations(UpsertLetterOfRecommendationsRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _initialCredentialProgramService.UpsertLetterOfRecommendations(input);
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
        public IActionResult DownloadAttachment(DownloadAttachmentRequest input)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                ThoughtFocus.Domain.Request.InitialCredentialProgram.FormAttachments obj = _initialCredentialProgramService.DownloadAttachment(input);
                fileName = obj.Filename;
                inputStream = obj.FileContent;
                string[] fileSplit = obj.Filename.Split('.');
                string fileextension = obj.Filename.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddInstructorToForm")]
        public BaseResponse AddInstructorToForm(AddInstructorRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AddInstructorToForm(input);
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
        [HttpPost("UpdateFormStudentMessageBoard")]
        public BaseResponse UpdateFormStudentMessageBoard(StudentMessageBoardRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.UpdateFormStudentMessageBoard(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to update data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("ValidateEmail")]
        public BaseResponse ValidateEmail(string emailAddress)
        {
            BaseResponse response = new BaseResponse();
            string validatedResult = string.Empty;
            StringBuilder sbProps = new StringBuilder();
            var zeroBounceAPI = new ZeroBounceV2.ZeroBounceAPI();
            var jsonObj = JObject.Parse(System.IO.File.ReadAllText(@"SupportFiles/MycedConfigurations/CSULBCEDConfig.json"));
            zeroBounceAPI.api_key = jsonObj["ZeroBounceAPIKey"]?.ToString();
            zeroBounceAPI.EmailToValidate = emailAddress;
            // zeroBounceAPI.ip_address = "IP Address Where Email Registered From";

            zeroBounceAPI.ReadTimeOut = 100000; // "Any integer value in milliseconds;
            zeroBounceAPI.RequestTimeOut = 100000; // "Any integer value in milliseconds;

            var apiProperties = zeroBounceAPI.ValidateEmail();
            if (apiProperties != null)
            {
                PropertyInfo[] properties = apiProperties.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //check if the the status is catch-all then return valid as status
                    if (property.Name == "status" && apiProperties.status == "catch-all")
                    {
                        if ((!string.IsNullOrEmpty(apiProperties.firstName)) && (!string.IsNullOrEmpty(apiProperties.lastName)))
                        {
                            sbProps.Append(property.Name + ": " + "valid" + "\n");
                            response.IsSuccess = true;
                        }
                        else
                        {
                            sbProps.Append(property.Name + ": " + property.GetValue(apiProperties) + "\n");
                            response.IsSuccess = false;
                        }
                    }
                    else if (apiProperties.error != null)
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        sbProps.Append(property.Name + ": " + property.GetValue(apiProperties) + "\n");
                        if (property.Name == "status" && apiProperties.status == "valid")
                        {
                            response.IsSuccess = true;
                        }
                    }
                }
            }

            return response;
        }

        [HttpPost("UpsertFormExperienceAttachment")]
        public FormExperienceAttachmentResponse UpsertFormExperienceAttachment(FormExperienceAttachmentRequest input)
        {
            try
            {
                #region commented area to pull the file content 
                //string filepath = "D:\\ExcelDoc\\CTC_10232024162937.pdf";
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\timelog from S4.pdf";
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpeg";
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\MyDOC.docx";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                #endregion
                FormExperienceAttachmentResponse response = _initialCredentialProgramService.UpsertFormExperienceAttachment(input);
                return response;
            }
            catch (Exception ex)
            {
                FormExperienceAttachmentResponse response = new FormExperienceAttachmentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("DownloadFormExperienceAttachment")]
        public IActionResult DownloadFormExperienceAttachment(string GUID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                ThoughtFocus.Domain.Request.InitialCredentialProgram.FormAttachments obj = _initialCredentialProgramService.DownloadFormExperienceAttachment(GUID);
                fileName = obj.Filename;
                inputStream = obj.FileContent;
                string[] fileSplit = obj.Filename.Split('.');
                string fileextension = obj.Filename.Split('.').Last();
                fileType = GetFileType(fileextension);
                return File(inputStream, fileType, fileName);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetFormAttachment")]
        public FormDocumentResponse GetFormAttachment(int userId, int formID, int programID)
        {
            try
            {
                FormDocumentResponse response = _graduateProgramService.GetFormAttachment(userId, formID, programID);
                return response;
            }
            catch (Exception ex)
            {
                FormDocumentResponse response = new FormDocumentResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertFormDocument")]
        public BaseResponse UpsertFormDocument(FormDocumentRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpsertFormDocument(input);
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
        [HttpPost("UpdateFormDocumentValidation")]
        public BaseResponse UpdateFormDocumentValidation(FormDocumentValidationRequest input)
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
                BaseResponse response = _graduateProgramService.UpdateFormDocumentValidation(input);
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

        //[HttpPost("UpdateRecommendation")]
        //public BaseResponse UpdateRecommendation(UpdateRecommendation input)
        //{
        //    try
        //    {

        //        BaseResponse response = _graduateProgramService.UpdateRecommendation(input);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseResponse response = new BaseResponse();
        //        response.IsSuccess = false;
        //        response.Message = "Failed to update recommendation data , please try after sometime";
        //        response.StackTrace = ex.Message;
        //        _logger.LogError(ex, ex.Message);
        //        return response;
        //    }
        //}
        //[HttpPost("DeleteRecommendation")]
        //public BaseResponse DeleteRecommendation(DeleteRecommendations input)
        //{
        //    try
        //    {

        //        BaseResponse response = _graduateProgramService.DeleteRecommendation(input);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseResponse response = new BaseResponse();
        //        response.IsSuccess = false;
        //        response.Message = "Failed to delete recommendation data , please try after sometime";
        //        response.StackTrace = ex.Message;
        //        _logger.LogError(ex, ex.Message);
        //        return response;
        //    }
        //}


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
