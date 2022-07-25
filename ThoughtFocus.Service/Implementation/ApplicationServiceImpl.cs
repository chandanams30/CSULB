using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Params;
using ThoughtFocus.Domain.User;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.CustomView;
using ThoughtFocus.Workflow;
using ThoughtFocus.Common.Exceptions;


namespace ThoughtFocus.Service.Implementation
{
    public class ApplicationServiceImpl : IApplicationService
    {
        #region Fields
        private readonly ILogger<ApplicationServiceImpl> _Logger;
        private long WorkFlowID = 1;
        private string _StateName = "";


        #endregion Fields

        #region Constructors
        public ApplicationServiceImpl(ILogger<ApplicationServiceImpl> logger)
        {
            this._Logger = logger;
        }
        #endregion Constructors

        #region Methods
        public BaseResponse ApplicationCommandHandler(ApplicationRequest applicationParam, UserSessionEntity userSessionEntity)
        {
            BaseResponse createResponse = new BaseResponse();

            if (String.IsNullOrEmpty(applicationParam.CommandName))
            {
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }
            if (userSessionEntity == null)
            {
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }
            if (userSessionEntity.UserID == 0)
            {
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }

            CommonResponse commonResponse = null;
        
            try
            {
                if (applicationParam.CommandName == WorkFlowCommandEnumeration.Submit.ToString() || applicationParam.CommandName == WorkFlowCommandEnumeration.Save.ToString())
                {
                    _Logger.LogDebug("Submit is initiated by User {0}", userSessionEntity.UserID);
                    //if (applicationParam.CommandName == WorkFlowCommandEnumeration.Submit.ToString())
                    //    commonResponse = this.SaveLoanApplication(applicationParam, userSessionEntity);
                    //if (applicationParam.CommandName == WorkFlowCommandEnumeration.Save.ToString() && applicationParam.IsBusinessProfile)
                    //    commonResponse = this.SaveLoanBusinessDetailAndOwner(applicationParam, userSessionEntity);
                    //if (applicationParam.CommandName == WorkFlowCommandEnumeration.Save.ToString() && applicationParam.IsFundingApp)
                    //    commonResponse = this.SaveFundingApplicationData(applicationParam, userSessionEntity);
                    //if (applicationParam.CommandName == WorkFlowCommandEnumeration.Save.ToString() && applicationParam.IsApplicationDocuments)
                    //    commonResponse = this.SaveLoanApplicationDocuments(applicationParam, userSessionEntity);

                    //if (commonResponse.StatusMessage == "Success")
                    //{
                        //if (commonResponse.ID > 0)
                        //{
                            _Logger.LogDebug(String.Format("Save is successfull  by User {0} with ApplicationID​​​​​​​​ {1}", userSessionEntity.UserID, createResponse.ID));
                            _StateName = "SetState";
                            applicationParam.ApplicationID​​​​​​​​ = 1;//s commonResponse.ID;
                            this.CreateProcessInstance(1, WorkFlowID);
                            createResponse = this.ExecuteWorkflowCommand(1, applicationParam, WorkFlowID, _StateName, userSessionEntity);

                            if (!createResponse.IsSuccess)
                            {
                                createResponse.Message = "Unable to process command.Please try after sometime.";
                                return createResponse;
                            }
                            createResponse.IsSuccess = true;
                            createResponse.Message = "Loan Application Created successfully.";
                            createResponse.ID = applicationParam.ApplicationID​​​​​​​​;
                    //    }
                    //}
                    //        else
                    //{
                    //    createResponse.IsSuccess = false;
                    //    createResponse.ValidationErrors = commonResponse.ValidationErrors;
                    //    return createResponse;
                    //}
        }

                else
                {

                    createResponse = this.ExecuteWorkflowCommand(applicationParam.ApplicationID​​​​​​​​, applicationParam, WorkFlowID, _StateName, userSessionEntity);
                    if (!createResponse.IsSuccess)
                    {
                        createResponse.Message = "Unable to process command.Please try after sometime.";
                        return createResponse;
                    }
                    createResponse.Message = "Transaction completed successfully.";
                    createResponse.ID = applicationParam.ApplicationID​​​​​​​​;
                }
            }
            catch (ConditionFailedException ex)
            {
                LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in ApplicationCommandHandler-> applicationParam, userSessionEntity ", null);
                createResponse.IsSuccess = false;
                createResponse.Message = ex.Message;
                return createResponse;
            }
            catch (WorkFlowException ex)
            {
                LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in ApplicationCommandHandler-> applicationParam, userSessionEntity ", null);
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }
            catch (BusinessException ex)
            {
                LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in ApplicationCommandHandler-> applicationParam, userSessionEntity ", null);
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }
            catch (Exception ex)
            {
                LoggerExtensions.LogInformation(_Logger, null, ex, "Exception in ApplicationCommandHandler-> applicationParam, userSessionEntity ", null);
                createResponse.IsSuccess = false;
                createResponse.Message = "Unable to process command.Please try after sometime.";
                return createResponse;
            }
            createResponse.IsSuccess = true;
            createResponse.Message = "Application saved successfully.";
            createResponse.ID = applicationParam.ApplicationID​​​​​​​​;
            return createResponse;
        }


