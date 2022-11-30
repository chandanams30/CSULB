using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class InitialCredentialProgramController : ControllerBase
    {
        public ILogger<InitialCredentialProgramController> _logger;
        public IGraduateProgramService _graduateProgramService;
        public IInitialCredentialProgramService _initialCredentialProgramService;
        private readonly IConfiguration _configuration;
        public InitialCredentialProgramController(IInitialCredentialProgramService initialCredentialProgramService
                                                 , IGraduateProgramService graduateProgramService
                                                 , ILogger<InitialCredentialProgramController> logger
                                                , IConfiguration configuration)
        {
            _logger = logger;
            _graduateProgramService = graduateProgramService;
            _initialCredentialProgramService = initialCredentialProgramService;
            _configuration = configuration;
        }
        [HttpGet("GetApplicationPrograms")]
        public ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode)
        {
            try
            {
                ApplicationProgramResponse response = _initialCredentialProgramService.GetApplicationPrograms(userID, applicationTypeID, termCode);
                return response;
            }
            catch (Exception ex)
            {
                ApplicationProgramResponse response = new ApplicationProgramResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetAppliedFormsByPrograms")]
        public AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termCode, int formStateID)
        {
            try
            {
                AppliedFormsByProgramsResponse response = _initialCredentialProgramService.GetAppliedFormsByPrograms(userID, programID, termCode, formStateID);
                return response;
            }
            catch (Exception ex)
            {
                AppliedFormsByProgramsResponse response = new AppliedFormsByProgramsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetAppliedForms")]
        public AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID)
        {
            try
            {
                AppliedFormsResponse response = _initialCredentialProgramService.GetAppliedForms(userID, applicationTypeID);
                return response;
            }
            catch (Exception ex)
            {
                AppliedFormsResponse response = new AppliedFormsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetFormStates")]
        public FormStatesResponse GetFormStates(int userID)
        {
            try
            {
                FormStatesResponse response = _initialCredentialProgramService.GetFormStates(userID);
                return response;
            }
            catch (Exception ex)
            {
                FormStatesResponse response = new FormStatesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetSemesterList")]
        public SemesterListResponse GetSemesterList()
        {
            try
            {
                SemesterListResponse response = _initialCredentialProgramService.GetSemesterList();
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
        [HttpGet("GetIntialCreditialOptionItemsList")]
        public OptionItemListResponse GetIntialCreditialOptionItemsList(int programID)
        {
            try
            {
                OptionItemListResponse response = _initialCredentialProgramService.GetIntialCreditialOptionItemsList(programID);
                return response;
            }
            catch (Exception ex)
            {
                OptionItemListResponse response = new OptionItemListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
    }
}
