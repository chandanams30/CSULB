using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.Rubrics;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RubricsController : ControllerBase
    {
        private ILogger<RubricsController> _logger;
        private IRubricsService _rubricsService;
        private readonly IConfiguration _configuration;
        public RubricsController(IRubricsService rubricsService,
              ILogger<RubricsController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _rubricsService = rubricsService;
        }
        [HttpGet("GetRubricsTemplateList")]
        public RubricsTemplateListResponse GetRubricsTemplateList()
        {
            try
            {
                RubricsTemplateListResponse response = _rubricsService.GetRubricsTemplateList();
                return response;
            }
            catch (Exception ex)
            {
                RubricsTemplateListResponse response = new RubricsTemplateListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertRubricsTemplate")]
        public BaseResponse UpsertRubricsTemplate(UpsertRubricsTemplateRequest input)
        {
            try
            {
                BaseResponse response = _rubricsService.UpsertRubricsTemplate(input);
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
        [HttpPost("PublishRubricsForm")]
        public BaseResponse PublishRubricsForm(PublishRubricsFormRequest input)
        {
            try
            {
                BaseResponse response = _rubricsService.PublishRubricsForm(input);
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
        [HttpGet("GetRubricsTemplateByID")]
        public RubricsTemplateByIDResponse GetRubricsTemplateByID(int TemplateID)
        {
            try
            {
                RubricsTemplateByIDResponse response = _rubricsService.GetRubricsTemplateByID(TemplateID);
                return response;
            }
            catch (Exception ex)
            {
                RubricsTemplateByIDResponse response = new RubricsTemplateByIDResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetRubricSubmittedFormsList")]
        public GetRubricSubmittedFormsListResponse GetRubricSubmittedFormsList(int UserID, int ProgramID,string TermCode)
        {
            try
            {
                GetRubricSubmittedFormsListResponse response = _rubricsService.GetRubricSubmittedFormsList(UserID,ProgramID,TermCode);
                return response;
            }
            catch (Exception ex)
            {
                GetRubricSubmittedFormsListResponse response = new GetRubricSubmittedFormsListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetPublishedRubricsDetails")]
        public PublishedRubricsDetailsResponse GetPublishedRubricsDetails(int TemplateID)
        {
            try
            {
                PublishedRubricsDetailsResponse response = _rubricsService.GetPublishedRubricsDetails(TemplateID);
                return response;
            }
            catch (Exception ex)
            {
                PublishedRubricsDetailsResponse response = new PublishedRubricsDetailsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertRubricsFilledForm")]
        public BaseResponse UpsertRubricsFilledForm(UpsertRubricsFilledFormRequest input)
        {
            try
            {
                BaseResponse response = _rubricsService.UpsertRubricsFilledForm(input);
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
        [HttpPost("RubricsApplicationForm")]
        public RubricsApplicationFormResponse GetRubricsApplicationForm(RubricsApplicationFormRequest input)
        {
            try
            {
                RubricsApplicationFormResponse response = _rubricsService.GetRubricsApplicationForm(input);
                return response;
            }
            catch (Exception ex)
            {
                RubricsApplicationFormResponse response = new RubricsApplicationFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
    }
}
