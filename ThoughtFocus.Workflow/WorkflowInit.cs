using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ThoughtFocus.Common.Exceptions;
using ThoughtFocus.Common.Workflow.Core.Bus;
using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.Persistence;
using ThoughtFocus.Common.Workflow.Core.Runtime;
using ThoughtFocus.Common.Workflow.DbPersistance;
using ThoughtFocus.Common.WorkFlowDataAccess;
//using ThoughtFocus.Constants;
//using ThoughtFocus.DataAccess.Models.User;
using ThoughtFocus.Domain;
using ThoughtFocus.Domain.Common;
using ThoughtFocus.Domain.CustomView;
//using ThoughtFocus.Domain.Params;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.User;
using ThoughtFocus.Workflow;

namespace ThoughtFocus.Workflow
{
    public class WorkflowInit
    {
      // public static readonly ILogger<WorkflowInit> _logger;

        public static ConnectionStrings _connectionStrings;
        private static SqlConnectionStrings _sqlConnectionString;
        public WorkflowInit(IOptions<ConnectionStrings> connectionStrings, IOptions<SqlConnectionStrings> sqlConnectionString)
        {
            _connectionStrings = connectionStrings.Value;
             _sqlConnectionString = sqlConnectionString.Value;
        }
        public WorkflowInit()
        {
        }
        private static volatile WorkflowRuntime _runtime;

        private static readonly object _sync = new object();

        public static WorkflowRuntime Runtime
        {
            get
            {
                if (_runtime == null)
                {
                    lock (_sync)
                    {
                        if (_runtime == null)
                        {
                          //  _logger.LogDebug("Creating a new Runtime object");

                            _runtime = new WorkflowRuntime(new Guid("{8D38DB8F-F3D5-4F26-A989-4FDD40F32D9D}"))
                                .WithActionProvider(DependencyHelper.WorkflowActions)
                                .WithRuleProvider(DependencyHelper.WorkflowRule)
                                .WithBus(new NullBus())
                                .WithRoleProvider(DependencyHelper.WorkflowRole)
                                .WithPersistenceProvider(new DbPersistenceProvider(new WorkFlowContext(_sqlConnectionString.AppDBConnection), _sqlConnectionString.AppDBConnection))
                                .WithBuilder(new DbWorkflowPersistanceProvider(new WorkFlowContext(_sqlConnectionString.AppDBConnection), _sqlConnectionString.AppDBConnection))
                                .Start();

                            _runtime.ProcessStatusChanged += new EventHandler<ProcessStatusChangedEventArgs>(_runtime_ProcessStatusChanged);
                        }
                    }
                }
                return _runtime;
            }
        }


        static void _runtime_ProcessStatusChanged(object sender, ProcessStatusChangedEventArgs e)
        {
            if (e.NewStatus != ProcessStatus.Idled && e.NewStatus != ProcessStatus.Finalized)
                return;

            //_runtime.PreExecuteFromCurrentActivity(e.ProcessId,e.workflowID);

            if (e.NewStatus != ProcessStatus.Finalized)
            {
                //TODO: Define action object and invoke
            }

            //Change state name
            var nextState = _runtime.GetLocalizedStateName(e.ProcessId, e.workflowID, e.ProcessInstance.CurrentState);
        }

