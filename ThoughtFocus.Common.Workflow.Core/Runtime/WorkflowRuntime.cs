using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Bus;
using ThoughtFocus.Common.Workflow.Core.Fault;
using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.Persistence;
using ThoughtFocus.Common.Exceptions;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public sealed class WorkflowRuntime
    {
       

        public WorkflowRuntime()
        { }
        internal bool ValidateSettings()
        {
            return Bus != null && Builder != null && PersistenceProvider != null;
        }
       

        internal bool IsAutoUpdateSchemeBeforeGetAvailableCommands { get; set; }

        //public TimeSpan TimerOvnershipIgnoranceInterval { get; set; }

        internal event EventHandler<NeedDeterminingParametersEventArgs> OnNeedDeterminingParameters;

        public event EventHandler<SchemaWasChangedEventArgs> OnSchemaWasChanged;

        internal IWorkflowBus Bus;

        //private readonly RuntimeTimer _runtimeTimer; 
        public Guid Id { get; private set; }

        private IWorkflowActionProvider _actionProvider;
        public IWorkflowActionProvider ActionProvider
        {
            get
            {
                if (_actionProvider == null)
                    throw new InvalidOperationException();
                return _actionProvider;
            }
            internal set { _actionProvider = value; }
        }

        private IWorkflowRuleProvider _ruleProvider;
        public IWorkflowRuleProvider RuleProvider
        {
            get
            {
                if (_ruleProvider == null)
                    throw new InvalidOperationException();
                return _ruleProvider;
            }
            internal set { _ruleProvider = value; }
        }

        private IWorkflowRoleProvider _roleProvider;
        public IWorkflowRoleProvider RoleProvider
        {
            get
            {
                if (_roleProvider == null)
                    throw new InvalidOperationException();
                return _roleProvider;
            }
            internal set { _roleProvider = value; }
        }

        public IWorkflowProvider _builder;
        public IWorkflowProvider Builder
        {
            get
            {
                if (_builder == null)
                    throw new InvalidOperationException();
                return _builder;
            }
            internal set { _builder = value; }
        }



        internal IPersistenceProvider _persistenceProvider;
        public IPersistenceProvider PersistenceProvider
        {
            get
            {
                if (_persistenceProvider == null)
                    throw new InvalidOperationException();
                return _persistenceProvider;
            }
            internal set { _persistenceProvider = value; }
        }

        internal IRuntimePersistence _runtimePersistence;
        public IRuntimePersistence RuntimePersistence
        {
            get
            {
                if (_runtimePersistence == null)
                    throw new InvalidOperationException();
                return _runtimePersistence;
            }
            internal set { _runtimePersistence = value; }
        }

        public event EventHandler<ProcessStatusChangedEventArgs> ProcessStatusChanged;



        public WorkflowRuntime(Guid runtimeId)
        {
            Id = runtimeId;

            //_runtimeTimer = _runtimePersistence.LoadTimer(Id);
            //if (_runtimeTimer == null) 
            //    _runtimeTimer = new RuntimeTimer();

            //_runtimeTimer.TimerComplete += TimerComplete;
            //_runtimeTimer.NeedSave += _runtimeTimer_NeedSave;
        }

        //private object _timerSaveLock = new object();

        //void _runtimeTimer_NeedSave(object sender, EventArgs e)
        //{
        //    lock (_timerSaveLock)
        //    {
        //        _runtimePersistence.SaveTimer(Id, _runtimeTimer);
        //    }
        //}

        private void TimerComplete(object sender, RuntimeTimerEventArgs e)
        {
            TransitionDefinition currentTimerTransition;
            ProcessInstance processInstance;
            try
            {
                processInstance = Builder.GetWorkflowInstance(e.ProcessId, e.workFlowDefinitionID);
                PersistenceProvider.FillProcessParameters(processInstance);

                currentTimerTransition =
                    processInstance.Workflow.GetTimerTransitionForActivity(processInstance.CurrentActivity).
                        FirstOrDefault(p => p.Trigger.Timer.Name == e.TimerName);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error Timer Complete Workflow UNKNOWN", ex);
                throw;
            }

            if (currentTimerTransition != null)
            {
                try
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Running);
                    var parametersLocal = new List<ParameterDefinitionWithValue>();
                    parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterIdentityId, Guid.Empty));
                    parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterImpersonatedIdentityId, Guid.Empty));
                    parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterSchemeId, processInstance.SchemeId));
                    var newExecutionParameters = new List<ExecutionRequestParameters>();
                    newExecutionParameters.Add(ExecutionRequestParameters.Create(processInstance.ProcessId, processInstance.WorkFlowID, processInstance.ProcessParameters, currentTimerTransition));
                    Bus.QueueExecution(newExecutionParameters);

                }
                catch (Exception ex)
                {
                    Logger.Log.Error(string.Format("Error Timer Complete Workflow Id={0}", processInstance.ProcessId), ex);
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                }
            }
        }

        private void FillParameters(ProcessInstance instance, ExecutionResponseParametersComplete newParameters)
        {

            foreach (var parameter in newParameters.ParameterContainer)
            {
                var parameterDefinition = instance.Workflow.GetNullableParameterDefinition(parameter.Name);
                if (parameterDefinition != null)
                {
                    var parameterDefinitionWithValue = ParameterDefinition.Create(parameterDefinition, parameter.Value);

                    instance.AddParameter(parameterDefinitionWithValue);
                }
            }
        }


        internal void BusExecutionComplete(object sender, ExecutionResponseEventArgs e)
        {
            var executionResponseParameters = e.Parameters;
            var processInstance = Builder.GetWorkflowInstance(executionResponseParameters.ProcessId, executionResponseParameters.WorkFlowDefinitionID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);
            //TODO Сделать метод филл CurrentActivity//fill cuurent activity
            if (executionResponseParameters.IsEmplty)
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                //var timerTransitions =
                //    processInstance.ProcessScheme.GetTimerTransitionForActivity(processInstance.CurrentActivity).ToList();

                //timerTransitions.ForEach(p=>_runtimeTimer.UpdateTimer(processInstance.ProcessId,p.Trigger.Timer));

                return;
            }
            if (executionResponseParameters.IsError)
            {
                var executionErrorParameters = executionResponseParameters as ExecutionResponseParametersError;

                Logger.Log.Error(string.Format("Error Execution Complete Workflow Id={0}\n{1}",
                    processInstance.ProcessId,
                    processInstance.ProcessParametersToString(ParameterPurposeEnumeration.System)),
                    executionErrorParameters.Exception);

                if (string.IsNullOrEmpty(executionErrorParameters.ExecutedTransitionName))
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                    throw executionErrorParameters.Exception;
                }

                var transition = processInstance.Workflow.FindTransition(executionErrorParameters.ExecutedTransitionName);

                var onErrorDefinition = transition.OnErrors.Where(
                    oe => executionErrorParameters.Exception.GetType().Equals(oe.ExceptionType)).
                                                          OrderBy(oe => oe.Priority).FirstOrDefault() ??
                                                      transition.OnErrors.Where(
                                                          oe => oe.ExceptionType.IsAssignableFrom(executionErrorParameters.Exception.GetType())).
                                                          OrderBy(oe => oe.Priority).FirstOrDefault();
                if (onErrorDefinition == null)
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                    throw executionErrorParameters.Exception;
                }

                if (onErrorDefinition.ActionType == OnErrorActionType.SetActivity)
                {
                    var from = processInstance.CurrentActivity;
                    var to = processInstance.Workflow.FindActivity((onErrorDefinition as SetActivityOnErrorDefinition).NameRef);
                    PersistenceProvider.UpdatePersistenceState(processInstance, TransitionDefinition.Create(from, to));
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                }

                throw executionErrorParameters.Exception;
            }


            try
            {

                ActivityDefinition newCurrentActivity;
                if (string.IsNullOrEmpty(executionResponseParameters.ExecutedTransitionName))
                {
                    if (executionResponseParameters.ExecutedActivityName == processInstance.Workflow.InitialActivity.Name)
                        newCurrentActivity = processInstance.Workflow.InitialActivity;
                    else
                    {
                        var from = processInstance.CurrentActivity;
                        var to = processInstance.Workflow.FindActivity(executionResponseParameters.ExecutedActivityName);
                        newCurrentActivity = to;
                        PersistenceProvider.UpdatePersistenceState(processInstance, TransitionDefinition.Create(from, to));
                    }
                }
                else
                {
                    var executedTransition =
                        processInstance.Workflow.FindTransition(executionResponseParameters.ExecutedTransitionName);
                    newCurrentActivity = executedTransition.To;
                    PersistenceProvider.UpdatePersistenceState(processInstance, executedTransition);

                }

                FillParameters(processInstance, (executionResponseParameters as ExecutionResponseParametersComplete));
                PersistenceProvider.SavePersistenceParameters(processInstance);

                var autoTransitions =
                    processInstance.Workflow.GetAutoTransitionForActivity(newCurrentActivity).ToList();
                if (autoTransitions.Count() < 1)
                {
                    SetProcessNewStatus(processInstance,
                                        newCurrentActivity.IsFinal ? ProcessStatus.Finalized : ProcessStatus.Idled);

                    //var timerTransitions =
                    //processInstance.ProcessScheme.GetTimerTransitionForActivity(newCurrentActivity).ToList();

                    //timerTransitions.ForEach(p => _runtimeTimer.SetTimer(processInstance.ProcessId, p.Trigger.Timer));

                    return;
                }

                PersistenceProvider.FillProcessParameters(processInstance);

                var newExecutionParameters = new List<ExecutionRequestParameters>();
                newExecutionParameters.AddRange(
                    autoTransitions.Select(
                        at =>
                        ExecutionRequestParameters.Create(processInstance.ProcessId, processInstance.WorkFlowID, processInstance.ProcessParameters,
                                                          at)));
                Bus.QueueExecution(newExecutionParameters);
            }
            catch (ActivityNotFoundException)
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Terminated);
            }
            //TODO Обработка ошибок
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("Error Execution Complete Workflow Id={0}", processInstance.ProcessId), ex);
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw;
            }
        }

        private bool _isColdStart;

        internal void Start()
        {
            PersistenceProvider.ResetWorkflowRunning();
            Bus.Start();
            //_runtimeTimer.RefreshTimer();
        }

        internal void ColdStart()
        {
            _isColdStart = true;
            //_runtimeTimer.IsCold = true;
            Bus.Start();
        }

        public void CreateInstance(long workFlowID, long processId)
        {
            CreateInstance(workFlowID, processId, new Dictionary<string, IEnumerable<object>>());
        }

        public void CreateInstance(long workFlowID, long processId, IDictionary<string, IEnumerable<object>> parameters)
        {
            Logger.Log.Debug(String.Format("CreateInstance is started for processId {0} ", processId));
            var processInstance = Builder.CreateNewWorkflow(processId, workFlowID, parameters);
            PersistenceProvider.InitializeProcess(processInstance);
            Logger.Log.Debug(String.Format("Initialize Process instance completed  for processId {0} ", processId));
            SetProcessNewStatus(processInstance, ProcessStatus.Initialized);
            if (processInstance.Workflow.InitialActivity.HaveImplementation)
            {
                try
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Running);
                    ExecuteRootActivity(processInstance);
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(String.Format("{0} {1}", Extensions.GetMessage("Exception occured in CreateInstance with processID", processId), Extensions.GetMessage("exception", ex)));
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                }

            }
            SetProcessNewStatus(processInstance, ProcessStatus.Idled);
        }



        private void ExecuteRootActivity(ProcessInstance processInstance)
        {
            PersistenceProvider.FillProcessParameters(processInstance);
            processInstance.AddParameter(ParameterDefinition.Create(DefaultDefinitions.ParameterSchemeId, processInstance.SchemeId));

            //TODO Убрать после обработки команд
            try
            {
                Bus.QueueExecution(ExecutionRequestParameters.Create(processInstance.ProcessId,
                     processInstance.WorkFlowID,
                                                                processInstance.ProcessParameters,
                                                                processInstance.Workflow.InitialActivity,
                                                                ConditionDefinition.Always));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("Error Execute Root Workflow Id={0}", processInstance.ProcessId), ex);
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw ex;
            }

        }


        public WorkflowDefinition GetProcessScheme(long processId, long workflowID)
        {
            return Builder.GetWorkflowInstance(processId, workflowID).Workflow;
        }



        public IEnumerable<WorkflowCommand> GetAvailableCommands(long processId, long workflowID, IEnumerable<long> identityIds, string commandNameFilter = null, long? mainIdentityId = null)
        {
            var identityIdsList = mainIdentityId.HasValue
                                      ? identityIds.Except(new List<long> { mainIdentityId.Value }).ToList()
                                      : identityIds.ToList();

            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);


            if (IsAutoUpdateSchemeBeforeGetAvailableCommands)
                processInstance = UpdateScheme(processId, workflowID, processInstance);

            var currentActivity = processInstance.Workflow.FindActivity(processInstance.CurrentActivityName);

            List<TransitionDefinition> commandTransitions;
            if (string.IsNullOrEmpty(commandNameFilter))
                commandTransitions = processInstance.Workflow.GetCommandTransitions(currentActivity).ToList();
            else
            {
                commandTransitions = processInstance.Workflow.GetCommandTransitions(currentActivity).Where(c => c.Trigger.Command.Name == commandNameFilter).ToList();
            }

            var commands = new List<WorkflowCommand>();

            foreach (var transitionDefinition in commandTransitions)
            {
                List<long> availiableIds = null;
                if (mainIdentityId.HasValue && ValidateActor(processId, mainIdentityId.Value, transitionDefinition))
                    availiableIds = new List<long>() { mainIdentityId.Value };

                if (availiableIds == null)
                    availiableIds = identityIdsList.Where(id => ValidateActor(processId, id, transitionDefinition)).ToList();

                if (availiableIds.Count() > 0)
                {
                    var command = WorkflowCommand.Create(processId, workflowID, transitionDefinition);
                    foreach (var availiableId in availiableIds)
                    {
                        command.AddIdentity(availiableId);
                    }

                    command.LocalizedName = processInstance.GetLocalizedCommandName(command.CommandName,
                                                                                    CultureInfo.CurrentCulture);

                    commands.Add(command);
                }
            }



            return commands;

        }



        public IEnumerable<WorkflowCommand> GetInitialCommands(long workFlowID, long identityId)
        {
            return GetInitialCommands(workFlowID, new List<long>() { identityId });
        }

        public WorkflowState GetInitialState(long workFlowID, IDictionary<string, IEnumerable<object>> processParameters = null)
        {
            var processDefinition = processParameters != null ? Builder.GetWorkflowDefinition(workFlowID, processParameters) : Builder.GetWorkflowDefinition(workFlowID);

            var initialActivity = processDefinition.InitialActivity;

            return new WorkflowState()
                {
                    Name = initialActivity.State,
                    WorkFlowID = workFlowID,
                    VisibleName = processDefinition.GetLocalizedStateName(initialActivity.State, CultureInfo.CurrentCulture)
                };
        }
        public WorkflowState GetInitialState(string workFlowName, IDictionary<string, IEnumerable<object>> processParameters = null)
        {
            var processDefinition = processParameters != null ? Builder.GetWorkflowDefinition(workFlowName, processParameters) : Builder.GetWorkflowDefinition(workFlowName);

            var initialActivity = processDefinition.InitialActivity;

            return new WorkflowState()
            {
                Name = initialActivity.State,
                WorkFlowName = workFlowName,
                VisibleName = processDefinition.GetLocalizedStateName(initialActivity.State, CultureInfo.CurrentCulture)
            };
        }

        public string GetLocalizedStateNameByProcessName(string workflowName, string stateName, IDictionary<string, IEnumerable<object>> processParameters = null)
        {
            var processDefinition = processParameters != null ? Builder.GetWorkflowDefinition(workflowName, processParameters) : Builder.GetWorkflowDefinition(workflowName);
            return processDefinition.GetLocalizedStateName(stateName, CultureInfo.CurrentCulture);
        }

        public IEnumerable<WorkflowCommand> GetInitialCommands(string workflowName, IEnumerable<long> identityIds, IDictionary<string, IEnumerable<object>> processParameters = null, string commandNameFilter = null, Guid? mainIdentityId = null)
        {
            var processDefinition = processParameters != null ? Builder.GetWorkflowDefinition(workflowName, processParameters) : Builder.GetWorkflowDefinition(workflowName);

            var initialActivity = processDefinition.InitialActivity;

            List<TransitionDefinition> commandTransitions;
            if (string.IsNullOrEmpty(commandNameFilter))
                commandTransitions = processDefinition.GetCommandTransitions(initialActivity).ToList();
            else
            {
                commandTransitions = processDefinition.GetCommandTransitions(initialActivity).Where(c => c.Trigger.Command.Name == commandNameFilter).ToList();
            }

            var commands = new List<WorkflowCommand>();

            foreach (var transitionDefinition in commandTransitions.Where(c => c.Condition.ConditionType == ConditionTypeEnumeration.Always))
            {
                var command = WorkflowCommand.Create(0, 0, transitionDefinition); //Why 0 in processID???-(Ask Arpitha)

                command.LocalizedName = processDefinition.GetLocalizedCommandName(command.CommandName,
                                                                                CultureInfo.CurrentCulture);

                commands.Add(command);
            }



            return commands;

        }


        public IEnumerable<WorkflowCommand> GetInitialCommands(long workFlowID, IEnumerable<long> identityIds, IDictionary<string, IEnumerable<object>> processParameters = null, string commandNameFilter = null, Guid? mainIdentityId = null)
        {
            var processDefinition = processParameters != null ? Builder.GetWorkflowDefinition(workFlowID, processParameters) : Builder.GetWorkflowDefinition(workFlowID);

            var initialActivity = processDefinition.InitialActivity;

            List<TransitionDefinition> commandTransitions;
            if (string.IsNullOrEmpty(commandNameFilter))
                commandTransitions = processDefinition.GetCommandTransitions(initialActivity).ToList();
            else
            {
                commandTransitions = processDefinition.GetCommandTransitions(initialActivity).Where(c => c.Trigger.Command.Name == commandNameFilter).ToList();
            }

            var commands = new List<WorkflowCommand>();

            foreach (var transitionDefinition in commandTransitions.Where(c => c.Condition.ConditionType == ConditionTypeEnumeration.Always))
            {
                var command = WorkflowCommand.Create(0, workFlowID, transitionDefinition); //Why 0 in processID???-(Ask Arpitha)

                command.LocalizedName = processDefinition.GetLocalizedCommandName(command.CommandName,
                                                                                CultureInfo.CurrentCulture);

                commands.Add(command);
            }



            return commands;

        }

        public IEnumerable<WorkflowCommand> GetAvailableCommands(long processId, long identityId, long workflowID)
        {
            return GetAvailableCommands(processId, workflowID, new List<long>() { identityId });
        }


        public void ExecuteCommand(long processId, long workflowID, Guid identityId, Guid impersonatedIdentityId, WorkflowCommand command)
        {
            Logger.Log.Debug(String.Format("Execution of command  started for processId {0} ", processId));

            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);

            SetProcessNewStatus(processInstance, ProcessStatus.Running);

            IEnumerable<TransitionDefinition> transitions;


            try
            {
                PersistenceProvider.FillSystemProcessParameters(processInstance);

                if (processInstance.CurrentActivityName != command.ValidForActivityName)
                {

                    throw new CommandNotValidForStateException();
                }

                transitions =
                    processInstance.Workflow.GetCommandTransitionForActivity(
                        processInstance.Workflow.FindActivity(processInstance.CurrentActivityName),
                        command.CommandName);

                if (transitions.Count() < 1)
                {
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw;
            }


            var parametersLocal = new List<ParameterDefinitionWithValue>();

            try
            {

                foreach (var commandParameter in command.Parameters)
                {
                    var parameterDefinition = processInstance.Workflow.GetParameterDefinition(commandParameter.Name);

                    if (parameterDefinition != null)
                        parametersLocal.Add(ParameterDefinition.Create(parameterDefinition, commandParameter.Value));

                }

                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterCurrentCommand,
                                                               (object)command.CommandName));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterIdentityId, identityId));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterImpersonatedIdentityId,
                                                               impersonatedIdentityId));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterSchemeId,
                                                               processInstance.SchemeId));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterProcessInstance,
                                                               processInstance));

                parametersLocal.ForEach(processInstance.AddParameter);

                PersistenceProvider.SavePersistenceParameters(processInstance);
                PersistenceProvider.FillPersistedProcessParameters(processInstance);
            }
            catch (Exception)
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw;
            }


            try
            {
                var newExecutionParameters = new List<ExecutionRequestParameters>();
                newExecutionParameters.AddRange(
                    transitions.Select(
                        at =>
                        ExecutionRequestParameters.Create(processInstance.ProcessId, processInstance.WorkFlowID, processInstance.ProcessParameters, at)));
                Logger.Log.Debug(String.Format("QueueExecution started for ProcessID {0}", processId));
                Bus.QueueExecution(newExecutionParameters);
            }
            catch (ConditionFailedException ex)
            {
                Logger.Log.Error(string.Format("Error Execute Command Workflow Id={0}", processInstance.ProcessId), ex);
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("Error Execute Command Workflow Id={0}", processInstance.ProcessId), ex);
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw;
            }
           


        }

        public IEnumerable<WorkflowState> GetAvailableStateToSet(long processId, CultureInfo culture, long workflowID)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            var activities = processInstance.Workflow.Activities.Where(a => a.IsForSetState && a.IsState);
            return
                activities.Select(
                    activity =>
                    new WorkflowState
                        {
                            Name = activity.State,
                            VisibleName = processInstance.GetLocalizedStateName(activity.State, culture),
                            WorkFlowName = processInstance.Workflow.Name
                        }).ToList();
        }

        public IEnumerable<WorkflowState> GetAvailableStateToSet(string workflowName, CultureInfo culture)
        {
            var workflow = Builder.GetWorkflowDefinition(workflowName);
            var activities = workflow.Activities.Where(a => a.IsForSetState && a.IsState);
            return
                activities.Select(
                    activity =>
                    new WorkflowState
                        {
                            Name = activity.State,
                            VisibleName = workflow.GetLocalizedStateName(activity.State, culture),
                            WorkFlowName = workflow.Name
                        })
                          .ToList();
        }


        public string GetCurrentStateName(long processId, long workflowID)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);

            return processInstance.GetParameter(DefaultDefinitions.ParameterCurrentState.Name).Value.ToString();
        }

        public string GetCurrentActivityName(long processId, long workflowID)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);

            return processInstance.GetParameter(DefaultDefinitions.ParameterCurrentActivity.Name).Value.ToString();
        }

        public IEnumerable<WorkflowState> GetAvailableStateToSet(long processId, long workflowID)
        {
            return GetAvailableStateToSet(processId, CultureInfo.CurrentCulture, workflowID);
        }

        public IEnumerable<WorkflowState> GetAvailableStateToSet(string workflowName)
        {
            return GetAvailableStateToSet(workflowName, CultureInfo.CurrentCulture);
        }

        public void SetState(long processId, long workflowID, Guid identityId, Guid impersonatedIdentityId, string stateName, IDictionary<string, object> parameters, bool preventExecution)
        {
            Logger.Log.Debug(String.Format("SetState is started for processId {0} ", processId));
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            var activityToSet =
                processInstance.Workflow.Activities.FirstOrDefault(
                    a => a.IsState && a.IsForSetState && a.State == stateName);

            if (activityToSet == null)
                throw new ActivityNotFoundException();

            if (!preventExecution)
                SetStateWithExecution(identityId, impersonatedIdentityId, parameters, activityToSet, processInstance);
            else
                SetStateWithoutExecution(activityToSet, processInstance);
        }

        private void SetStateWithoutExecution(ActivityDefinition activityToSet, ProcessInstance processInstance)
        {
            Logger.Log.Debug(String.Format("SetStateWithoutExecution is initiated for processId {0} ", processInstance.ProcessId));
            SetProcessNewStatus(processInstance, ProcessStatus.Running);

            IEnumerable<TransitionDefinition> transitions;
            try
            {
                PersistenceProvider.FillSystemProcessParameters(processInstance);
                var from = processInstance.CurrentActivity;
                var to = activityToSet;
                PersistenceProvider.UpdatePersistenceState(processInstance, TransitionDefinition.Create(from, to));
                Logger.Log.Debug(String.Format("SetStateWithoutExecution is completed for processId {0} ", processInstance.ProcessId));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("Workflow Id={0}", processInstance.ProcessId), ex);
                throw;
            }
            finally
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
            }
        }

        private void SetStateWithExecution(Guid identityId,
                                           Guid impersonatedIdentityId,
                                           IDictionary<string, object> parameters,
                                           ActivityDefinition activityToSet,
                                           ProcessInstance processInstance)
        {
            Logger.Log.Debug(String.Format("SetStateWithExecution is started for processId {0} ", processInstance.ProcessId));
            SetProcessNewStatus(processInstance, ProcessStatus.Running);

            IEnumerable<TransitionDefinition> transitions;


            try
            {
                PersistenceProvider.FillSystemProcessParameters(processInstance);
                PersistenceProvider.FillPersistedProcessParameters(processInstance);

                var parametersLocal = new List<ParameterDefinitionWithValue>();
                foreach (var commandParameter in parameters)
                {
                    var parameterDefinition = processInstance.Workflow.GetParameterDefinition(commandParameter.Key);

                    if (parameterDefinition != null)
                        parametersLocal.Add(ParameterDefinition.Create(parameterDefinition, commandParameter.Value));
                }
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterCurrentCommand,
                                                               (object)DefaultDefinitions.CommandSetState.Name));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterIdentityId, identityId));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterImpersonatedIdentityId,
                                                               impersonatedIdentityId));
                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterSchemeId, processInstance.SchemeId));

                parametersLocal.Add(ParameterDefinition.Create(DefaultDefinitions.ParameterProcessInstance, processInstance));

                parametersLocal.ForEach(processInstance.AddParameter);

            }
            catch (Exception ex)
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw ex;
            }

            //TODO Убрать после обработки командf
            try
            {
                Logger.Log.Debug(String.Format("QueueExecution is started for processId {0} ", processInstance.ProcessId));
                Bus.QueueExecution(ExecutionRequestParameters.Create(processInstance.ProcessId,
                     processInstance.WorkFlowID,
                                                                      processInstance.ProcessParameters,
                                                                      activityToSet,
                                                                      ConditionDefinition.Always));
                Logger.Log.Debug(String.Format("SetStateWithExecution is completed for processId {0} ", processInstance.ProcessId));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(string.Format("Workflow Id={0}", processInstance.ProcessId), ex);
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                throw ex;
            }
        }

        public void SetState(long processId, long workflowID, Guid identityId, Guid impersonatedIdentityId, string stateName, IDictionary<string, object> parameters)
        {

            SetState(processId, workflowID, identityId, impersonatedIdentityId, stateName, parameters, false);

        }

        private IEnumerable<Guid> GetActors(long processId, TransitionDefinition transition)
        {
            if (transition.Restrictions.Count() < 1)
                return new List<Guid>();

            List<Guid> result = null;
            //TODO Здесь возможно обрабатывать случай - запрещено только одному
            foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.RestrictionType == RestrictionTypeEnumeration.Allow))
            {
                var allowed = new List<Guid>();
                var actorDefinitionIsIdentity = restrictionDefinition.Actor as ActorDefinitionIsIdentity;
                if (actorDefinitionIsIdentity != null)
                    allowed.Add(actorDefinitionIsIdentity.IdentityId);

                var actorDefinitionIsInRole = restrictionDefinition.Actor as ActorDefinitionIsInRole;
                if (actorDefinitionIsInRole != null)
                    allowed.AddRange(RoleProvider.GetAllInRole(actorDefinitionIsInRole.RoleId));

                var actorDefinitionExecute = restrictionDefinition.Actor as ActorDefinitionExecuteRule;
                if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count < 1)
                    allowed.AddRange(RuleProvider.GetIdentitiesForRule(processId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters));
                else if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count > 0)
                    allowed.AddRange(RuleProvider.GetIdentitiesForRule(processId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters));

                if (result == null || result.Count() < 1)
                    result = allowed;
                else
                    result = result.Intersect(allowed).ToList();
            }

            if (result == null)
                return new List<Guid>();
            if (result.Count() < 1)
                return result;

            foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.RestrictionType == RestrictionTypeEnumeration.Restrict))
            {
                var restricted = new List<Guid>();
                var actorDefinitionIsIdentity = restrictionDefinition.Actor as ActorDefinitionIsIdentity;
                if (actorDefinitionIsIdentity != null)
                    restricted.Add(actorDefinitionIsIdentity.IdentityId);

                var actorDefinitionIsInRole = restrictionDefinition.Actor as ActorDefinitionIsInRole;
                if (actorDefinitionIsInRole != null)
                    restricted.AddRange(RoleProvider.GetAllInRole(actorDefinitionIsInRole.RoleId));

                var actorDefinitionExecute = restrictionDefinition.Actor as ActorDefinitionExecuteRule;
                if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count < 1)
                    restricted.AddRange(RuleProvider.GetIdentitiesForRule(processId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters));
                else if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count > 0)
                    restricted.AddRange(RuleProvider.GetIdentitiesForRule(processId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters));

                result.RemoveAll(p => restricted.Contains(p));
                if (result.Count() < 1)
                    return result;
            }

            return result;

        }

        private bool ValidateActor(long processId, long identityId, TransitionDefinition transition)
        {
            if (transition.Restrictions.Count() < 1)
                return true;

            var identityRestrictionSpecified = false;
            var identityAllowed = true;
            var ruleAllowed = true;
            var roleAllowed = false;

            //First filter all Identity rules
            //foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.Actor is ActorDefinitionIsIdentity))
            //{
            //    var actorDefinitionIsIdentity = restrictionDefinition.Actor as ActorDefinitionIsIdentity;

            //    if (actorDefinitionIsIdentity.IdentityId == identityId &&
            //         restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow)
            //    {
            //        identityAllowed = true;
            //        identityRestrictionSpecified = true;
            //    }

            //    else if (actorDefinitionIsIdentity.IdentityId == identityId &&
            //         restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict)
            //    {
            //        return false;
            //    }
            //}

            //Filter all role rules
            var notAllowedRoles = transition.Restrictions.Where(r => r.Actor is ActorDefinitionIsInRole &&
                                                                r.RestrictionType == RestrictionTypeEnumeration.Restrict)
                                                         .Select(r => (r.Actor as ActorDefinitionIsInRole).Name);

            var allowedRoles = transition.Restrictions.Where(r => r.Actor is ActorDefinitionIsInRole &&
                                                                r.RestrictionType == RestrictionTypeEnumeration.Allow &&
                                                                !notAllowedRoles.Contains((r.Actor as ActorDefinitionIsInRole).Name))
                                                      .Select(r => (r.Actor as ActorDefinitionIsInRole).Name);

            //Put a debug that these roles can perform
            foreach (var roleName in allowedRoles)
            {
                if (!RoleProvider.IsInRole(identityId, roleName, processId))
                    continue;

                roleAllowed = true;
            }

            //Filter all execute rule rules

            //foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.Actor is ActorDefinitionExecuteRule))
            //{
            //    var actorDefinitionExecute = restrictionDefinition.Actor as ActorDefinitionExecuteRule;
            //    if ((restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow &&
            //             !RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters)) ||
            //            (restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict &&
            //             RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters)))
            //        return false;
            //    continue;
            //}

            return (identityAllowed && roleAllowed && ruleAllowed) ||
                ((identityRestrictionSpecified && identityAllowed) && !roleAllowed && ruleAllowed);

            //First filter all Identity rules
            //foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.Actor is ActorDefinitionIsIdentity))
            //{
            //    var actorDefinitionIsIdentity = restrictionDefinition.Actor as ActorDefinitionIsIdentity;
            //    if (actorDefinitionIsIdentity != null)
            //    {
            //        if ((actorDefinitionIsIdentity.IdentityId != identityId &&
            //             restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow) ||
            //            (actorDefinitionIsIdentity.IdentityId == identityId &&
            //             restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict))
            //            continue;
            //        return true;
            //    }

            //}

            ////Filter all role rules
            //foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.Actor is ActorDefinitionIsInRole))
            //{
            //    var actorDefinitionIsInRole = restrictionDefinition.Actor as ActorDefinitionIsInRole;
            //    if (actorDefinitionIsInRole != null)
            //    {
            //        if ((restrictionDefinition.RestrictionType.Id == (int)RestrictionTypeEnumeration.Allow &&
            //             !RoleProvider.IsInRole(identityId, actorDefinitionIsInRole.RoleId, processId)) ||
            //            (restrictionDefinition.RestrictionType.Id == (int)RestrictionTypeEnumeration.Restrict &&
            //             RoleProvider.IsInRole(identityId, actorDefinitionIsInRole.RoleId, processId)))
            //            continue;
            //        return true;
            //    }
            //}

            ////Filter all execute rule rules

            //foreach (var restrictionDefinition in transition.Restrictions.Where(r => r.Actor is ActorDefinitionExecuteRule))
            //{
            //    var actorDefinitionExecute = restrictionDefinition.Actor as ActorDefinitionExecuteRule;
            //    if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count < 1)
            //    {
            //        if ((restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow &&
            //             !RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters)) ||
            //            (restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict &&
            //             RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters)))
            //            continue;
            //        return true;
            //    }
            //    if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count > 0)
            //    {
            //        if ((restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow &&
            //             !RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName,
            //                                     actorDefinitionExecute.Parameters)) ||
            //            (restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict &&
            //             RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName,
            //                                    actorDefinitionExecute.Parameters)))
            //            continue;
            //        return true;
            //    }
            //}

            //foreach (var restrictionDefinition in transition.Restrictions)
            //{
            //    var actorDefinitionIsIdentity = restrictionDefinition.Actor as ActorDefinitionIsIdentity;
            //    if (actorDefinitionIsIdentity != null)
            //    {
            //        if ((actorDefinitionIsIdentity.IdentityId != identityId &&
            //             restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow) ||
            //            (actorDefinitionIsIdentity.IdentityId == identityId &&
            //             restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict))
            //            return false;
            //        continue;
            //    }

            //    var actorDefinitionIsInRole = restrictionDefinition.Actor as ActorDefinitionIsInRole;
            //    if (actorDefinitionIsInRole != null)
            //    {
            //        if ((restrictionDefinition.RestrictionType.Id == (int)RestrictionTypeEnumeration.Allow &&
            //             !RoleProvider.IsInRole(identityId, actorDefinitionIsInRole.RoleId, processId)) ||
            //            (restrictionDefinition.RestrictionType.Id == (int)RestrictionTypeEnumeration.Restrict &&
            //             RoleProvider.IsInRole(identityId, actorDefinitionIsInRole.RoleId, processId)))
            //            return false;
            //        continue;
            //    }

            //    var actorDefinitionExecute = restrictionDefinition.Actor as ActorDefinitionExecuteRule;
            //    if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count < 1)
            //    {
            //        if ((restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow &&
            //             !RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters.FirstOrDefault().Value)) ||
            //            (restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict &&
            //             RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName, actorDefinitionExecute.Parameters.FirstOrDefault().Value)))
            //            return false;
            //        continue;
            //    }
            //    if (actorDefinitionExecute != null && actorDefinitionExecute.Parameters.Count > 0)
            //    {
            //        if ((restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Allow &&
            //             !RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName,
            //                                     actorDefinitionExecute.Parameters)) ||
            //            (restrictionDefinition.RestrictionType == RestrictionTypeEnumeration.Restrict &&
            //             RuleProvider.CheckRule(processId, identityId, actorDefinitionExecute.RuleName,
            //                                    actorDefinitionExecute.Parameters)))
            //            return false;
            //        continue;
            //    }
            //}

            return false;
        }


        public bool IsProcessExists(long processId, long workFlowID)
        {
            return PersistenceProvider.IsProcessExists(processId, workFlowID);
        }

        public ProcessStatus GetProcessStatus(long processId, long workflowID)
        {
            return PersistenceProvider.GetInstanceStatus(processId, workflowID);
        }

        private void SetProcessNewStatus(ProcessInstance processInstance, ProcessStatus newStatus)
        {

            var oldStatus = PersistenceProvider.GetInstanceStatus(processInstance.ProcessId, processInstance.WorkFlowID);
            if (newStatus == ProcessStatus.Finalized)
                PersistenceProvider.SetWorkflowFinalized(processInstance);
            else if (newStatus == ProcessStatus.Idled)
                PersistenceProvider.SetWorkflowIdled(processInstance);
            else if (newStatus == ProcessStatus.Initialized)
                PersistenceProvider.SetWorkflowIniialized(processInstance);
            else if (newStatus == ProcessStatus.Running)
                PersistenceProvider.SetWorkflowRunning(processInstance);
            else if (newStatus == ProcessStatus.Terminated)
                PersistenceProvider.SetWorkflowTerminated(processInstance, ErrorLevel.Critical, "Terminated");
            else
            {
                return;
            }

            if (ProcessStatusChanged != null)
                ProcessStatusChanged(this, new ProcessStatusChangedEventArgs(processInstance.ProcessId, oldStatus, newStatus, processInstance.WorkFlowID, processInstance) { ProcessParameters = processInstance.ProcessParameters.ToList(), ProcessName = processInstance.Workflow.Name });
        }

        public DateTime RuntimeDateTimeNow { get; set; }

        public void PreExecute(long processId, long workflowID, WorkflowCommand command, bool ignoreCurrentStateCheck = false)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);

            //PreExecute(processId, fromActivityName, ignoreCurrentStateCheck, processInstance);
             PreExecute(processId, ignoreCurrentStateCheck, processInstance, command);
        }

        private void PreExecute(long processId, bool ignoreCurrentStateCheck, ProcessInstance processInstance, WorkflowCommand command)
        {
            var activity = processInstance.Workflow.FindActivity(processInstance.CurrentActivityName);
            var currentActivity = processInstance.Workflow.FindActivity(command.ValidForActivityName);
            if (!ignoreCurrentStateCheck && activity.State != currentActivity.State)
                return;

            var executor = new ActivityExecutor(true);


            processInstance.AddParameter(ParameterDefinition.Create(processInstance.ProcessParameters.Single(a => a.Name == DefaultDefinitions.ParameterProcessId.Name), processId));
            processInstance.AddParameter(ParameterDefinition.Create(processInstance.ProcessParameters.Single(a => a.Name == DefaultDefinitions.ParameterSchemeId.Name), processInstance.SchemeId));

            if (currentActivity.HavePreExecutionImplementation && currentActivity.IsInitial)
            {
                var response = executor.Execute(new List<ExecutionRequestParameters>
                                                    {
                                                        ExecutionRequestParameters.Create(processInstance.ProcessId,
                                                        processInstance.WorkFlowID,
                                                                                          processInstance.ProcessParameters,
                                                                                          //processInstance.ProcessScheme.InitialActivity,
                                                                                          currentActivity,
                                                                                          ConditionDefinition.Always,
                                                                                          true)
                                                    });

                if (PreExecuteProcessResponse(processInstance, response)) return;
            }

            do
            {
                if (!string.IsNullOrEmpty(currentActivity.State))
                    processInstance.AddParameter(ParameterDefinition.Create(processInstance.ProcessParameters.Single(a => a.Name == DefaultDefinitions.ParameterCurrentState.Name), (object)currentActivity.State));

                var transitions =
                    processInstance.Workflow.GetPossibleTransitionsForActivity(currentActivity).Where(t => t.TransitionClassifier == TransitionClassifierEnumeration.Direct);

                currentActivity = null;

                var autotransitions = transitions.Where(t => t.Trigger.Type == TriggerTypeEnumeration.Auto);

                var newExecutionParameters = FillExecutionRequestParameters(processId, processInstance, autotransitions);

                if (newExecutionParameters.Count > 0)
                {
                    var response = executor.Execute(newExecutionParameters);

                    if (!PreExecuteProcessResponse(processInstance, response))
                    {
                        currentActivity =
                            processInstance.Workflow.FindTransition(response.ExecutedTransitionName).To;
                    }
                }

                if (currentActivity == null)
                {
                    var commandTransitions = transitions.Where(t => t.Trigger.Type.Id == (int)TriggerTypeEnumeration.Command);

                    if (commandTransitions.Count(t => t.Condition.ConditionType.Id == (int)ConditionTypeEnumeration.Always && !t.Condition.ResultOnPreExecution.HasValue) < 2)
                    //Это не является ошибкой валидациии при разных командах
                    {
                        newExecutionParameters = FillExecutionRequestParameters(processId,
                                                                                processInstance,
                                                                                commandTransitions);

                        if (newExecutionParameters.Count > 0)
                        {
                            var response = executor.Execute(newExecutionParameters);

                            if (!PreExecuteProcessResponse(processInstance, response))
                            {
                                currentActivity =
                                    processInstance.Workflow.FindTransition(response.ExecutedTransitionName).To;
                            }
                        }
                    }
                }
            } while (currentActivity != null && !currentActivity.IsFinal);
        }

        public void PreExecuteFromInitialActivity(long processId, long workflowID, bool ignoreCurrentStateCheck = false)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);

            //PreExecute(processId, processInstance.Workflow.InitialActivity.Name, ignoreCurrentStateCheck, processInstance);
        }

        public void PreExecuteFromCurrentActivity(long processId, long workflowID, bool ignoreCurrentStateCheck = false)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillSystemProcessParameters(processInstance);

            //PreExecute(processId, processInstance.CurrentActivityName, ignoreCurrentStateCheck, processInstance);
        }

        private bool PreExecuteProcessResponse(ProcessInstance processInstance, ExecutionResponseParameters response)
        {
            if (response.IsEmplty)
                return true;

            if (!response.IsError)
                FillParameters(processInstance, response as ExecutionResponseParametersComplete);
            else
            {
                throw (response as ExecutionResponseParametersError).Exception;
            }
            return false;
        }

        private List<ExecutionRequestParameters> FillExecutionRequestParameters(long processId, ProcessInstance processInstance, IEnumerable<TransitionDefinition> transitions)
        {
            var newExecutionParameters = new List<ExecutionRequestParameters>();

            foreach (var transition in transitions)
            {
                var parametersLocal = ExecutionRequestParameters.Create(processInstance.ProcessId,
                                                                            processInstance.WorkFlowID,
                                                                        processInstance.ProcessParameters,
                                                                        transition, true);

                if (transition.Trigger.Type.Id != (int)TriggerTypeEnumeration.Auto || transition.Restrictions.Count() > 0)
                {
                    var actors = GetActors(processId, transition);

                    parametersLocal.AddParameterInContainer(
                        ParameterDefinition.Create(DefaultDefinitions.ParameterIdentityIds,
                                                   actors));
                }

                if (transition.Trigger.Type.Id == (int)TriggerTypeEnumeration.Command)
                    parametersLocal.AddParameterInContainer(ParameterDefinition.Create(DefaultDefinitions.ParameterCurrentCommand, (object)transition.Trigger.Command.Name));


                newExecutionParameters.Add(parametersLocal);
            }
            return newExecutionParameters;
        }

        /// <summary>
        /// If the scheme is in scheme persistent store marked as obsolete. Upgrades scheme.
        /// </summary>
        /// <param name="processId">Process instance id</param>
        public void UpdateSchemeIfObsolete(long processId, long workflowID)
        {
            UpdateSchemeIfObsolete(processId, workflowID, new Dictionary<string, IEnumerable<object>>());
        }

        /// <summary>
        /// If the scheme is in scheme persistent store marked as obsolete. Upgrades scheme.
        /// </summary>
        /// <param name="processId">Process instance id</param>
        /// <param name="parameters">Defining parameters of process</param>
        public void UpdateSchemeIfObsolete(long processId, long workflowID, IDictionary<string, IEnumerable<object>> parameters)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            var isSchemeObsolete = processInstance.IsSchemeObsolete;
            var isDeterminingParametersChanged = processInstance.IsDeterminingParametersChanged;

            if (!isSchemeObsolete && !isDeterminingParametersChanged)
                return;

            SetProcessNewStatus(processInstance, ProcessStatus.Running);

            try
            {
                processInstance = Builder.CreateNewWorkflowScheme(processId, processInstance.Workflow.Name, parameters);
                PersistenceProvider.BindProcessToNewScheme(processInstance, true);
                if (OnSchemaWasChanged != null)
                    OnSchemaWasChanged(this,
                                       new SchemaWasChangedEventArgs
                                           {
                                               DeterminingParametersWasChanged = isDeterminingParametersChanged,
                                               ProcessId = processId,
                                               SchemeId = processInstance.SchemeId,
                                               SchemaWasObsolete = isSchemeObsolete
                                           });
            }
            finally
            {
                SetProcessNewStatus(processInstance, ProcessStatus.Idled);
            }

        }


        private ProcessInstance UpdateScheme(long processId, long workflowID, ProcessInstance processInstance)
        {
            if (processInstance.CurrentActivity.IsAutoSchemeUpdate && (processInstance.IsSchemeObsolete || processInstance.IsDeterminingParametersChanged))
            {
                try
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Running);
                    processInstance = Builder.GetWorkflowInstance(processId, workflowID);
                    PersistenceProvider.FillSystemProcessParameters(processInstance);

                    var isSchemeObsolete = processInstance.IsSchemeObsolete;
                    var isDeterminingParametersChanged = processInstance.IsDeterminingParametersChanged;

                    if (processInstance.CurrentActivity.IsAutoSchemeUpdate && (isSchemeObsolete || isDeterminingParametersChanged))
                    {
                        var args = new NeedDeterminingParametersEventArgs { ProcessId = processId };
                        if (OnNeedDeterminingParameters != null)
                            OnNeedDeterminingParameters(this, args);

                        if (args.DeterminingParameters == null)
                            args.DeterminingParameters = new Dictionary<string, IEnumerable<object>>();

                        processInstance = Builder.CreateNewWorkflowScheme(processId, processInstance.Workflow.Name,
                                                                          args.DeterminingParameters);
                        PersistenceProvider.BindProcessToNewScheme(processInstance, true);
                        if (OnSchemaWasChanged != null)
                            OnSchemaWasChanged(this,
                                               new SchemaWasChangedEventArgs
                                                   {
                                                       DeterminingParametersWasChanged = isDeterminingParametersChanged,
                                                       ProcessId = processId,
                                                       SchemeId = processInstance.SchemeId,
                                                       SchemaWasObsolete = isSchemeObsolete
                                                   });
                        PersistenceProvider.FillSystemProcessParameters(processInstance);
                    }
                }
                finally
                {
                    SetProcessNewStatus(processInstance, ProcessStatus.Idled);
                }
            }
            return processInstance;
        }

        public string GetLocalizedStateName(long processId, long workflowID, string stateName)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            return processInstance.GetLocalizedStateName(processInstance.CurrentState, CultureInfo.CurrentCulture);
        }

        public string GetLocalizedCommandName(long processId, long workflowID, string commandName)
        {
            var processInstance = Builder.GetWorkflowInstance(processId, workflowID);
            return processInstance.GetLocalizedCommandName(commandName, CultureInfo.CurrentCulture);
        }

        //public string GetLocalizedCommandNameBySchemeId(Guid schemeId, string commandName)
        //{
        //    var processscheme =  Builder.GetProcessScheme(schemeId);
        //    return processscheme.GetLocalizedCommandName(commandName, CultureInfo.CurrentCulture);
        //}

        //public string GetLocalizedStateNameBySchemeId(Guid schemeId, string stateName)
        //{
        //    var processscheme = Builder.GetProcessScheme(schemeId);
        //    return processscheme.GetLocalizedStateName(stateName, CultureInfo.CurrentCulture);
        //}

        public ProcessInstance GetProcessInstanceAndFillProcessParameters(long processId, long workflowID)
        {
            var pi = Builder.GetWorkflowInstance(processId, workflowID);
            PersistenceProvider.FillProcessParameters(pi);
            return pi;
        }
    }
}
