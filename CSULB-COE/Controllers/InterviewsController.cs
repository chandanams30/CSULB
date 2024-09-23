using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using ThoughtFocus.Domain.Request.Interviews;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.Interviews;
using ThoughtFocus.Domain.Response.Rubrics;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterviewsController : ControllerBase
    {
        private ILogger<InterviewsController> _logger;
        private IInterviewService _interviewService;
        private readonly IConfiguration _configuration;
        public InterviewsController(IInterviewService interviewService,
              ILogger<InterviewsController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _interviewService = interviewService;
        }
        [HttpPost("UpsertInterview")]
        public InterviewBasicDetailsResponse UpsertInterview(UpsertInterview input)
        {
            try
            {
                InterviewBasicDetailsResponse response = _interviewService.UpsertInterview(input);
                return response;
            }
            catch (Exception ex)
            {
                InterviewBasicDetailsResponse response = new InterviewBasicDetailsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetInterviewDetails")]
        public InterviewDetails GetInterviewDetails(int InterviewId,int UserId)
        {
            try
            {
                InterviewDetails response = _interviewService.GetInterviewDetails(InterviewId, UserId);
                return response;
            }
            catch (Exception ex)
            {
                InterviewDetails response = new InterviewDetails();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpsertInterviewSlots")]
        public InterviewSlotsList UpsertInterviewSlots(UpsertInterviewSlots input)
        {
            try
            {
                input.InterviewLink = "<a href=\"http://20.25.58.133/CSULBced-dev/swagger/index.html\">Development</a>";
                InterviewSlotsList response = _interviewService.UpsertInterviewSlots(input);
                return response;
            }
            catch (Exception ex)
            {
                InterviewSlotsList response = new InterviewSlotsList();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetInterviewList")]
        public InterviewList GetInterviewList(int UserId)
        {
            try
            {
                InterviewList response = _interviewService.GetInterviewList(UserId);
                return response;
            }
            catch (Exception ex)
            {
                InterviewList response = new InterviewList();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("GetInterviewerList")]

        public InterviewersList GetInterviewerList()
        {
            try
            {
                InterviewersList response = _interviewService.GetInterviewerList();
                return response;
            }
            catch (Exception ex)
            {
                InterviewersList response = new InterviewersList();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
    }
}
