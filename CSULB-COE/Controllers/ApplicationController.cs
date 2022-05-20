using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response.Application;


namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
