using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.Admin;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private ILogger<AdminController> _logger;
        private IAdminService _adminService;
        private readonly IConfiguration _configuration;
        public AdminController(IAdminService adminService,
              ILogger<AdminController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _adminService = adminService;
        }

        [HttpGet("GetRolesList")]
        public RolesListResponse GetRolesList()
        {
            try
            {
                RolesListResponse response = _adminService.GetRolesList();
                return response;
            }
            catch (Exception ex)
            {
                RolesListResponse response = new RolesListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetUsersList")]
        public UserListResponse GetUsersList(string searchString,int RoleID)
        {
            try
            {
                UserListResponse response = _adminService.GetUsersList(searchString,RoleID);
                return response;
            }
            catch (Exception ex)
            {
                UserListResponse response = new UserListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetUserOptionList")]
        public UserDataOptionListResponse GetUserOptionList(int UserID)
        {
            try
            {
                UserDataOptionListResponse response = _adminService.GetUserOptionList(UserID);
                return response;
            }
            catch (Exception ex)
            {
                UserDataOptionListResponse response = new UserDataOptionListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetUser")]
        public UserDetailResponse GetUser(int UserID)
        {
            try
            {
                UserDetailResponse response = _adminService.GetUser(UserID);
                return response;
            }
            catch (Exception ex)
            {
                UserDetailResponse response = new UserDetailResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetCommunityDistrictList")]
        public CommunityDistrictListResponse GetCommunityDistrictList()
        {
            try
            {
                CommunityDistrictListResponse response = _adminService.GetCommunityDistrictList();
                return response;
            }
            catch (Exception ex)
            {
                CommunityDistrictListResponse response = new CommunityDistrictListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetCommunitySchoolList")]
        public CommunitySchoolListResponse GetCommunitySchoolList()
        {
            try
            {
                CommunitySchoolListResponse response = _adminService.GetCommunitySchoolList();
                return response;
            }
            catch (Exception ex)
            {
                CommunitySchoolListResponse response = new CommunitySchoolListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("DownloadMessageBoardAttachment")]
        public IActionResult DownloadMessageBoardAttachment(string fileName)
        {
            try
            {
                string fileType = string.Empty;
                string attFileName = string.Empty;
                DownloadMessageBoardAttachment response = _adminService.DownloadMessageBoardAttachment(fileName);
                return File(response.FileContent, "application/pdf", fileName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetUsersByProgramRole")]
        public GetUsersByProgramRoleResponse GetUsersByProgramRole(int ProgramID,int RoleID)
        {
            try
            {
                GetUsersByProgramRoleResponse response = _adminService.GetUsersByProgramRole(ProgramID,RoleID);
                return response;

            }
            catch (Exception ex)
            {
                GetUsersByProgramRoleResponse response = new GetUsersByProgramRoleResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("AssignUserToRole")]
        public BaseResponse AssignUserToRole(AssignUserToRoleRequest input)
        {
            try
            {
                BaseResponse response = _adminService.AssignUserToRole(input);
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

        [HttpPost("AddUser")]
        public BaseResponse AddUser(AddUserRequest input)
        {
            try
            {
                BaseResponse response = _adminService.AddUser(input);
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

        [HttpPost("UpdateUser")]
        public BaseResponse UpdateUser(UpdateUserRequest input)
        {
            try
            {
                BaseResponse response = _adminService.UpdateUser(input);
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
        [HttpPost("AssignUsersToProgram")]
        public BaseResponse AssignUsersToProgram(AssignUsersToProgramRequest input)
        {
            try
            {
                BaseResponse response = _adminService.AssignUsersToProgram(input);
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

        [HttpPost("UploadMessageBoardAttachment")]
        public BaseResponse UploadMessageBoardAttachment(UploadMessageBoardAttachmentRequest input)
        {
            try
            {
                BaseResponse response = _adminService.UploadMessageBoardAttachment(input);
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

        [HttpPost("UpsertCommunityDistrict")]
        public BaseResponse UpsertCommunityDistrict(UpsertCommunityDistrictRequest input)
        {
            try
            {
                BaseResponse response = _adminService.UpsertCommunityDistrict(input);
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

        [HttpPost("UpsertCommunitySchool")]
        public BaseResponse UpsertCommunitySchool(UpsertCommunitySchoolRequest input)
        {
            try
            {
                BaseResponse response = _adminService.UpsertCommunitySchool(input);
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
