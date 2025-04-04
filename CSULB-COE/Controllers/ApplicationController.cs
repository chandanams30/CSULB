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
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        public ILogger<ApplicationController> _logger;
        private readonly IApplicationService _applicationService;
        public IStudentProfile _studentProfileService;
        private readonly IConfiguration _configuration;
        public ApplicationController(IApplicationService applicationService
                                    ,ILogger<ApplicationController> logger
                                    ,IStudentProfile studentProfileService
                                    , IConfiguration configuration)
        {
            _applicationService = applicationService;
            _logger = logger;
            _studentProfileService = studentProfileService;
            _configuration = configuration;
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
        [HttpGet("ValidateEmail")]
        public BaseResponse ValidateEmail(string emailAddress)
        {
            BaseResponse response = new BaseResponse();
            string validatedResult = string.Empty;
            StringBuilder sbProps = new StringBuilder();
            var zeroBounceAPI = new ZeroBounceV2.ZeroBounceAPI();
            zeroBounceAPI.api_key = _configuration["ApplicationKeys:ZeroBounceAPIKey"];
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
    }
}
