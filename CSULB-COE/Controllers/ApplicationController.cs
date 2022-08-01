using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Params;
using ThoughtFocus.Domain.CustomView;
using ThoughtFocus.Domain;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        public ILogger<LoginController> _logger;
        private IApplicationService _applicationService;

        public ApplicationController(ILogger<LoginController> logger,
            IApplicationService applicationService)
        {
            _logger = logger;
            this._applicationService = applicationService;
        }

        [HttpPost]
        [Route("ApplicationCommandHandler")]
        public BaseResponse ApplicationCommandHandler(ApplicationRequest applicationParam)
        {
            BaseResponse baseResponse = new BaseResponse();
            try
            {
                #region Validation

                if (applicationParam == null)
                {
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to draft application at this moment.Please try after sometime.";
                    return baseResponse;
                }
                #endregion

                //ThoughtFocus.Domain.User.UserSessionEntity userSession = LoginUserInformation.getLoggedInUser(HttpContext);
                ThoughtFocus.Domain.User.UserSessionEntity userSession = new ThoughtFocus.Domain.User.UserSessionEntity();
                userSession.UserID = 1;

                baseResponse = this._applicationService.ApplicationCommandHandler(applicationParam, userSession);
                return baseResponse;
            }
            catch (Exception ex)
            {

                return baseResponse;
            }
        }

        [HttpGet]
        [Route("GetWorkFlowCommands")]
        public WorkFlowCommandResponse GetWorkFlowCommands(int applicationID)
        {
            WorkFlowCommandResponse workFlowCommandResponse = new WorkFlowCommandResponse();
            try
            {
                //ThoughtFocus.Domain.User.UserSessionEntity userSession = LoginUserInformation.getLoggedInUser(HttpContext);
                ThoughtFocus.Domain.User.UserSessionEntity userSession = new ThoughtFocus.Domain.User.UserSessionEntity();
                userSession.UserID = 11;

                workFlowCommandResponse = this._applicationService.GetWorkFlowCommands(applicationID, userSession);
                workFlowCommandResponse.IsSuccess = true;
                return workFlowCommandResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error encountered at workFlowCommandResponse GetWorkFlowCommands >> ", ex);
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.Message = "Exception occurred while getting Workflow Commands";
                return workFlowCommandResponse;
            }
        }
    }
}
