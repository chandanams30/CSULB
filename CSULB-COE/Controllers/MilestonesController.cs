using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MilestonesController : ControllerBase
    {
        private ILogger<MilestonesController> _logger;
        private IMilestonesService _milestonesService;
        private readonly IConfiguration _configuration;
        public MilestonesController(IMilestonesService milestonesService,
              ILogger<MilestonesController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _milestonesService = milestonesService;
        }
        [HttpGet("GetMilestoneList")]
        public MilestonesListResponse GetMilestoneList()
        {
            try
            {
                MilestonesListResponse response = _milestonesService.GetMilestoneList();
                return response;
            }
            catch (Exception ex)
            {
                MilestonesListResponse response = new MilestonesListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestonesByProgramTerm")]
        public PublishedMilestonesListResponse GetPublishedMilestoneList(int ProgramID, string TermCode)
        {
            try
            {
                PublishedMilestonesListResponse response = _milestonesService.GetPublishedMilestoneList(ProgramID,TermCode);
                return response;
            }
            catch (Exception ex)
            {
                PublishedMilestonesListResponse response = new PublishedMilestonesListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        
        [HttpGet("GetMilestoneApplicationFormsList")]
        public MilestoneApplicationFormsListResponse GetMilestoneApplicationFormsList(int UserID,int FormID, int ProgramID, string TermCode)
        {
            try
            {
                MilestoneApplicationFormsListResponse response = _milestonesService.GetMilestoneApplicationFormsList(UserID, FormID, ProgramID, TermCode);
                return response;
            }
            catch (Exception ex)
            {
                MilestoneApplicationFormsListResponse response = new MilestoneApplicationFormsListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        
        [HttpGet("GetMilestoneApplicationForm")]
        public GetMilestoneApplicationFormResponse GetMilestoneApplicationForm(int UserID,int MilestoneFormID, int FormID, int MilestonePublishedFormID,bool IsReApply)
        {
            try
            {
                GetMilestoneApplicationFormResponse response = _milestonesService.GetMilestoneApplicationForm(UserID,MilestoneFormID, FormID,MilestonePublishedFormID,IsReApply);
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

        [HttpGet("GetMilestone")]
        public GetMilestoneResponse GetMilestone(int MilestoneID)
        {
            try
            {
                GetMilestoneResponse response = _milestonesService.GetMilestone(MilestoneID);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneResponse response = new GetMilestoneResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneApproverUserList")]
        public MilestoneApproverUserListResponse GetMilestoneApproverUserList()
        {
            try
            {
                MilestoneApproverUserListResponse response = _milestonesService.GetMilestoneApproverUserList();
                return response;
            }
            catch (Exception ex)
            {
                MilestoneApproverUserListResponse response = new MilestoneApproverUserListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpsertMilestone")]
        public BaseResponse UpsertMilestone(UpsertMilestoneRequest input)
        {
            try
            {
                BaseResponse response = _milestonesService.UpsertMilestone(input);
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

        [HttpPost("AssignMilestoneToProgram")]
        public BaseResponse AssignMilestoneProgram(AssignMilestoneToProgramRequest input)
        {
            try
            {
                BaseResponse response=new BaseResponse();
                foreach (AssignMilestoneToProgram amp in input.AssignMilestones)
                {
                     BaseResponse appResponse = _milestonesService.AssignMilestoneProgram(amp);
                }
                response.IsSuccess = true;
                response.Message = "Data Saved Successfully";
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

        [HttpGet("GetMilestoneTypes")]
        public MilestoneTypesResponse GetMilestoneTypes()
        {
            try
            {
                MilestoneTypesResponse response = _milestonesService.GetMilestoneTypes();
                return response;
            }
            catch (Exception ex)
            {
                MilestoneTypesResponse response = new MilestoneTypesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpsertMilestoneTemplate")]
        public BaseResponse UpsertMilestoneTemplate(UpsertMilestoneTemplateRequest input)
        {
            try
            {
                BaseResponse response = _milestonesService.UpsertMilestoneTemplate(input);
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
        [HttpGet("GetMilestoneTemplateList")]
        public MilestoneTemplateListResponse GetMilestoneTemplateList(bool isReadyToPublish)
        { 
            try
            {
                MilestoneTemplateListResponse response = _milestonesService.GetMilestoneTemplateList(isReadyToPublish);
                return response;
            }
            catch (Exception ex)
            {
                MilestoneTemplateListResponse response = new MilestoneTemplateListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneTemplateByID")]
        public MilestoneTemplateByIDResponse GetMilestoneTemplateByID(int MilestoneTemplateID)
        {
            try
            {
                MilestoneTemplateByIDResponse response = _milestonesService.GetMilestoneTemplateByID(MilestoneTemplateID);
                return response;
            }
            catch (Exception ex)
            {
                MilestoneTemplateByIDResponse response = new MilestoneTemplateByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetMilestoneApproverTypes")]
        public MilestonesApproverTypesResponse GetMilestoneApproverTypes()
        {
            try
            {
                MilestonesApproverTypesResponse response = _milestonesService.GetMilestoneApproverTypes();
                return response;
            }
            catch (Exception ex)
            {
                MilestonesApproverTypesResponse response = new MilestonesApproverTypesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("PublishMilestoneForm")]
        public BaseResponse PublishMilestoneForm(PublishMilestoneFormRequest input)
        {
            try
            {
                BaseResponse response = _milestonesService.PublishMilestoneForm(input);
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

        [HttpGet("GetMilestoneUsersList")]
        public MilestoneUsersListResponse GetMilestoneUsersList(int RoleID, int ProgramID, string TermCode)
        {
            try
            {
                MilestoneUsersListResponse response = _milestonesService.GetMilestoneUsersList(RoleID,ProgramID,TermCode);
                return response;
            }
            catch (Exception ex)
            {
                MilestoneUsersListResponse response = new MilestoneUsersListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneProgramTermList")]
        public MilestoneProgramTermListResponse GetMilestoneProgramTermList()
        {
            try
            {
                MilestoneProgramTermListResponse response = _milestonesService.GetMilestoneProgramTermList();
                return response;
            }
            catch (Exception ex)
            {
                MilestoneProgramTermListResponse response = new MilestoneProgramTermListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestonePublishedFormsList")]
        public MilestonePublishedFormsListResponse GetMilestonePublishedFormsList(int UserID)
        {
            try
            {
                MilestonePublishedFormsListResponse response = _milestonesService.GetMilestonePublishedFormsList(UserID);
                return response;
            }
            catch (Exception ex)
            {
                MilestonePublishedFormsListResponse response = new MilestonePublishedFormsListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("FetchMilestoneUsersList")]
        public GetMilestoneUsersListResponse GetMilestoneUsersList()
        {
            try
            {
                GetMilestoneUsersListResponse response = _milestonesService.GetMilestoneUsersList();
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneUsersListResponse response = new GetMilestoneUsersListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneFilledFormByUserList")]
        public GetMilestoneFilledFormByUserListResponse GetMilestoneFilledFormByUserList(int UserID)
        {
            try
            {
                GetMilestoneFilledFormByUserListResponse response = _milestonesService.GetMilestoneFilledFormByUserList(UserID);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneFilledFormByUserListResponse response = new GetMilestoneFilledFormByUserListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneFilledFormByPublishedFormList")]
        public GetMilestoneFilledFormByPublishedFormListResponse GetMilestoneFilledFormByPublishedFormList(int MilestonePublishedFormID, int UserID)
        {
            try
            {
                GetMilestoneFilledFormByPublishedFormListResponse response = _milestonesService.GetMilestoneFilledFormByPublishedFormList(MilestonePublishedFormID,UserID);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneFilledFormByPublishedFormListResponse response = new GetMilestoneFilledFormByPublishedFormListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetMilestoneRequirementList")]
        public GetMilestoneRequirementListResponse GetMilestoneRequirementList()
        {
            try
            {
                GetMilestoneRequirementListResponse response = _milestonesService.GetMilestoneRequirementList();
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneRequirementListResponse response = new GetMilestoneRequirementListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetMilestoneSubmittedFormsList")]
        public GetMilestoneSubmittedFormsListResponse GetMilestoneSubmittedFormsList(int RoleID, int ApproverUserID)
        {
            try
            {
                GetMilestoneSubmittedFormsListResponse response = _milestonesService.GetMilestoneSubmittedFormsList(RoleID,ApproverUserID);
                return response;
            }
            catch (Exception ex)
            {
                GetMilestoneSubmittedFormsListResponse response = new GetMilestoneSubmittedFormsListResponse();
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
                GetMilestoneWorkflowProcessTransitionHistoryResponse response = _milestonesService.GetMilestoneWorkflowProcessTransitionHistory(MilestoneFormID,RoleID);
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
        [HttpGet("GetApplicationProgramsByTermCode")]
        public ApplicationProgramsResponse GetApplicationProgramsByTermCode(string termCode)
        {
            try
            {
                ApplicationProgramsResponse response = _milestonesService.GetApplicationProgramsByTermCode(termCode);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationProgramsResponse response = new ApplicationProgramsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetDistinctSemesterList")]
        public SemesterListResponse GetDistinctSemesterList()
        {
            try
            {
                SemesterListResponse response = _milestonesService.GetDistinctSemesterList();
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

                FormAttachments obj = _milestonesService.DownloadMilestoneFormAttachments(FileName);
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
        [HttpGet("GetPublishedMilestoneDetails")]
        public PublishedMilestoneDetailsResponse GetPublishedMilestoneDetails(int MilestoneTemplateID)
        {
            try
            {
                PublishedMilestoneDetailsResponse response = _milestonesService.GetPublishedMilestoneDetails(MilestoneTemplateID);
                return response;
            }
            catch (Exception ex)
            {
                PublishedMilestoneDetailsResponse response = new PublishedMilestoneDetailsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
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
