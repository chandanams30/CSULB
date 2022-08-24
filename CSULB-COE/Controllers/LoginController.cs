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
using System.Globalization;
using Owin;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Graph;
using Google.Apis.Json;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        public ILogger<LoginController> _logger;
        private readonly HttpClient _client;
        public LoginController(IUserLoginService userLoginService, ILogger<LoginController> logger, IHttpClientFactory httpClientFactory)
        {
            _userLoginService = userLoginService;
            _logger = logger;
            _client = httpClientFactory.CreateClient();
        }

        [HttpPost("Authenticate")]
        public IActionResult Login([FromBody] LoginRequest request)
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
        [HttpPost("AuthenticateSSOToken")]
        public async Task<IActionResult> AuthenticateSSOToken([FromBody] LoginSSORequest request)
        {
            try
            {
                _logger.LogInformation(request.Token.ToString(), request);
                string accessToken = request.Token;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");
                string emplid = string.Empty;
                if (graphResponse.IsSuccessStatusCode)
                {
                    
                    var graphResults=graphResponse.Content.ReadAsStringAsync().Result;
                    var json = new NewtonsoftJsonSerializer();
                    dynamic dynObj = JsonConvert.DeserializeObject(graphResults);
                    foreach (var data in dynObj)
                    {
                        if (data.Name == "employeeId")
                        {
                            emplid = data.Value;
                            break;
                        }
                    }
                    var response = _userLoginService.AuthenticateSSO(emplid);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Authentication Failed");
                }
                   
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        //[HttpPost("AuthenticateSSO")]
        //public IActionResult AuthenticateSSO(string CSULBID)
        //{
        //    try
        //    {
        //        // logic to connect to graph API 
        //        //string accessToken = CSULBID;
        //        //_client.DefaultRequestHeaders.Clear();
        //        //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        //        //var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");
        //        //_logger.LogInformation(graphResponse.ToString(), graphResponse);
        //        ////string id = Convert.ToString(graphResponse.Id);
        //        //string id = string.Empty;
        //        //var response = _userLoginService.AuthenticateSSO(id);
        //        var response = _userLoginService.AuthenticateSSO(CSULBID);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}

    }
}
