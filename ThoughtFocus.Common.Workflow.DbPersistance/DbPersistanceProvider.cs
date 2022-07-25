using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Fault;
using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.PersistanceModel;
using ThoughtFocus.Common.Workflow.Core.Persistence;
using ThoughtFocus.Common.WorkFlowDataAccess;

namespace ThoughtFocus.Common.Workflow.DbPersistance
{
    public sealed class DbPersistenceProvider : DbProvider, IPersistenceProvider
    {
        private Core.Runtime.WorkflowRuntime _runtime;
        private WorkFlowContext _context;
        public DbPersistenceProvider(string connectionString)
            : base(connectionString)
        {
        }

        public DbPersistenceProvider(WorkFlowContext context, string AppDBConnectionString)
            : base(AppDBConnectionString)
        {
            _context = context;
        }


        public void Init(Core.Runtime.WorkflowRuntime runtime)
        {
            _runtime = runtime;
        }

        public void InitializeProcess(ProcessInstance processInstance)
        {
            try
            {
                //using (var scope = PredefinedTransactionScopes.ReadCommittedSupressedScope)
                //{
                    using (var context = CreateContext())
                    {
                        var oldProcess = _context.WorkflowProcessInstances.SingleOrDefault(wpi => wpi.ProcessInstanceID == processInstance.ProcessId && wpi.WorkflowDefinitionID == processInstance.WorkFlowID);
                        if (oldProcess != null)
                            throw new ProcessAlredyExistsException();
                        var newProcess = new WorkflowProcessInstance
                        {
                            ProcessInstanceID = processInstance.ProcessId,
                            SchemeId = processInstance.SchemeId,
                            ActivityName = processInstance.Workflow.InitialActivity.Name,
                            StateName = processInstance.Workflow.InitialActivity.State,
                            WorkflowDefinitionID = processInstance.WorkFlowID
                        };
                        _context.WorkflowProcessInstances.Add(newProcess);
                        _context.SaveChanges();
                    //}
                    //scope.Complete();
                }
            }
            catch(Exception ex)
            {
                Logger.Log.Error(String.Format("{0} {1}", Extensions.GetMessage("Exception occured in InitializeProcess with processID", processInstance.ProcessId), Extensions.GetMessage("exception", ex)));
                throw ex;
            }
        }

        public void BindProcessToNewScheme(ProcessInstance processInstance)
        {
            BindProcessToNewScheme(processInstance, false);
        }

