using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using CSULB_COE.ViewModels;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Request;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        public ILogger<LoginController> _logger;
        public LoginController(IUserLoginService userLoginService , ILogger<LoginController> logger)
        {
            _userLoginService = userLoginService;
            _logger = logger;
        }

        [HttpPost("Authenticate")]
        //public IActionResult Login ([FromBody]Models.AuthenticateRequest model) // uncomment after testing // updated testing venky
        public IActionResult Login([FromBody]LoginRequest request)
        {
            //_logger.LogInformation("Start : Authenticating for {userName}",userName);
            ViewModels.AuthenticateRequest authModel = new ViewModels.AuthenticateRequest();
            authModel.Username = request.UserName;
            authModel.Password = request.Password;
            try
            {
                var response = _userLoginService.Authenticate(authModel);
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