        public BaseResponse ExecuteWorkflowCommand(long processID, ApplicationRequest applicationParam, long workFlowID, string stateName, UserSessionEntity userSessionEntity)
        {

            BaseResponse baseResponse = new BaseResponse();
            try
            {
                //User user = this._userRepository.GetByUserID(userSessionEntity.UserID);

                //if (user == null)
                //{
                //    _Logger.LogError(String.Format("user associated with UserID {0} is null in ExecuteWorkflowCommand.", userSessionEntity.UserID));
                //    baseResponse.IsSuccess = false;
                //    return baseResponse;
                //}
                String ProcessStateName = "";
                Dictionary<string, object> workFlowParameters = new Dictionary<string, object>();

                if (ThoughtFocus.Workflow.WorkflowInit.Runtime.IsProcessExists(processID, workFlowID))
                {
                    workFlowParameters.Add("UserSessionModel", userSessionEntity);
                    //workFlowParameters.Add("UserModel", user);
                    workFlowParameters.Add("Command", applicationParam.CommandName);
                    

                    if (stateName == "SetState")
                    {
                        ProcessStateName = WorkflowInit.Runtime.GetCurrentActivityName(processID, workFlowID);
                        if (String.IsNullOrEmpty(ProcessStateName))
                        {
                            _Logger.LogError(String.Format("ProcessStateName is Empty for ProcessID {0}  in ExecuteWorkflowCommand.", processID));
                            baseResponse.IsSuccess = false;
                            return baseResponse;
                        }
                        if (applicationParam.CommandName == WorkFlowCommandEnumeration.Save.ToString())
                        {
                            baseResponse = WorkflowInit.SetState(processID, workFlowID, ApplicationStatusEnumeration.Drafted.ToString(), workFlowParameters);
                            if (!baseResponse.IsSuccess)
                            {
                                _Logger.LogError(String.Format("baseResponse  returned false in WorkflowInit.SetState call for Process ID {0}", processID));
                                baseResponse.IsSuccess = false;
                                return baseResponse;
                            }
                        }
                        if (applicationParam.CommandName == WorkFlowCommandEnumeration.Submit.ToString())
                        {
                            if (ProcessStateName == ApplicationStatusEnumeration.Initialized.ToString() || ProcessStateName == ApplicationStatusEnumeration.Drafted.ToString())
                            {
                                baseResponse = WorkflowInit.SetState(processID, workFlowID, ApplicationStatusEnumeration.Submitted.ToString(), workFlowParameters);
                                if (!baseResponse.IsSuccess)
                                {
                                    _Logger.LogError(String.Format("baseResponse  returned false in WorkflowInit.SetState call for Process ID {0}", processID));
                                    baseResponse.IsSuccess = false;
                                    return baseResponse;
                                }
                            }
                            else if (ProcessStateName == ApplicationStatusEnumeration.RequestedMoreInfo.ToString())
                            {
                                baseResponse = WorkflowInit.SetState(processID, workFlowID, ApplicationStatusEnumeration.RequestCompleted.ToString(), workFlowParameters);
                                if (!baseResponse.IsSuccess)
                                {
                                    _Logger.LogError(String.Format("baseResponse  returned false in WorkflowInit.SetState call for Process ID {0}", processID));
                                    baseResponse.IsSuccess = false;
                                    return baseResponse;
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    bool isTransactionDocument = applicationParam.ApplicationDocuments.Any(a => a.DocumentTypeID == (long)DocumentTypeEnumeration.FundingDetailDocument);
                    //    if (isTransactionDocument)
                    //    {
                    //        applicationParam.FundUtilization.ApplicationDocument = new DocumentRequest();
                    //        foreach (var document in applicationParam.ApplicationDocuments)
                    //        {
                    //            applicationParam.FundUtilization.ApplicationDocument.DocumentGUID = document.DocumentGUID;
                    //            applicationParam.FundUtilization.ApplicationDocument.DocumentTypeID = document.DocumentTypeID;
                    //            applicationParam.FundUtilization.ApplicationDocument.FileName = document.FileName;
                    //            applicationParam.FundUtilization.ApplicationDocument.FileSize = document.FileSize;
                    //            applicationParam.FundUtilization.ApplicationDocument.DocumentName = document.DocumentName;
                    //            applicationParam.FundUtilization.ApplicationDocument.ApplicationID​​​​​​​​ = applicationParam.ApplicationID​​​​​​​​;
                    //            applicationParam.FundUtilization.ApplicationDocument.PhysicalFileStorageKey = document.PhysicalFileStorageKey;
                    //        }

                    //    }

                    //    workFlowParameters.Add("FundUtilization", applicationParam.FundUtilization);
                    //    _Logger.LogDebug(String.Format("ExecuteCommand  is initiated for applicationID {0} with State {1}", processID, applicationParam.CommandName));
                    //    baseResponse = WorkflowInit.ExecuteCommand(processID, applicationParam.CommandName, WorkFlowID, workFlowParameters);
                    //    if (!baseResponse.IsSuccess)
                    //    {
                    //        _Logger.LogError(String.Format("baseResponse  returned false in WorkflowInit.ExecuteCommand call for Process ID {0}", processID));
                    //        baseResponse.IsSuccess = false;
                    //        return baseResponse;
                    //    }
                    //}

                }
                else
                {
                    _Logger.LogError(String.Format("ExecuteCommad was called before Process is created  for ProcessID {0} with StateName {1}", processID, stateName));
                }
            }
            catch (WorkFlowException ex)
            {
                throw ex;
            }
            catch (ConditionFailedException ex)
            {
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            baseResponse.IsSuccess = true;
            baseResponse.ID = processID;
            return baseResponse;
        }

        private void CreateProcessInstance(long processID, long workFlowID)
        {
            try
            {
                _Logger.LogDebug(String.Format("Process instance creation is initiated for applicationID {0}", processID));
                if (WorkflowInit.Runtime.IsProcessExists(processID, workFlowID))
                {
                    _Logger.LogDebug(String.Format("Process instance is already created for applicationID {0}", processID));
                    return;
                }
                WorkflowInit.CreateProcessInstance(processID, workFlowID);
                _Logger.LogDebug(String.Format("Process instance is successfully created for applicationID {0}", processID));
                return;
            }
            catch (WorkFlowException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public WorkFlowCommandResponse GetWorkFlowCommands(long applicationID, UserSessionEntity userSessionEntity)
        {
            WorkFlowCommandResponse workFlowCommandResponse = new Domain.CustomView.WorkFlowCommandResponse();
            #region Validation

            if (userSessionEntity == null)
            {
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
                return workFlowCommandResponse;
            }
            if (userSessionEntity.UserID == 0)
            {
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
                return workFlowCommandResponse;
            }

            #endregion


            List<WorkflowCommandViewEntity> WorkflowCommandViewEntities = new List<WorkflowCommandViewEntity>();
            try
            {
                long UserID = userSessionEntity.UserID;
                //Guid UserID = Guid.Empty;
                //User user = this._userRepository.GetByUserID(userSessionEntity.UserID);
                //if (user != null)
                //{
                //    UserID = user.IdentityID;
                //}
                //else
                //{
                //    _Logger.LogError(String.Format("user is null for userID {0}", userSessionEntity.UserID));
                //    workFlowCommandResponse.IsSuccess = false;
                //    workFlowCommandResponse.WorkFlowCommands = null;
                //    workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
                //}

                WorkflowCommandViewEntities = WorkflowInit.GetWorkFlowCommands(applicationID, UserID, WorkFlowID);
                workFlowCommandResponse.IsSuccess = true;
                workFlowCommandResponse.WorkFlowCommands = WorkflowCommandViewEntities;
            }
            catch (WorkFlowException ex)
            {
                //LoggerExtensions.LogMessage(Logger, ex);
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.WorkFlowCommands = null;
                workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
            }
            catch (BusinessException ex)
            {
                //LoggerExtensions.LogMessage(Logger, ex);
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.WorkFlowCommands = null;
                workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
            }
            catch (Exception ex)
            {
                // LoggerExtensions.LogMessage(Logger, ex);
                workFlowCommandResponse.IsSuccess = false;
                workFlowCommandResponse.WorkFlowCommands = null;
                workFlowCommandResponse.Message = "Unable to fetch commands.Please try after sometime.";
            }
            return workFlowCommandResponse;
        }

        #endregion Methods
    }
}
