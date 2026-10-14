using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class StudentTeachingMilestoneController : ControllerBase
    {
        private ILogger<StudentTeachingMilestoneController> _logger;
        private IStudentTeachingMilestoneService _studentTeachingMilestoneService;
        private readonly IConfiguration _configuration;
        public StudentTeachingMilestoneController(IStudentTeachingMilestoneService studentTeachingMilestoneService,
              ILogger<StudentTeachingMilestoneController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _studentTeachingMilestoneService = studentTeachingMilestoneService;
        }
        [HttpGet("GetLatestForm")]
        public GetLatestFormResponse GetLatestForm(string csulbid)
        {
            try
            {
                GetLatestFormResponse response = _studentTeachingMilestoneService.GetLatestForm(csulbid);
                return response;
            }
            catch (Exception ex)
            {
                GetLatestFormResponse response = new GetLatestFormResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                return response;
            }
        }
        [HttpGet("GetStudentTeachingMilestoneDetails")]
        public StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsulbId, int UserID, int formID, int roleID)
        {
            try
            {
                StudentTeachingMilestonesResponse response = _studentTeachingMilestoneService.GetStudentTeachingMilestoneDetails(CsulbId, UserID, formID, roleID);
                return response;
            }
            catch (Exception ex)
            {
                StudentTeachingMilestonesResponse response = new StudentTeachingMilestonesResponse();
                response.IsSuccess = false;
                response.Message = "Failed to fetch student profile data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
    }
}