        public static BaseResponse ExecuteCommand(long processID, string CommandName, long workFlowID, Dictionary<string, object> workFlowParameters)
        {
            BaseResponse baseResponse = new BaseResponse();
            try
            {
                #region Validation
                if (workFlowParameters == null)
                {
                    // _logger.LogError(String.Format("workFlowParameters is null for Process ID {0}", processID));
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to process command.Please try after sometime.";
                }
                if (workFlowParameters.Where(a => a.Key == "UserModel") == null)
                {
                    //  _logger.LogError(String.Format("workFlowParameters -UserModel is null for Process ID {0}", processID));
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to process command.Please try after sometime.";
                }

                #endregion

                UserSessionEntity userEntity = (UserSessionEntity)workFlowParameters.Where(a => a.Key == "UserSessionModel").FirstOrDefault().Value;
                if (userEntity == null)
                {
                    // _logger.LogError(String.Format("workFlowParameters -UserModel is null for Process ID {0}", processID));
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to process command.Please try after sometime.";
                }
                IEnumerable<WorkflowCommand> workFlowCommands = Runtime.GetAvailableCommands(processID, userEntity.UserID, workFlowID);

                if (workFlowCommands == null)
                {
                  //  _logger.LogError(String.Format("WorkFlowCommands is null for ProcessID {0} and UserID {1}", processID, userEntity.UserID));
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to process command.Please try after sometime.";
                }

                WorkflowCommand workflowCommand = workFlowCommands.FirstOrDefault(a => a.CommandName == CommandName);

                if (workflowCommand == null)
                {
                   // _logger.LogError(String.Format("WorkFlowCommand is null with CommandName {0} for ProcessID {1} for UserID {2}", CommandName, processID, userEntity.UserID));
                    baseResponse.IsSuccess = false;
                    baseResponse.Message = "Unable to process command.Please try after sometime.";
                }
                workflowCommand.CommandName = CommandName;
                workflowCommand.ProcessId = processID;
                workflowCommand.WorkFlowId = workFlowID;

                #region FillCommandParameters

                List<ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter> CommmandParamters = new List<ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter>();

                #region AddCommand
                ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter command = new ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter();
                command.Name = "Command";
                command.Value = workFlowParameters.FirstOrDefault(a => a.Key == "Command").Value == null ? String.Empty : workFlowParameters.FirstOrDefault(a => a.Key == "Command").Value.ToString();
                command.TypeAsString = "System.String";
                CommmandParamters.Add(command);
                #endregion

                #region AddUserSessionModel
                ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter userSessionParameter = new ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter();
                userSessionParameter.Name = "UserSessionModel";
                userSessionParameter.Value = (UserSessionEntity)workFlowParameters.Where(a => a.Key == "UserSessionModel").FirstOrDefault().Value; ;
                userSessionParameter.TypeAsString = "ThoughtFocus.Domain.User.UserSessionEntity,ThoughtFocus.Domain";
                CommmandParamters.Add(userSessionParameter);
                #endregion

                //#region FundUtilization
                //ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter fundUtilization = new ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter();
                //fundUtilization.Name = "FundUtilization";
                //fundUtilization.Value = (FundUtilizationParam)workFlowParameters.Where(a => a.Key == "FundUtilization").FirstOrDefault().Value; ;
                //fundUtilization.TypeAsString = "ThoughtFocus.Domain.Params.FundUtilizationParam,ThoughtFocus.Domain";
                //CommmandParamters.Add(fundUtilization);
                //#endregion

                //#region AddTransitionComment
                //ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter comment = new ThoughtFocus.Common.Workflow.Core.Runtime.WorkflowCommand.CommandParameter();
                //comment.Name = "Comment";
                //comment.Value = workFlowParameters.FirstOrDefault(a => a.Key == "Comment").Value == null ? String.Empty : workFlowParameters.FirstOrDefault(a => a.Key == "Comment").Value.ToString();
                //comment.TypeAsString = "System.String";
                //CommmandParamters.Add(comment);
                //#endregion

                workflowCommand.Parameters = CommmandParamters;

                #endregion
               
                Runtime.ExecuteCommand(processID, workFlowID, userEntity.IdentityID, userEntity.IdentityID, workflowCommand);
                baseResponse.IsSuccess = true;
                    
            }
            catch (ConditionFailedException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new WorkFlowException("",ex);
            }
            return baseResponse;
        }

        public static void CreateProcessInstance(long processID, long workFlowID)
        {
            try
            {
                Runtime.CreateInstance(workFlowID, processID);
            }
            catch (Exception ex)
            {
                throw new WorkFlowException("", ex);
            }
        }

