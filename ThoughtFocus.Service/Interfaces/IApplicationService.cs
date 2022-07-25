using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.CustomView;
using ThoughtFocus.Workflow;
using ThoughtFocus.Common.Exceptions;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Params;
using ThoughtFocus.Domain.User;
using ThoughtFocus.Domain.Enumeration;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IApplicationService
    {
        //FormResponse getAllForms();
        // getFormsSemesterWise(int semesterId);
        // getFormsProgramWise(int semesterId,int programId);

        /// <summary>
        /// This method is used to get the available commands 
        /// </summary>
        /// <param name="applicationID">Application Id</param>
        /// <param name="userSessionEntity">User Session Entity</param>        
        /// <returns>WorkFlowCommandResponse</returns>       
        /// <exception cref="Exception">Exception</exception>
        WorkFlowCommandResponse GetWorkFlowCommands(long applicationID, UserSessionEntity userSessionEntity);

        /// <summary>
        /// This method is used to Save the application details with state
        /// </summary>
        /// <param name="LoanApplicationRequest">Loan Application Request</param>
        /// <param name="userSessionEntity">User Session Entity</param>        
        /// <returns>BaseResponse</returns>       
        /// <exception cref="Exception">Exception</exception>
        BaseResponse ApplicationCommandHandler(ApplicationRequest applicationParam, UserSessionEntity userSessionEntity);

        /// <summary>
        /// This method is used to save the state change in the workflow
        /// </summary>
        /// <param name="processID">Process IDt</param>
        /// <param name="LoanApplicationRequest">Loan Application Request</param>
        /// <param name="workFlowID">WorkFlow ID</param>
        /// <param name="stateName">stateName</param>
        /// <param name="userSessionEntity">User Session Entity</param>        
        /// <returns>BaseResponse</returns>       
        /// <exception cref="Exception">Exception</exception>
        BaseResponse ExecuteWorkflowCommand(long processID, ApplicationRequest applicationParam, long workFlowID, string stateName, UserSessionEntity userSessionEntity);

    }
}
