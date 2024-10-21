using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response.Application;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        public ILogger<ApplicationController> _logger;
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService
                                    ,ILogger<ApplicationController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpGet("GetApplicationList")]
        public IActionResult GetApplicationList(int userId)
        {
            try
            {
                #region Below code is to pull the claims from token , currently pulling just the UserID -------------------
                //var user = User as ClaimsPrincipal;
                //string userIdFromToken = user.Claims.Where(c => c.Type == "UserID")
                //    .Select(x => x.Value).FirstOrDefault();
                // gets the application list  
                #endregion

                List<ApplicationListResponse> response = _applicationService.GetApplications(userId);
                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
            
        }

        [HttpGet("GetStudentNotification")]
        public IActionResult GetStudentNotification(string CSULBID)
        {
            try
            {
                StudentNotificationResponse response = _applicationService.GetStudentNotification(CSULBID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        //[HttpPost("SaveStudentAggrement")]
        //public BaseResponse SaveStudentAggrement(SaveStudentAggrementRequest input)
        //{
        //    try
        //    {
        //        BaseResponse response = new BaseResponse();
        //        response = _studentProfileService.SaveStudentAggrement(input);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        BaseResponse response = new BaseResponse();
        //        response.IsSuccess = false;
        //        response.Message = "Failed to save data , please try after sometime";
        //        response.StackTrace = ex.Message;
        //        _logger.LogError(ex, ex.Message);
        //        return response;
        //    }
        //}
        [HttpGet("GetApplicationsForAdminPanel")]
        public IActionResult GetApplicationsForAdminPanel(int userId)
        {
            try
            {
                List<ApplicationProgram> response = _applicationService.GetApplicationsForAdminPanel(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
    }
}
