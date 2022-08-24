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
    }
}
