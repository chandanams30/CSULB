using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InitialCredentialProgramController : ControllerBase
    {
        public ILogger<InitialCredentialProgramController> _logger;
        public IGraduateProgramService _graduateProgramService;
        public IInitialCredentialProgramService _initialCredentialProgramService;
        private readonly IConfiguration _configuration;
        public InitialCredentialProgramController(IInitialCredentialProgramService initialCredentialProgramService
                                                 , IGraduateProgramService graduateProgramService
                                                 , ILogger<InitialCredentialProgramController> logger
                                                , IConfiguration configuration)
        {
            _logger = logger;
            _graduateProgramService = graduateProgramService;
            _initialCredentialProgramService = initialCredentialProgramService;
            _configuration = configuration;
        }
        [HttpGet("GetApplicationPrograms")]
        public ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode)
        {
            try
            {
                ApplicationProgramResponse response = _initialCredentialProgramService.GetApplicationPrograms(userID, applicationTypeID, termCode);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationProgramResponse response = new ApplicationProgramResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetAppliedFormsByPrograms")]
        public AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termCode, int formStateID)
        {
            try
            {
                AppliedFormsByProgramsResponse response = _initialCredentialProgramService.GetAppliedFormsByPrograms(userID, programID, termCode, formStateID);
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

        [HttpGet("GetAppliedForms")]
        public AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID)
        {
            try
            {
                AppliedFormsResponse response = _initialCredentialProgramService.GetAppliedForms(userID, applicationTypeID);
                return response;
            }
            catch (Exception ex)
            {
                AppliedFormsResponse response = new AppliedFormsResponse();
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
                FormStatesResponse response = _initialCredentialProgramService.GetFormStates(userID);
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
                SemesterListResponse response = _initialCredentialProgramService.GetSemesterList();
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

        [HttpPost("UpsertFormEducationInformationAttachment")]
        public UpsertFormEducationInformationAttachmentResponse UpsertFormEducationInformationAttachment(UpsertFormEducationInformationAttachmentRequest input)
        {
            try
            {
                UpsertFormEducationInformationAttachmentResponse response = new UpsertFormEducationInformationAttachmentResponse();

                #region to get the file content from local
                //byte[] fileContent = null;
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\test3.pdf";
                //string filepath = "D:\\ExcelDoc\\CTC_10232024162937.pdf";
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
        [HttpPost("UpdateFormSubSectionApproveral")]
        public BaseResponse UpdateFormSubSectionApproveral(UpdateFormSubSectionApproveralRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _initialCredentialProgramService.UpdateFormSubSectionApproveral(input);
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
        [HttpPost("UpdateAdditionalOfficialDocument")]
        public BaseResponse UpdateAdditionalOfficialDocument(UpdateAdditionalOfficialDocumentRequest input)
        {
            #region Get file content
            //byte[] fileContent = null;
            //string filepath = "D:\\CSULB\\Document\\test.pdf";
            //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            //long byteLength = new System.IO.FileInfo(filepath).Length;
            //fileContent = binaryReader.ReadBytes((Int32)byteLength);
            //fs.Close();
            //fs.Dispose();
            //binaryReader.Close();
            //input.FileContent = fileContent;
            #endregion

            try
            {
                BaseResponse response = _initialCredentialProgramService.UpdateAdditionalOfficialDocument(input);
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
        [HttpPost("GetAdditionalOfficialDocument")]
        public IActionResult GetAdditionalOfficialDocument(GetAdditionalOfficialDocumentRequest input)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                ThoughtFocus.Domain.Request.InitialCredentialProgram.FormAttachments obj = _initialCredentialProgramService.GetAdditionalOfficialDocument(input);
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
        [HttpPost("DeleteAdditionalOfficialDocument")]
        public BaseResponse DeleteAdditionalOfficialDocument(DeleteAdditionalOfficialDocumentRequest input)
        {
            try
            {

                BaseResponse response = _initialCredentialProgramService.DeleteAdditionalOfficialDocument(input);
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
        [AllowAnonymous]
        [HttpGet("GetLetterOfRecommendationsByRecommenderIdentifier")]
        public LetterOfRecommendationsByRecommenderIdentifierResponse GetLetterOfRecommendationsByRecommenderIdentifier(string recommenderIdentifier)
        {
            try
            {

                LetterOfRecommendationsByRecommenderIdentifierResponse response = _initialCredentialProgramService.GetLetterOfRecommendationsByRecommenderIdentifier(recommenderIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                LetterOfRecommendationsByRecommenderIdentifierResponse response = new LetterOfRecommendationsByRecommenderIdentifierResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [AllowAnonymous]
        [HttpPost("UpdateLetterOfRecommendationsJSON")]
        public BaseResponse UpdateLetterOfRecommendationsJSON(UpdateLetterOfRecommendationsJSONRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _initialCredentialProgramService.UpdateLetterOfRecommendationsJSON(input);
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
        [HttpPost("DeleteSubSectionAttachment")]
        public BaseResponse DeleteSubSectionAttachments(DeleteSubSectionAttachmentRequest input)
        {
            try
            {
                BaseResponse response = _initialCredentialProgramService.DeleteSubSectionAttachments(input);
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
        [HttpGet("GetClinicalPracticeEquivalencyAttachmentList")]
        public ClinicalPracticeEquivalencyAttachmentList GetClinicalPracticeEquivalencyAttachmentList(int UserID,int FormID)
        {
            try
            {
                ClinicalPracticeEquivalencyAttachmentList response = _initialCredentialProgramService.GetClinicalPracticeEquivalencyAttachmentList(UserID,FormID);
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
        [HttpPost("UpdateAdmissionRequirementsMailBody")]
        public BaseResponse UpdateAdmissionRequirementsMailBody(AdmissionRequirementsBody input)
        {
            try
            {
                BaseResponse response = _initialCredentialProgramService.UpdateAdmissionRequirementsMailBody(input);
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
        [HttpGet("GetAdmissionRequirementsMailBody")]
        public AdmissionRequirementsBodyResponse GetAdmissionRequirementsMailBody(int programID,string sectionName,string categoryName, string identifier,string name)
        {
            try
            {
                AdmissionRequirementsBodyResponse response = _initialCredentialProgramService.GetAdmissionRequirementsMailBody(programID,sectionName,categoryName,identifier,name);
                return response;
            }
            catch (Exception ex)
            {
                AdmissionRequirementsBodyResponse response = new AdmissionRequirementsBodyResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

    }
}
