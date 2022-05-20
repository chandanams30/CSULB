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

        [HttpGet("Authenticate")]
        //public IActionResult Login ([FromBody]Models.AuthenticateRequest model) // uncomment after testing
        public IActionResult Login(string userName,string password)
        {
            //_logger.LogInformation("Start : Authenticating for {userName}",userName);
            ViewModels.AuthenticateRequest authModel = new ViewModels.AuthenticateRequest();
            authModel.Username = userName;
            authModel.Password = password;
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
