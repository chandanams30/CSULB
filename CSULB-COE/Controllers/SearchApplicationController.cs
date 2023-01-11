using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Response.SearchApplication;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchApplicationController : ControllerBase
    {
        public ILogger<SearchApplicationController> _logger;
        public ISearchApplication _searchApplicationService;
        private readonly IConfiguration _configuration;
        public SearchApplicationController(ISearchApplication searchApplicationService,
              ILogger<SearchApplicationController> logger
            , IConfiguration configuration)
        {
            _logger = logger;
            _searchApplicationService = searchApplicationService;
            _configuration = configuration;
        }
        [HttpGet("GetStudentSearchData")]
        public StudentSearchResponse GetStudentSearchData(string searchString)
        {
            try
            {
                StudentSearchResponse response = _searchApplicationService.GetStudentSearchData(searchString);
                return response;
            }
            catch (Exception ex)
            {
                StudentSearchResponse response = new StudentSearchResponse();
                response.IsSuccess = false;
                response.Message = "Failed to search data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
    }
}
