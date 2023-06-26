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
using ThoughtFocus.Domain.Request.Login;
using ThoughtFocus.Domain.Response;
using Microsoft.AspNetCore.Http.Features;

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
                if (response.IsSuccess == true)
                {
                    AuditLogRequest req = new AuditLogRequest();
                    req.UserID = response.UserId;
                    req.Type = "Login";
                    req.IPAddress = GetClientIPAddress();
                    req.AuthenticationType = "Basic";
                    var auditResponse = _userLoginService.SaveAuditLog(req);

                }
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
            AuthenticateResponse response = new AuthenticateResponse();
            try
            {
                _logger.LogInformation(request.Token.ToString(), request);
                string accessToken = request.Token;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");
                string emplid = string.Empty;
                string displayName = string.Empty;
                string mail = string.Empty;
                string LastName = string.Empty;
                string FirstName = string.Empty;
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
                        }
                        if (data.Name == "displayName")
                        {
                            displayName = data.Value;
                        }
                        if (data.Name == "mail")
                        {
                            mail = data.Value;
                        }
                        if (data.Name == "surname")
                        {
                            LastName = data.Value;
                        }
                        if (data.Name == "givenName")
                        {
                            FirstName = data.Value;
                        }
                    }
                    response = _userLoginService.AuthenticateSSO(emplid,displayName,mail,LastName,FirstName);
                    if (response.IsSuccess == true)
                    {
                        AuditLogRequest req = new AuditLogRequest();
                        req.UserID = response.UserId;
                        req.Type = "Login";
                        req.IPAddress=GetClientIPAddress();
                        req.AuthenticationType = "SSO";
                        var auditResponse = _userLoginService.SaveAuditLog(req);

                    }
                    return Ok(response);
                }
                else
                {

                    response.IsSuccess = false;
                    response.message = "Authentication Failed";
                    return BadRequest(response);
                }
                   
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        #region AuthenticateSSO old method 
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
        #endregion

        [HttpPost("SaveUserRegistration")]
        public BaseResponse SaveUserRegistration([FromBody] LoginUserRegistrationRequest request)
        {
            try
            {
                var response = _userLoginService.SaveUserRegistration(request);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("SaveAuditLog")]
        public BaseResponse SaveAuditLog(AuditLogRequest request)
        {
            try
            {
                var response = _userLoginService.SaveAuditLog(request);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        private string GetClientIPAddress()
        {
            var ip = string.Empty;
            ip= Convert.ToString(Request.HttpContext.Connection.RemoteIpAddress);  
            return ip.ToString();

        }
    }
}