        public static BaseResponse SetState(long processID, long workflowID, string stateName, Dictionary<string, object> workFlowParameters)
        {
            BaseResponse baseResponse = new BaseResponse();

            #region Validation
            if (workFlowParameters == null)
            {
               // _logger.LogError(String.Format("workFlowParameters is null for Process ID {0}", processID));
                baseResponse.IsSuccess = false;
                baseResponse.Message = "Unable to process command.Please try after sometime.";
            }
            #endregion
            try
            {
                UserSessionEntity usersessionEntity = (UserSessionEntity)workFlowParameters.FirstOrDefault(a => a.Key == "UserSessionModel").Value;

                string command = workFlowParameters.FirstOrDefault(a => a.Key == "Command").Value == null ? String.Empty : workFlowParameters.FirstOrDefault(a => a.Key == "Command").Value.ToString();
                //string comment = workFlowParameters.FirstOrDefault(a => a.Key == "Comment").Value == null ? String.Empty : workFlowParameters.FirstOrDefault(a => a.Key == "Comment").Value.ToString();

                Dictionary<string, object> workflowDictionary = new Dictionary<string, object>();
                //workflowDictionary.Add("UserModel", userEntity);
                workflowDictionary.Add("UserSessionModel", usersessionEntity);
                //workflowDictionary.Add("Comment", comment);
                workflowDictionary.Add("Command", command);

                Runtime.SetState(processID, workflowID, usersessionEntity.IdentityID, usersessionEntity.IdentityID, stateName, workflowDictionary);
                
            }
            catch (WorkFlowException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new WorkFlowException("", ex);
            }
            baseResponse.IsSuccess = true;
            return baseResponse;

        }

        public static List<WorkflowCommandViewEntity> GetWorkFlowCommands(long applicationID, long userID, long workFlowID)
        {
            List<WorkflowCommandViewEntity> WorkflowCommandViewEntities = new List<WorkflowCommandViewEntity>();
            try
            {
                IEnumerable<WorkflowCommand> workFlowCommands;
                if (applicationID > 0)
                {
                    workFlowCommands = Runtime.GetAvailableCommands(applicationID, userID, workFlowID);
                }
                else
                {
                    workFlowCommands = Runtime.GetInitialCommands(workFlowID, userID);
                }
                if (workFlowCommands != null)
                {
                    foreach (var command in workFlowCommands.ToList())
                    {
                        WorkflowCommandViewEntity workflowCommandEntity = new WorkflowCommandViewEntity();
                        workflowCommandEntity.LocalizedName = command.LocalizedName;
                        workflowCommandEntity.CommandIconClass = command.CommandIconClass;
                        workflowCommandEntity.WorkflowCommandID = command.CommandID;
                        workflowCommandEntity.Name = command.CommandName;
                        if (command.TransitionType == "Direct")
                        {

                            workflowCommandEntity.CommandTransitionType = "btn btn-primary btn-outline";
                        }
                        else if (command.TransitionType == "Reverse")
                        {
                            workflowCommandEntity.CommandTransitionType = "btn btn-warning btn-outline";
                        }
                        else
                        {
                            workflowCommandEntity.CommandTransitionType = "btn btn-secondary btn-outline";
                        }
                        #region SettingValidationRules

                        workflowCommandEntity.WorkFlowCommandValidationEntity = new WorkFlowCommandValidationEntity();
                        //if (command.TransitionValidationDefinations != null)
                        //{
                        //    if (command.TransitionValidationDefinations.FirstOrDefault(a => a.ValidationDefination.ValidationDefinationName == ValidationDefinationEnumeration.CommentRequiredValidation.ToString()) != null)
                        //        workflowCommandEntity.WorkFlowCommandValidationEntity.CommentRequiredValidation = true;
                        //    if (command.TransitionValidationDefinations.FirstOrDefault(a => a.ValidationDefination.ValidationDefinationName == ValidationDefinationEnumeration.FundUtilizationValidation.ToString()) != null)
                        //        workflowCommandEntity.WorkFlowCommandValidationEntity.FundUtilizationValidation = true;
                        //    if (command.TransitionValidationDefinations.FirstOrDefault(a => a.ValidationDefination.ValidationDefinationName == ValidationDefinationEnumeration.AgreementValidation.ToString()) != null)
                        //        workflowCommandEntity.WorkFlowCommandValidationEntity.AgreementValidation = true;    
                            
                        //}
                        #endregion

                        WorkflowCommandViewEntities.Add(workflowCommandEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new WorkFlowException("", ex);
            }
            return WorkflowCommandViewEntities;
        }

    }
}