        public void BindProcessToNewScheme(ProcessInstance processInstance, bool resetIsDeterminingParametersChanged)
        {
            using (var scope = PredefinedTransactionScopes.ReadCommittedSupressedScope)
            {

                using (var context = CreateContext())
                {
                    var oldProcess =
                        _context.WorkflowProcessInstances.SingleOrDefault(wpi => wpi.ProcessInstanceID == processInstance.ProcessId && wpi.WorkflowDefinitionID==processInstance.WorkFlowID);
                    if (oldProcess == null)
                        throw new ProcessNotFoundException();
                    oldProcess.SchemeId = processInstance.SchemeId;
                    if (resetIsDeterminingParametersChanged)
                        oldProcess.IsDeterminingParametersChanged = false;
                    _context.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void RegisterTimer(long processId, string name, DateTime nextExecutionDateTime, bool notOverrideIfExists)
        {
            using (var context = CreateContext())
            {
                var oldTimer =
                    _context.WorkflowProcessTimers.SingleOrDefault(wpt => wpt.ProcessInstanceID == processId && wpt.Name == name);

                if (oldTimer == null)
                {
                    oldTimer = new WorkflowProcessTimer()
                    {
                        WorkflowProcessTimerID = Guid.NewGuid(),
                        Name = name,
                        NextExecutionDateTime = nextExecutionDateTime,
                        //Add WorkFlowID
                        ProcessInstanceID = processId
                    };

                    oldTimer.Ignore = false;

                    _context.WorkflowProcessTimers.Add(oldTimer);
                }

                if (!notOverrideIfExists)
                {
                    oldTimer.NextExecutionDateTime = nextExecutionDateTime;
                }

                context.SaveChanges();
            }
        }

        public void ClearTimers(long processId, List<string> timersIgnoreList)
        {
            using (var context = CreateContext())
            {
                var timers = _context.WorkflowProcessTimers.Where(wpt => wpt.ProcessInstanceID == processId && !timersIgnoreList.Contains(wpt.Name)).ToList();

                _context.WorkflowProcessTimers.RemoveRange(timers);

                _context.SaveChanges();
            }
        }

        public void ClearTimersIgnore()
        {
            using (var context = CreateContext())
            {
                var timers =
                    _context.WorkflowProcessTimers.Where(
                        wpt => wpt.Ignore).ToList();

                foreach (var timer in timers)
                {
                    timer.Ignore = false;
                }

                _context.SaveChanges();
            }

        }

        public void ClearTimer(Guid timerId)
        {
            using (var context = CreateContext())
            {
                var timer = _context.WorkflowProcessTimers.FirstOrDefault(t => t.WorkflowProcessTimerID == timerId);

                if (timer != null)
                {
                    _context.WorkflowProcessTimers.Remove(timer);

                    _context.SaveChanges();
                }

            }
        }

        public DateTime? GetCloseExecutionDateTime()
        {
            using (var context = CreateContext())
            {
                var timer =
                    _context.WorkflowProcessTimers.Where(t => !t.Ignore).OrderBy(wpt => wpt.NextExecutionDateTime).FirstOrDefault();

                if (timer == null)
                    return null;

                return timer.NextExecutionDateTime;
            }
        }



        public void FillProcessParameters(ProcessInstance processInstance)
        {
            Logger.Log.Debug(String.Format("Filling Processparameters  for processId {0} ", processInstance.ProcessId));
            processInstance.AddParameters(GetProcessParameters(processInstance.ProcessId, processInstance.Workflow));
        }

        public void FillPersistedProcessParameters(ProcessInstance processInstance)
        {
            processInstance.AddParameters(GetPersistedProcessParameters(processInstance.ProcessId, processInstance.Workflow));
        }

        public void FillSystemProcessParameters(ProcessInstance processInstance)
        {
            processInstance.AddParameters(GetSystemProcessParameters(processInstance.ProcessId, processInstance.Workflow));
        }

        public void SavePersistenceParameters(ProcessInstance processInstance)
        {
            var parametersToPersistList =
                processInstance.ProcessParameters.Where(ptp => ptp.Purpose == ParameterPurposeEnumeration.Persistence)
                    .Select(ptp => new { Parameter = ptp, SerializedValue = ptp.Value })
                    .ToList();
            var persistenceParameters = processInstance.Workflow.PersistenceParameters.ToList();

            using (var scope = PredefinedTransactionScopes.ReadUncommittedSupressedScope)
            {
                using (var context = CreateContext())
                {
                    var persistedParameters =
                       _context.WorkflowProcessInstancePersistences.Where(
                           wpip =>
                           wpip.ProcessInstanceID == processInstance.ProcessId &&
                           persistenceParameters.Select(pp => pp.Name).Contains(wpip.ParameterName)).ToList();

                    foreach (var parameterDefinitionWithValue in parametersToPersistList)
                    {
                        WorkflowProcessInstancePersistence persistence =
                            persistedParameters.SingleOrDefault(
                                pp => pp.ParameterName == parameterDefinitionWithValue.Parameter.Name);
                        {
                            if (persistence == null)
                            {
                                if (parameterDefinitionWithValue.SerializedValue != null)
                                {
                                    persistence = new WorkflowProcessInstancePersistence()
                                    {
                                        WorkflowProcessInstancePersistenceID = Guid.NewGuid(),
                                        ParameterName = parameterDefinitionWithValue.Parameter.Name,
                                        ProcessInstanceID = processInstance.ProcessId,
                                        Value = parameterDefinitionWithValue.SerializedValue.ToString(),
                                        WorkFlowDefinationID = processInstance.WorkFlowID
                                    };
                                    _context.WorkflowProcessInstancePersistences.Add(persistence);
                                }
                            }
                            else
                            {
                                if (parameterDefinitionWithValue.SerializedValue != null)
                                    persistence.Value = parameterDefinitionWithValue.SerializedValue.ToString();
                                else
                                    _context.WorkflowProcessInstancePersistences.Remove(persistence);
                            }
                        }
                    }

                    _context.SaveChanges();
                }

                scope.Complete();
            }
        }

        public void SetWorkflowIniialized(ProcessInstance processInstance)
        {
            using (var scope = PredefinedTransactionScopes.SerializableSupressedScope)
            {
                using (var context = CreateContext())
                {
                    var instanceStatus = _context.WorkflowProcessInstanceStatus.SingleOrDefault(wpis => wpis.ProcessInstanceID == processInstance.ProcessId && wpis.WorkFlowDefinationID==processInstance.WorkFlowID);
                    if (instanceStatus == null)
                    {
                        instanceStatus = new WorkflowProcessInstanceStatus()
                        {
                            
                            ProcessInstanceID = processInstance.ProcessId,
                            Lock = Guid.NewGuid(),
                            Status = ProcessStatus.Initialized.Id,
                            WorkFlowDefinationID = processInstance.WorkFlowID
                        };

                        _context.WorkflowProcessInstanceStatus.Add(instanceStatus);

                    }
                    else
                    {
                        instanceStatus.Status = ProcessStatus.Initialized.Id;
                    }

                    _context.SaveChanges();
                }

                scope.Complete();
            }
        }

        public void SetWorkflowIdled(ProcessInstance processInstance)
        {
            SetCustomStatus(processInstance.ProcessId, ProcessStatus.Idled);
        }

        public void SetWorkflowRunning(ProcessInstance processInstance)
        {
            using (var scope = PredefinedTransactionScopes.SerializableSupressedScope)
            {
                using (var context = CreateContext())
                {
                    var instanceStatus = _context.WorkflowProcessInstanceStatus.SingleOrDefault(wpis => wpis.ProcessInstanceID == processInstance.ProcessId && wpis.WorkFlowDefinationID == processInstance.WorkFlowID);
                    if (instanceStatus == null)
                        throw new StatusNotDefinedException();

                    if (instanceStatus.Status == ProcessStatus.Running.Id)
                        throw new ImpossibleToSetStatusException();

                    instanceStatus.Lock = Guid.NewGuid();
                    instanceStatus.Status = ProcessStatus.Running.Id;

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new ImpossibleToSetStatusException();
                    }
                }

                scope.Complete();
            }
        }

        public void SetWorkflowFinalized(ProcessInstance processInstance)
        {
            SetCustomStatus(processInstance.ProcessId, ProcessStatus.Finalized);
        }

        public void SetWorkflowTerminated(ProcessInstance processInstance, ErrorLevel level, string errorMessage)
        {
            SetCustomStatus(processInstance.ProcessId, ProcessStatus.Terminated);
        }

        public void ResetWorkflowRunning()
        {
            using (var context = CreateContext())
            {
                //context.spWorkflowProcessResetRunningStatus();//Change This to method
            }
        }

        public void UpdatePersistenceState(ProcessInstance processInstance, TransitionDefinition transition)
        {
            var paramIdentityId = processInstance.GetParameter(DefaultDefinitions.ParameterIdentityId.Name);
            var paramImpIdentityId = processInstance.GetParameter(DefaultDefinitions.ParameterImpersonatedIdentityId.Name);

            string identityId = paramIdentityId == null || paramIdentityId.Value == null
                ? string.Empty
                : paramIdentityId.Value.ToString();
            string impIdentityId = paramImpIdentityId == null || paramImpIdentityId.Value == null
                ? identityId
                : paramImpIdentityId.Value.ToString();

            using (var context = CreateContext())
            {
                WorkflowProcessInstance inst = _context.WorkflowProcessInstances.FirstOrDefault(c => c.ProcessInstanceID == processInstance.ProcessId && c.WorkflowDefinitionID==processInstance.WorkFlowID);
                if (inst != null)
                {
                    if (!string.IsNullOrEmpty(transition.To.State))
                        inst.StateName = transition.To.State;

                    inst.ActivityName = transition.To.Name;
                    inst.PreviousActivity = transition.From.Name;

                    if (!string.IsNullOrEmpty(transition.From.State))
                        inst.PreviousState = transition.From.State;

                    if (transition.TransitionClassifier.Id == (int)TransitionClassifierEnumeration.Direct)
                    {
                        inst.PreviousActivityForDirect = transition.From.Name;

                        if (!string.IsNullOrEmpty(transition.From.State))
                            inst.PreviousStateForDirect = transition.From.State;
                    }
                    else if (transition.TransitionClassifier.Id == (int)TransitionClassifierEnumeration.Reverse)
                    {
                        inst.PreviousActivityForReverse = transition.From.Name;
                        if (!string.IsNullOrEmpty(transition.From.State))
                            inst.PreviousStateForReverse = transition.From.State;
                    }
                }

                WorkflowProcessTransitionHistory history = new WorkflowProcessTransitionHistory()
                {
                    ActorIdentityId = impIdentityId,
                    ExecutorIdentityId = identityId,
                    WorkflowProcessTransitionHistoryID = Guid.NewGuid(),
                    IsFinalised = false,
                    WorkFlowDefinationID=processInstance.WorkFlowID,
                    //TODO Зачем на м финализед тут????
                    ProcessInstanceID = processInstance.ProcessId,
                    FromActivityName = transition.From.Name,
                    FromStateName = transition.From.State,
                    ToActivityName = transition.To.Name,
                    ToStateName = transition.To.State,
                    TransitionClassifier = transition.TransitionClassifier.Id.ToString(),
                    TransitionTime = DateTime.Now, //_runtime.RuntimeDateTimeNow,//Was Not Defined Just added in WorkFlowRuntime--
                    TriggerName = string.IsNullOrEmpty(processInstance.ExecutedTimer) ? processInstance.CurrentCommand : processInstance.ExecutedTimer
                };


                _context.WorkflowProcessTransitionHistories.Add(history);

                _context.SaveChanges();
            }

        }

        public bool IsProcessExists(long processId,long workflowID)
        {
            using (var context = CreateContext())
            {
                return _context.WorkflowProcessInstances.Count(wpi => wpi.ProcessInstanceID == processId && wpi.WorkflowDefinitionID==workflowID) > 0;
            }
        }

        public ProcessStatus GetInstanceStatus(long processId,long workflowID)
        {
            using (var context = CreateContext())
            {
                var instance = _context.WorkflowProcessInstanceStatus.FirstOrDefault(wpis => wpis.ProcessInstanceID == processId && wpis.WorkFlowDefinationID==workflowID);
                if (instance == null)
                    return ProcessStatus.NotFound;
                var status = ProcessStatus.All.FirstOrDefault(ins => ins.Id == instance.Status);
                if (status == null)
                    return ProcessStatus.Unknown;
                return status;
            }
        }

        private void SetCustomStatus(long processId, ProcessStatus status)
        {
            using (var scope = PredefinedTransactionScopes.SerializableSupressedScope)
            {
                using (var context = CreateContext())
                {
                    var instanceStatus = _context.WorkflowProcessInstanceStatus.SingleOrDefault(wpis => wpis.ProcessInstanceID == processId);
                    if (instanceStatus == null)
                        throw new StatusNotDefinedException();
                    instanceStatus.Status = status.Id;

                    _context.SaveChanges();
                }

                scope.Complete();
            }
        }

        private IEnumerable<ParameterDefinitionWithValue> GetProcessParameters(long processId, WorkflowDefinition workflowDefinition)
        {
            Logger.Log.Debug(String.Format("Getting ProcessParameters  for processId {0} ", processId));
            var parameters = new List<ParameterDefinitionWithValue>(workflowDefinition.Parameters.Count());
            parameters.AddRange(GetPersistedProcessParameters(processId, workflowDefinition));
            parameters.AddRange(GetSystemProcessParameters(processId, workflowDefinition));
            Logger.Log.Debug(String.Format("Got ProcessParameters  for processId {0} ", processId));
            return parameters;
        }

        private IEnumerable<ParameterDefinitionWithValue> GetSystemProcessParameters(long processId,
            WorkflowDefinition workflowDefinition)
        {
            Logger.Log.Debug(String.Format("Getting SystemProcessParameters  for processId {0} ", processId));
            var processInstance = GetProcessInstance(processId,workflowDefinition.ID);

            var systemParameters =
                workflowDefinition.Parameters.Where(p => p.Purpose == ParameterPurposeEnumeration.System).ToList();

            List<ParameterDefinitionWithValue> parameters=null;
            if (systemParameters.Count > 0)
            {
                parameters = new List<ParameterDefinitionWithValue>(systemParameters.Count())
            {
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterProcessId.Name),
                    processId),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousState.Name),
                    (object) processInstance.PreviousState),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterCurrentState.Name),
                    (object) processInstance.StateName),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousStateForDirect.Name),
                    (object) processInstance.PreviousStateForDirect),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousStateForReverse.Name),
                    (object) processInstance.PreviousStateForReverse),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousActivity.Name),
                    (object) processInstance.PreviousActivity),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterCurrentActivity.Name),
                    (object) processInstance.ActivityName),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousActivityForDirect.Name),
                    (object) processInstance.PreviousActivityForDirect),
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterPreviousActivityForReverse.Name),
                    (object) processInstance.PreviousActivityForReverse),              
                ParameterDefinition.Create(
                    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterSchemeId.Name),
                    (object) processInstance.SchemeId),
                //ParameterDefinition.Create(
                //    systemParameters.Single(sp => sp.Name == DefaultDefinitions.ParameterProcessInstance.Name),
                //    (object) processInstance)
            };
            }
            Logger.Log.Debug(String.Format("Got SystemProcessParameters  for processId {0} ", processId));
            return parameters;
        }

        private IEnumerable<ParameterDefinitionWithValue> GetPersistedProcessParameters(long processId, WorkflowDefinition workflowDefinition)
        {
            Logger.Log.Debug(String.Format("Getting  PersistedProcessParameters  for processId {0} ", processId));
            var persistenceParameters = workflowDefinition.PersistenceParameters.ToList();
            var parameters = new List<ParameterDefinitionWithValue>(persistenceParameters.Count());

            List<WorkflowProcessInstancePersistence> persistedParameters;
            using (PredefinedTransactionScopes.ReadUncommittedSupressedScope)
            {
                using (var context = CreateContext())
                {
                    persistedParameters =
                        _context.WorkflowProcessInstancePersistences.Where(
                            wpip =>
                            wpip.ProcessInstanceID == processId &&
                            persistenceParameters.Select(pp => pp.Name).Contains(wpip.ParameterName)).ToList();
                }
            }

            foreach (var persistedParameter in persistedParameters)
            {
                ParameterDefinition parameterDefinition = persistenceParameters.Single(p => p.Name == persistedParameter.ParameterName);
                //parameters.Add(ParameterDefinition.Create(parameterDefinition, (object)_runtime.DeserializeParameter(persistedParameter.Value, parameterDefinition.Type)));
                //Remove Object Conversion Once You get Deserialize Defination
            }
            Logger.Log.Debug(String.Format("Got PersistedProcessParameters  for processId {0} ", processId));
            return parameters;
        }

        private WorkflowProcessInstance GetProcessInstance(long processId,long workFlowID)
        {
            using (PredefinedTransactionScopes.ReadCommittedSupressedScope)
            {
                using (var context = CreateContext())
                {
                    var processInstance = _context.WorkflowProcessInstances.SingleOrDefault(wpi => wpi.ProcessInstanceID == processId && wpi.WorkflowDefinitionID == workFlowID);
                    if (processInstance == null)
                        throw new ProcessNotFoundException();
                    processInstance.WorkflowDefinition = _context.WorkflowDefinition.FirstOrDefault(a => a.ID == workFlowID);
                    return processInstance;
                }
            }
        }

        public void DeleteProcess(long processId)
        {
            using (var context = CreateContext())
            {
                var wpi = _context.WorkflowProcessInstances.Where(c => c.ProcessInstanceID == processId).FirstOrDefault();
                if (wpi != null)
                    _context.WorkflowProcessInstances.Remove(wpi);

                var wpis = _context.WorkflowProcessInstanceStatus.Where(c => c.ProcessInstanceID == processId ).FirstOrDefault();
                if (wpis != null)
                    _context.WorkflowProcessInstanceStatus.Remove(wpis);

                var wpths = _context.WorkflowProcessTransitionHistories.Where(c => c.ProcessInstanceID == processId).ToArray();
                _context.WorkflowProcessTransitionHistories.RemoveRange(wpths);

                var wpts = _context.WorkflowProcessTimers.Where(c => c.ProcessInstanceID == processId).ToArray();
                _context.WorkflowProcessTimers.RemoveRange(wpts);

                _context.SaveChanges();
            }
        }

        public void DeleteProcess(long[] processIds)
        {
            foreach (var p in processIds)
                DeleteProcess(p);
        }
        //public List<TimerToExecute> GetTimersToExecute()
        //{
        //    using (var context = CreateContext())
        //    {
        //        var now = _runtime.RuntimeDateTimeNow;
        //        var timers =
        //            context.WorkflowProcessTimers.Where(t => !t.Ignore && t.NextExecutionDateTime <= now).ToList();
        //        foreach (var timer in timers)
        //        {
        //            timer.Ignore = true;
        //        }
        //        context.SaveChanges();
        //        return timers.Select(t => new TimerToExecute() {Name = t.Name, ProcessId = t.ProcessId, TimerId = t.Id}).ToList();
        //    }
        //}
    }
}

