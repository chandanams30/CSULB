using log4net;
using System;
using ThoughtFocus.Common.Workflow.Core.Bus;
using ThoughtFocus.Common.Workflow.Core.Persistence;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
  
    public static class WorkflowRuntimeConfigurationExtension
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkflowRuntimeConfigurationExtension));

        public static WorkflowRuntime WithActionProvider(this WorkflowRuntime runtime, IWorkflowActionProvider workflowActionProvider)
        {
            if (workflowActionProvider==null)
            {
                Logger.Debug(String.Format("workflowActionProvider is null while initailizing the runtime"));
            }
            runtime.ActionProvider = workflowActionProvider;
            return runtime;
        }

        //public static WorkflowRuntime WithBuilder(this WorkflowRuntime runtime, IWorkflowBuilder builder)
        //{
        //    runtime.Builder = builder;
        //    return runtime;
        //}

        //public static WorkflowRuntime WithDefaultBuilder<TWorkflowMedium>(this WorkflowRuntime runtime) where TWorkflowMedium : class
        //{
        //    runtime.Builder = new WorkflowBuilder<TWorkflowMedium>();
        //    return runtime;
        //}

        public static WorkflowRuntime WithRoleProvider(this WorkflowRuntime runtime, IWorkflowRoleProvider roleProvider)
        {
            if (roleProvider == null)
            {
               Logger.Debug(String.Format("roleProvider is null while initailizing the runtime"));
            }
            runtime.RoleProvider = roleProvider;
            return runtime;
        }

        public static WorkflowRuntime WithRuleProvider(this WorkflowRuntime runtime, IWorkflowRuleProvider ruleProvider)
        {
            if (ruleProvider == null)
            {
                Logger.Debug(String.Format("ruleProvider is null while initailizing the runtime"));
            }
            runtime.RuleProvider = ruleProvider;
            return runtime;
        }

        public static WorkflowRuntime WithPersistenceProvider(this WorkflowRuntime runtime, IPersistenceProvider persistenceProvider)
        {
            if (persistenceProvider == null)
            {
                Logger.Debug(String.Format("persistenceProvider is null while initailizing the runtime"));
            }
            runtime.PersistenceProvider = persistenceProvider;
            return runtime;
        }

        public static WorkflowRuntime WithRuntimePersistance(this WorkflowRuntime runtime, IRuntimePersistence persistenceProvider)
        {
            runtime.RuntimePersistence = persistenceProvider;
            return runtime;
        }

        public static WorkflowRuntime WithBus(this WorkflowRuntime runtime, IWorkflowBus bus)
        {
            if (bus == null)
            {
                Logger.Debug(String.Format("bus is null while initailizing the runtime"));
            }
            bus.Initialize();
            runtime.Bus = bus;
            bus.ExecutionComplete += runtime.BusExecutionComplete;
            return runtime;
        }
        public static WorkflowRuntime WithBuilder(this WorkflowRuntime runtime, IWorkflowProvider builder)
        {
            if (builder == null)
            {
                Logger.Debug(String.Format("builder is null while initailizing the runtime"));
            }
            runtime.Builder = builder;
            return runtime;
        }
       
        public static WorkflowRuntime AttachDeterminingParametersGetter(this WorkflowRuntime runtime, EventHandler<NeedDeterminingParametersEventArgs> determiningParametersGetter)
        {
            runtime.OnNeedDeterminingParameters += determiningParametersGetter;
            return runtime;
        }

        public static WorkflowRuntime SwitchAutoUpdateWorkflowBeforeGetAvailableCommandsOn(this WorkflowRuntime runtime)
        {
            runtime.IsAutoUpdateSchemeBeforeGetAvailableCommands = true;
            return runtime;
        }

        public static WorkflowRuntime SwitchAutoUpdateWorkflowBeforeGetAvailableCommandsOn(this WorkflowRuntime runtime, EventHandler<NeedDeterminingParametersEventArgs> determiningParametersGetter)
        {
            runtime.IsAutoUpdateSchemeBeforeGetAvailableCommands = true;
            runtime.OnNeedDeterminingParameters += determiningParametersGetter;
            return runtime;
        }

        public static WorkflowRuntime SwitchAutoUpdateWorkflowBeforeGetAvailableCommandsOff(this WorkflowRuntime runtime)
        {
            runtime.IsAutoUpdateSchemeBeforeGetAvailableCommands = false;
            return runtime;
        }

        public static WorkflowRuntime Start(this WorkflowRuntime runtime)
        {
            if (!runtime.ValidateSettings())
                throw new InvalidOperationException();
            runtime.Start();
            return runtime;
        }

        public static WorkflowRuntime ColdStart(this WorkflowRuntime runtime)
        {
            if (!runtime.ValidateSettings())
                throw new InvalidOperationException();
            runtime.ColdStart();
            return runtime;
        }

        //public static IWorkflowBuilder WithCache(this IWorkflowBuilder bulder, IParsedProcessCache cache)
        //{
        //    bulder.SetCache(cache);
        //    return bulder;
        //}

        //public static IWorkflowBuilder WithDefaultCache(this IWorkflowBuilder bulder)
        //{
        //    bulder.SetCache(new DefaultParcedProcessCache());
        //    return bulder;
        //}

        //public static IWorkflowBuilder WithGenerator<TWorkflowMedium>(this WorkflowBuilder<TWorkflowMedium> bulder, IWorkflowGenerator<TWorkflowMedium> generator) where TWorkflowMedium : class
        //{
        //    bulder.Generator = generator;
        //    return bulder;
        //}

        //public static IWorkflowBuilder WithParser<TWorkflowMedium>(this WorkflowBuilder<TWorkflowMedium> bulder, IWorkflowParser<TWorkflowMedium> parser) where TWorkflowMedium : class
        //{
        //    bulder.Parser = parser;
        //    return bulder;
        //}

        //public static IWorkflowBuilder WithShemePersistenceProvider<TWorkflowMedium>(this WorkflowBuilder<TWorkflowMedium> bulder, IWorkflowPersistenceProvider<TWorkflowMedium> schemePersistenceProvider) where TWorkflowMedium : class
        //{
        //    bulder.WorkflowPersistenceProvider = schemePersistenceProvider;
        //    return bulder;
        //}

        //public static IWorkflowGenerator<TWorkflowMedium> WithMapping<TWorkflowMedium> (this IWorkflowGenerator<TWorkflowMedium> generator, string workflowName, object generatorSource) where TWorkflowMedium : class
        //{
        //    generator.AddMapping(workflowName,generatorSource);
        //    return generator;
        //}
    }
}
