using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GraduateProgramController : ControllerBase
    {
        public ILogger<GraduateProgramController> _logger;
        public IGraduateProgramService _graduateProgramService;
        private readonly IConfiguration _configuration;
        public IStudentProfile _studentProfileService;
        public GraduateProgramController(IGraduateProgramService graduateProgramService ,
              ILogger<GraduateProgramController> logger,
              IConfiguration configuration,
              IStudentProfile studentProfileService)
        {
            _logger = logger;
            _graduateProgramService = graduateProgramService;
            _configuration = configuration;
            _studentProfileService = studentProfileService;
        }
        [HttpGet("GetApplicationPrograms")]
        public ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID,string termCode)
        {
            try
            {
                ApplicationProgramResponse response = _graduateProgramService.GetApplicationPrograms(userID, applicationTypeID,termCode);
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

        [HttpGet("GetAppliedForms")]
        public AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID)
        {
            try
            {
                AppliedFormsResponse response = _graduateProgramService.GetAppliedForms(userID, applicationTypeID);
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

        [HttpPost("UpdatePersonalInfoSchema")]
        public BaseResponse UpdatePersonalInfoSchema(FormPersonalInfoSchemaRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpdatePersonalInfoSchema(input);
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

        [HttpPost("UpdateMessageBoardSchema")]
        public BaseResponse UpdateMessageBoardSchema(FormMessageBoardSchema input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpdateMessageBoardSchema(input);
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


        [HttpPost("UpsertFormAttachment")]
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
                BaseResponse response = _graduateProgramService.UpsertFormAttachment(input);
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

        [HttpPost("DeleteInsructorAttachment")]
        public BaseResponse DeleteInsructorAttachment(DeleteInsructorAttachmentRequest input)
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
                BaseResponse response = _graduateProgramService.DeleteInsructorAttachment(input);
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

        [HttpPost("AddRecommender")]
        public BaseResponse AddRecommender(FormAddRecommenderRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AddRecommender(input);
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

                FormAttachments obj = _graduateProgramService.DownloadFormAttachments(userID, formattachmentID);
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

        [HttpGet("GetFormRecommendations")]
        public IActionResult GetFormRecommendations(int userID, int recommendationAttachmentID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                RecommendationAttachments obj = _graduateProgramService.GetFormRecommendations(userID, recommendationAttachmentID);
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
        [HttpPost("UpdateReviwerReview")]
        public BaseResponse UpdateReviwerReview(FormReviewerReviewRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.UpdateReviwerReview(input);
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
        [HttpGet("AssignFormToReviewers")]
        public BaseResponse AssignFormToReviewers(int programID)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AssignFormToReviewers(programID);
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

        [HttpGet("GetMergedDocument")]
        public IActionResult GetMergedDocument(int formID)
        {
            Byte[] InputStream = null;
            //string documentName = string.Empty;
            //InputStream = _documentService.GetMergedDocument(userId);
             InputStream = _graduateProgramService.GetMergedDocument(formID);
            string documentName = string.Empty;
            documentName = "MergedDocument";
            return File(InputStream, "application/pdf;", documentName + ".pdf");

        }

        [HttpPost("UpdateInstructorFeedback")]
        public BaseResponse UpdateInstructorFeedback(UpdateInstructorFeedbackRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _graduateProgramService.UpdateInstructorFeedback(input);
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

        [HttpPost("UpdateInterviewerFeedback")]
        public BaseResponse UpdateInterviewerFeedback(UpdateInterviewerFeedbackRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.UpdateInterviewerFeedback(input);
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


        [HttpPost("AddInterviewerToForm")]
        public BaseResponse AddInterviewerToForm(AddInterviewerRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AddInterviewerToForm(input);
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

        [HttpPost("AddReviewerToForm")]
        public BaseResponse AddReviewerToForm(AddReviewerRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AddReviewerToForm(input);
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

        [HttpPost("RemoveReviewerFromForm")]
        public BaseResponse RemoveReviewerFromForm(AddReviewerRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.RemoveReviewerFromForm(input);
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
        [HttpPost("GetInterviewerList")]
        public InterviewerListResponse GetInterviewerList(GetInstructorInterviewerListRequest input)
        {
            try
            {
                InterviewerListResponse response = new InterviewerListResponse();
                response = _graduateProgramService.GetInterviewerList(input);
                return response;
            }
            catch (Exception ex)
            {
                InterviewerListResponse response = new InterviewerListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("GetReviewerList")]
        public ReviewerListResponse GetReviewerList(GetReviewerListRequest input)
        {
            try
            {
                ReviewerListResponse response = new ReviewerListResponse();
                response = _graduateProgramService.GetReviewerList(input);
                return response;
            }
            catch (Exception ex)
            {
                ReviewerListResponse response = new ReviewerListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetInstructionAttachment")]
        public IActionResult GetInstructionAttachment(int UserID,int InstructionAttachmentID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                InstructorAttachments obj = _graduateProgramService.GetInstructionAttachment(UserID, InstructionAttachmentID);
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

        [HttpGet("GetInterviewAttachments")]
        public IActionResult GetInterviewAttachments(int UserID, int InterviewAttachmentID)
        {
            try
            {
                byte[] inputStream = null;
                string fileType = string.Empty;
                string fileName = string.Empty;

                InterviewerAttachments obj = _graduateProgramService.GetInterviewAttachments(UserID, InterviewAttachmentID);
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

        [HttpPost("SaveFormGridNotes")]
        public BaseResponse SaveFormGridNotes(FormSaveGridNotesRequest input)
        {
            try
            {

                BaseResponse response = _graduateProgramService.SaveFormGridNotes(input);
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
        [HttpGet("AuthorizeRecommender")]
        public AuthorizeRecommenderResponse GetDetailsForRecommendation(string recommenderIdentifier)
        {
            try
            {
                AuthorizeRecommenderResponse response = new AuthorizeRecommenderResponse();
                response = _graduateProgramService.GetDetailsForRecommendation(recommenderIdentifier);
                return response;
            }
            catch (Exception ex)
            {
                AuthorizeRecommenderResponse response = new AuthorizeRecommenderResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }


        [AllowAnonymous]
        [HttpPost("AddRecommendation")]
        public BaseResponse AddRecommendation(FormAddRecommendationRequest input)
        {
            try
            {
                BaseResponse response = new BaseResponse();
                response = _graduateProgramService.AddRecommendation(input);
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
        [HttpPost("AddRecommendationForm")]
        public BaseResponse AddRecommendationForm(FormAddRecommendation input)
        {
            try
            {
                BaseResponse response = new BaseResponse();

                response = _graduateProgramService.AddRecommendationForm(input);
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
        [HttpGet("GetLatestWaitlistNumber")]
        public LatestWaitlistNumberResponse GetLatestWaitlistNumber(string TermCode, int ProgramID,int FormID)
        {
            try
            {
                LatestWaitlistNumberResponse response = new LatestWaitlistNumberResponse();
                response = _graduateProgramService.GetLatestWaitlistNumber(TermCode,ProgramID, FormID);
                return response;
            }
            catch (Exception ex)
            {
                LatestWaitlistNumberResponse response = new LatestWaitlistNumberResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("BulkOfferNotOfferUpdateFormState")]
        public BaseResponse BulkOfferNotOfferUpdateFormState(BulkNotOfferFormStatusUpdateRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.BulkOfferNotOfferUpdateFormState(input);
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
        [HttpPost("UpdateRecommendation")]
        public BaseResponse UpdateRecommendation(UpdateRecommendation input)
        {
            try
            {

                BaseResponse response = _graduateProgramService.UpdateRecommendation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to update recommendation data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("DeleteRecommendation")]
        public BaseResponse DeleteRecommendation(DeleteRecommendations input)
        {
            try
            {

                BaseResponse response = _graduateProgramService.DeleteRecommendation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to delete recommendation data , please try after sometime";
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


        [HttpPost("SendMailtoPendingRecommendations")]
        public AdhocMailLogResponse SendMailtoPendingRecommendations(PendingRecommendationsRequest input)
        {
            try
            {
                AdhocMailLogResponse response = _graduateProgramService.SendNotificationforPendingRecommendations(input);

                return response;
            }
            catch (Exception ex)
            {
                AdhocMailLogResponse response = new AdhocMailLogResponse();
                response.IsSuccess = false;
                response.Message = "There is an error while sending the notifications, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateProgramApplicationDates")]
        public BaseResponse UpdateProgramApplicationDates(UpdateProgramApplicationDatesRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpdateProgramApplicationDates(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to update program application dates , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetProgramApplicationDates")]
        public ProgramApplicationDates GetProgramApplicationDates(int programID, string termCode)
        {
            try
            {
                ProgramApplicationDates response = new ProgramApplicationDates();
                response = _graduateProgramService.GetProgramApplicationDates(programID, termCode);
                return response;
            }
            catch (Exception ex)
            {
                ProgramApplicationDates response = new ProgramApplicationDates();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetApplicationProgramsforDates")]
        public ApplicationProgramResponse GetApplicationProgramsforDates(int userID, int applicationTypeID, string termCode)
        {
            try
            {
                ApplicationProgramResponse response = _graduateProgramService.GetApplicationProgramsforDates(userID, applicationTypeID, termCode);
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
        [HttpGet("RevertBackToPreviousState")]
        public BaseResponse RevertBacktoPreviousState(int formID)
        {
            try
            {

                BaseResponse response = _graduateProgramService.RevertBacktoPreviousState(formID);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Revert Back to Previous State , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("MoveApplicationToSemester")]
        public BaseResponse MoveApplicationToSemester(MoveApplicationToSemesterRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.MoveApplicationToSemester(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to update semester , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetApplicationList")]
        public IActionResult GetApplicationList(int userId)
        {
            try
            {
                List<ApplicationList> response = _studentProfileService.GetApplications(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        [HttpGet("GetApplicationProgramList")]
        public ApplicationProgramsListResponse GetApplicationProgramList(int applicationId)
        {
            try
            {
                ApplicationProgramsListResponse response = _graduateProgramService.GetApplicationProgramList(applicationId);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationProgramsListResponse response = new ApplicationProgramsListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetRecommenderMailBody")]
        public RecommenderMailBodyResponse GetRecommenderMailBody(int applicationId,int programID)
        {
            try
            {
                RecommenderMailBodyResponse response = _graduateProgramService.GetRecommenderMailBody(applicationId,programID);
                return response;
            }
            catch (Exception ex)
            {
                RecommenderMailBodyResponse response = new RecommenderMailBodyResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateRecommenderMailBody")]
        public BaseResponse UpdateRecommenderMailBody(RecommenderBody input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpdateRecommenderMailBody(input);
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
        [HttpPost("UpsertDecisionLetters")]
        public BaseResponse UpsertDecisionLetters(DecisionLettersRequest input)
        {
            try
            {
                //input.MailBody = "<p>Dear [[ApplicantName]],</p><p>Your program application materials to the [[programName]] has been not offered.</p><p>You may track the status of your official transcripts and university application at <a href=\"http://www.csulb.edu/admissions/applicant-self-service\" target=\"_blank\">CSULB Applicant Self Service</a>.</p><p>Please let us know if you have any questions or concerns. We are here to help. </p><p>Warm regards</P><p>CSULB College of Education Graduate Studies Office<br/><a href=\"http://www.csulb.edu/ced/graduate\" target=\"_blank\">www.csulb.edu/ced/graduate</a><br/><a href=\"mailto:ced-gradstudies@csulb.edu\">ced-gradstudies@csulb.edu</a><br/>(562) 985-8476</p>";
                BaseResponse response = _graduateProgramService.UpsertDecisionLetters(input);
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
        [HttpGet("GetDecisionLetters")]
        public DecisionLettersResponse GetDecisionLetters(string programIdentifier, string offeredCategories,string decisionType)
        {
            try
            {
                DecisionLettersResponse response = _graduateProgramService.GetDecisionLetters(programIdentifier,offeredCategories,decisionType);
                return response;
            }
            catch (Exception ex)
            {
                DecisionLettersResponse response = new DecisionLettersResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetDropDownList")]
        public DropDownListResponse GetDropDownList(int programId,string controlLabel)
        {
            try
            {
                DropDownListResponse response = _graduateProgramService.GetDropDownList(programId,controlLabel);
                return response;
            }
            catch (Exception ex)
            {
                DropDownListResponse response = new DropDownListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetControlLabelList")]
        public ControlLabelListResponse GetControlLabelList(int programId)
        {
            try
            {
                ControlLabelListResponse response = _graduateProgramService.GetControlLabelList(programId);
                return response;
            }
            catch (Exception ex)
            {
                ControlLabelListResponse response = new ControlLabelListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertDropDown")]
        public BaseResponse UpsertDropDown(DropDownRequest input)
        {
            try
            {
                BaseResponse response = _graduateProgramService.UpsertDropDown(input);
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
