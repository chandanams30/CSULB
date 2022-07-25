using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.PersistanceModel;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;
using ThoughtFocus.Common.WorkFlowDataAccess.Helper;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess
{
    public class WorkFlowContext : DbContext
    {
        static WorkFlowContext()
        {
            // Database.SetInitializer<WorkFlowContext>(null);

        }
        public WorkFlowContext(string connectionString) : base(GetOptions(connectionString))
        {

        }
        public WorkFlowContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            DbContextOptionsBuilder optionsBuilder = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString);
            optionsBuilder.UseLazyLoadingProxies();
            return optionsBuilder.Options;
        }


        public DbSet<ActionDefinition> ActionDefinition { get; set; }
        public DbSet<TransitionDefinition> TransitionDefinition { get; set; }
        public DbSet<WorkflowDefinition> WorkflowDefinition { get; set; }
        public DbSet<CommandDefinition> CommandDefinition { get; set; }
        public DbSet<ActivityDefinition> ActivityDefinition { get; set; }
        public DbSet<ParameterDefinition> ParameterDefinition { get; set; }
        public DbSet<ConditionDefinition> ConditionDefinition { get; set; }
        public DbSet<LocalizeDefinition> LocalizeDefinition { get; set; }
        public DbSet<TriggerDefinition> TriggerDefinition { get; set; }
        public DbSet<RestrictionDefinition> RestrictionDefinition { get; set; }
        public DbSet<WorkflowProcessInstance> WorkflowProcessInstances { get; set; }
        public DbSet<WorkflowProcessInstancePersistence> WorkflowProcessInstancePersistences { get; set; }
        public DbSet<WorkflowProcessInstanceStatus> WorkflowProcessInstanceStatus { get; set; }
        public DbSet<WorkflowProcessTimer> WorkflowProcessTimers { get; set; }
        public DbSet<WorkflowProcessTransitionHistory> WorkflowProcessTransitionHistories { get; set; }
        public DbSet<ActorDefinitionExecuteRule> ActorDefinitionExecuteRules { get; set; }
        public DbSet<ActorDefinitionIsIdentity> ActorDefinitionIsIdentities { get; set; }
        public DbSet<ActorDefinitionIsInRole> ActorDefinitionIsInRoles { get; set; }
        public DbSet<ParameterDefinitionForAction> ParameterDefinitionForAction { get; set; }
        public DbSet<ActionDefinitionForActivity> ActionDefinitionForActivity { get; set; }

        //Enumeration
        public DbSet<TransitionClassifier> TransitionClassifier { get; set; }
        public DbSet<ParameterPurpose> ParameterPurpose { get; set; }
        public DbSet<ConditionType> ConditionType { get; set; }
        public DbSet<LocalizeType> LocalizeType { get; set; }
        public DbSet<TriggerType> TriggerType { get; set; }
        public DbSet<RestrictionType> RestrictionType { get; set; }
        public DbSet<ValidationType> ValidationType { get; set; }

        public DbSet<ValidationDefination> ValidationDefination { get; set; }
        public DbSet<ValidationFieldDefination> ValidationFieldDefination { get; set; }
        public DbSet<TransitionValidationDefination> TransitionValidationDefination { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActorDefinitionExecuteRule>().ToTable("ActorDefinitionExecuteRule", "WorkFlow");
            modelBuilder.Entity<ActorDefinitionIsIdentity>().ToTable("ActorDefinitionIsIdentity", "WorkFlow");
            modelBuilder.Entity<ActorDefinitionIsInRole>().ToTable("ActorDefinitionIsInRole", "WorkFlow");

            modelBuilder.Entity<ActionDefinitionForActivity>().HasOne(u => u.ActionDefinition);
            modelBuilder.Entity<ActionDefinitionForActivity>().HasOne(u => u.ActivityDefinition);
            modelBuilder.Entity<ParameterDefinition>().HasOne(u => u.WorkflowDefinition);
            modelBuilder.Entity<TransitionDefinition>().HasMany(u => u.RestrictionsList);
            modelBuilder.Entity<TransitionDefinition>().Ignore(p => p.RestrictionsList);
            modelBuilder.Entity<TriggerDefinition>().Ignore(p => p.CommandList);
            modelBuilder.Entity<TriggerDefinition>().HasOne(a => a.Command);
            modelBuilder.Entity<ActionDefinition>().HasMany(p => p.ParameterDefinitionForActions);
            modelBuilder.Entity<ActionDefinition>().HasMany(p => p.ParameterDefinitionForActionsLazy);
            modelBuilder.Entity<ActionDefinition>().Ignore(p => p.ParameterDefinitionForActionsLazy);
            modelBuilder.Entity<ParameterDefinitionForAction>().Ignore(p => p.ActionDefinition);
            modelBuilder.Entity<ActionDefinition>().HasMany(p => p.InputParameters);
            modelBuilder.Entity<ActionDefinition>().Navigation(b => b.InputParameters).HasField("InputParametersLazyLoading").UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
            modelBuilder.Entity<ActionDefinition>().HasMany(p => p.OutputParameters);
            modelBuilder.Entity<ActionDefinition>().Navigation(b => b.OutputParameters).HasField("InputParametersLazyLoading").UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

            modelBuilder.Entity<ActionDefinition>().Ignore(p => p.InputParameters);
            modelBuilder.Entity<ActionDefinition>().HasMany(p => p.OutputParameters);
            modelBuilder.Entity<ActionDefinition>().Ignore(p => p.OutputParameters);
            modelBuilder.Entity<RestrictionDefinition>().HasOne(u => u.Transition);
            modelBuilder.Entity<TransitionDefinition>().Navigation(b => b.Restrictions).HasField("RestrictionLazyLoading").UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
            modelBuilder.Entity<TriggerDefinition>().Navigation(b => b.Command).HasField("CommandsLazyLoading").UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

        }

    }
}

