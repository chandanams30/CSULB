using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class CSULB_DBContext : DbContext
    {
        public CSULB_DBContext()
        {
        }

        public CSULB_DBContext(DbContextOptions<CSULB_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionDefinition> ActionDefinitions { get; set; }
        public virtual DbSet<ActionDefinitionForActivity> ActionDefinitionForActivities { get; set; }
        public virtual DbSet<ActivityDefinition> ActivityDefinitions { get; set; }
        public virtual DbSet<ActorDefinitionExecuteRule> ActorDefinitionExecuteRules { get; set; }
        public virtual DbSet<ActorDefinitionIsIdentity> ActorDefinitionIsIdentities { get; set; }
        public virtual DbSet<ActorDefinitionIsInRole> ActorDefinitionIsInRoles { get; set; }
        public virtual DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
        public virtual DbSet<AuthenticationType> AuthenticationTypes { get; set; }
        public virtual DbSet<CommandDefinition> CommandDefinitions { get; set; }
        public virtual DbSet<ConditionDefinition> ConditionDefinitions { get; set; }
        public virtual DbSet<ConditionType> ConditionTypes { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormUser> FormUsers { get; set; }
        public virtual DbSet<FormsHistory> FormsHistories { get; set; }
        public virtual DbSet<LocalizeDefinition> LocalizeDefinitions { get; set; }
        public virtual DbSet<LocalizeType> LocalizeTypes { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<ParameterDefinition> ParameterDefinitions { get; set; }
        public virtual DbSet<ParameterDefinitionForAction> ParameterDefinitionForActions { get; set; }
        public virtual DbSet<ParameterPurpose> ParameterPurposes { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramUser> ProgramUsers { get; set; }
        public virtual DbSet<RestrictionDefinition> RestrictionDefinitions { get; set; }
        public virtual DbSet<RestrictionType> RestrictionTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<StudentDocument> StudentDocuments { get; set; }
        public virtual DbSet<TransitionClassifier> TransitionClassifiers { get; set; }
        public virtual DbSet<TransitionDefinition> TransitionDefinitions { get; set; }
        public virtual DbSet<TransitionValidationDefination> TransitionValidationDefinations { get; set; }
        public virtual DbSet<TriggerDefinition> TriggerDefinitions { get; set; }
        public virtual DbSet<TriggerType> TriggerTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<UserCred> UserCreds { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<ValidationDefination> ValidationDefinations { get; set; }
        public virtual DbSet<ValidationFieldDefination> ValidationFieldDefinations { get; set; }
        public virtual DbSet<ValidationType> ValidationTypes { get; set; }
        public virtual DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
        public virtual DbSet<WorkflowProcessInstance> WorkflowProcessInstances { get; set; }
        public virtual DbSet<WorkflowProcessInstancePersistence> WorkflowProcessInstancePersistences { get; set; }
        public virtual DbSet<WorkflowProcessInstanceStatus> WorkflowProcessInstanceStatuses { get; set; }
        public virtual DbSet<WorkflowProcessTimer> WorkflowProcessTimers { get; set; }
        public virtual DbSet<WorkflowProcessTransitionHistory> WorkflowProcessTransitionHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=CSULB_DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionDefinition>(entity =>
            {
                entity.ToTable("ActionDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ActionDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ActionDefinitionForActivity>(entity =>
            {
                entity.ToTable("ActionDefinitionForActivity", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActionDefinitionId).HasColumnName("ActionDefinitionID");

                entity.Property(e => e.ActivityDefinitionId).HasColumnName("ActivityDefinitionID");

                entity.HasOne(d => d.ActionDefinition)
                    .WithMany(p => p.ActionDefinitionForActivities)
                    .HasForeignKey(d => d.ActionDefinitionId);

                entity.HasOne(d => d.ActivityDefinition)
                    .WithMany(p => p.ActionDefinitionForActivities)
                    .HasForeignKey(d => d.ActivityDefinitionId);
            });

            modelBuilder.Entity<ActivityDefinition>(entity =>
            {
                entity.ToTable("ActivityDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ActivityDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ActorDefinitionExecuteRule>(entity =>
            {
                entity.ToTable("ActorDefinitionExecuteRule", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ActorDefinitionExecuteRules)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ActorDefinitionIsIdentity>(entity =>
            {
                entity.ToTable("ActorDefinitionIsIdentity", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ActorDefinitionIsIdentities)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ActorDefinitionIsInRole>(entity =>
            {
                entity.ToTable("ActorDefinitionIsInRole", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ActorDefinitionIsInRoles)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ApplicationDocument>(entity =>
            {
                entity.ToTable("ApplicationDocuments", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.Property(e => e.ProgramId).HasColumnName("ProgramID");
            });

            modelBuilder.Entity<ApplicationType>(entity =>
            {
                entity.ToTable("ApplicationTypes", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<AuthenticationType>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AuthenticationType", "Master");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CommandDefinition>(entity =>
            {
                entity.ToTable("CommandDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.CommandDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ConditionDefinition>(entity =>
            {
                entity.ToTable("ConditionDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActionId).HasColumnName("Action_ID");

                entity.Property(e => e.ConditionTypeId).HasColumnName("ConditionTypeID");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.ConditionDefinitions)
                    .HasForeignKey(d => d.ActionId);

                entity.HasOne(d => d.ConditionType)
                    .WithMany(p => p.ConditionDefinitions)
                    .HasForeignKey(d => d.ConditionTypeId);
            });

            modelBuilder.Entity<ConditionType>(entity =>
            {
                entity.ToTable("ConditionType", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documents", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Form>(entity =>
            {
                entity.ToTable("Forms", "Application");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.Form1)
                    .IsRequired()
                    .HasColumnName("Form");

                entity.Property(e => e.ProgramId).HasColumnName("ProgramID");

                entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Program)
                    .WithMany(p => p.Forms)
                    .HasForeignKey(d => d.ProgramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Forms_Application");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Forms)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Forms_Semester");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Forms)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Forms_Users");
            });

            modelBuilder.Entity<FormUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FormUsers", "Application");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<FormsHistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FormsHistory", "Application");

                entity.Property(e => e.Form).IsRequired();

                entity.Property(e => e.FormsId).HasColumnName("FormsID");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.VersionNumber)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LocalizeDefinition>(entity =>
            {
                entity.ToTable("LocalizeDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LocalizeTypeId).HasColumnName("LocalizeTypeID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.LocalizeType)
                    .WithMany(p => p.LocalizeDefinitions)
                    .HasForeignKey(d => d.LocalizeTypeId);

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.LocalizeDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<LocalizeType>(entity =>
            {
                entity.ToTable("LocalizeType", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log", "Application");

                entity.Property(e => e.Level).HasMaxLength(128);

                entity.Property(e => e.Properties).HasColumnType("xml");
            });

            modelBuilder.Entity<ParameterDefinition>(entity =>
            {
                entity.ToTable("ParameterDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PurposeId).HasColumnName("PurposeID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.Purpose)
                    .WithMany(p => p.ParameterDefinitions)
                    .HasForeignKey(d => d.PurposeId);

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.ParameterDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<ParameterDefinitionForAction>(entity =>
            {
                entity.ToTable("ParameterDefinitionForAction", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActionDefinitionId).HasColumnName("ActionDefinitionID");

                entity.Property(e => e.ParameterDefinitionId).HasColumnName("ParameterDefinitionID");

                entity.HasOne(d => d.ActionDefinition)
                    .WithMany(p => p.ParameterDefinitionForActions)
                    .HasForeignKey(d => d.ActionDefinitionId);

                entity.HasOne(d => d.ParameterDefinition)
                    .WithMany(p => p.ParameterDefinitionForActions)
                    .HasForeignKey(d => d.ParameterDefinitionId);
            });

            modelBuilder.Entity<ParameterPurpose>(entity =>
            {
                entity.ToTable("ParameterPurpose", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.ToTable("Programs", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ApplicationTypesId).HasColumnName("ApplicationTypesID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.ApplicationTypes)
                    .WithMany(p => p.Programs)
                    .HasForeignKey(d => d.ApplicationTypesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Application_ApplicationTypes");
            });

            modelBuilder.Entity<ProgramUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProgramUsers", "Application");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<RestrictionDefinition>(entity =>
            {
                entity.ToTable("RestrictionDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActorDefinitionExecuteRuleId).HasColumnName("ActorDefinitionExecuteRule_ID");

                entity.Property(e => e.ActorDefinitionIsIdentityId).HasColumnName("ActorDefinitionIsIdentity_ID");

                entity.Property(e => e.ActorDefinitionIsInRoleId).HasColumnName("ActorDefinitionIsInRole_ID");

                entity.Property(e => e.RestrictionTypeId).HasColumnName("RestrictionType_Id");

                entity.Property(e => e.TransitionId).HasColumnName("Transition_ID");

                entity.HasOne(d => d.ActorDefinitionExecuteRule)
                    .WithMany(p => p.RestrictionDefinitions)
                    .HasForeignKey(d => d.ActorDefinitionExecuteRuleId);

                entity.HasOne(d => d.ActorDefinitionIsIdentity)
                    .WithMany(p => p.RestrictionDefinitions)
                    .HasForeignKey(d => d.ActorDefinitionIsIdentityId);

                entity.HasOne(d => d.ActorDefinitionIsInRole)
                    .WithMany(p => p.RestrictionDefinitions)
                    .HasForeignKey(d => d.ActorDefinitionIsInRoleId);

                entity.HasOne(d => d.RestrictionType)
                    .WithMany(p => p.RestrictionDefinitions)
                    .HasForeignKey(d => d.RestrictionTypeId);

                entity.HasOne(d => d.Transition)
                    .WithMany(p => p.RestrictionDefinitions)
                    .HasForeignKey(d => d.TransitionId);
            });

            modelBuilder.Entity<RestrictionType>(entity =>
            {
                entity.ToTable("RestrictionType", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastModifiedByUserId).HasColumnName("LastModifiedByUserID");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("Semester", "Master");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.CreatedDateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<StudentDocument>(entity =>
            {
                entity.ToTable("StudentDocuments", "Application");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.Property(e => e.FileExtn)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FolderName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.StudentDocuments)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentDocuments_Documents");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StudentDocuments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentDocuments_Users");
            });

            modelBuilder.Entity<TransitionClassifier>(entity =>
            {
                entity.ToTable("TransitionClassifier", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TransitionDefinition>(entity =>
            {
                entity.ToTable("TransitionDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ConditionId).HasColumnName("ConditionID");

                entity.Property(e => e.FromId).HasColumnName("FromID");

                entity.Property(e => e.ToId).HasColumnName("ToID");

                entity.Property(e => e.TransitionClassifierId).HasColumnName("TransitionClassifierID");

                entity.Property(e => e.TriggerId).HasColumnName("TriggerID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.Condition)
                    .WithMany(p => p.TransitionDefinitions)
                    .HasForeignKey(d => d.ConditionId);

                entity.HasOne(d => d.From)
                    .WithMany(p => p.TransitionDefinitionFroms)
                    .HasForeignKey(d => d.FromId);

                entity.HasOne(d => d.To)
                    .WithMany(p => p.TransitionDefinitionTos)
                    .HasForeignKey(d => d.ToId);

                entity.HasOne(d => d.TransitionClassifier)
                    .WithMany(p => p.TransitionDefinitions)
                    .HasForeignKey(d => d.TransitionClassifierId);

                entity.HasOne(d => d.Trigger)
                    .WithMany(p => p.TransitionDefinitions)
                    .HasForeignKey(d => d.TriggerId);

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.TransitionDefinitions)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<TransitionValidationDefination>(entity =>
            {
                entity.ToTable("TransitionValidationDefination", "WorkFlow");

                entity.Property(e => e.TransitionValidationDefinationId).HasColumnName("TransitionValidationDefinationID");

                entity.Property(e => e.TransitionDefinitionId).HasColumnName("TransitionDefinitionID");

                entity.Property(e => e.ValidationDefinationId).HasColumnName("ValidationDefinationID");

                entity.HasOne(d => d.TransitionDefinition)
                    .WithMany(p => p.TransitionValidationDefinations)
                    .HasForeignKey(d => d.TransitionDefinitionId);

                entity.HasOne(d => d.ValidationDefination)
                    .WithMany(p => p.TransitionValidationDefinations)
                    .HasForeignKey(d => d.ValidationDefinationId);
            });

            modelBuilder.Entity<TriggerDefinition>(entity =>
            {
                entity.ToTable("TriggerDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CommandId).HasColumnName("CommandID");

                entity.HasOne(d => d.Command)
                    .WithMany(p => p.TriggerDefinitions)
                    .HasForeignKey(d => d.CommandId);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.TriggerDefinitions)
                    .HasForeignKey(d => d.TypeId);
            });

            modelBuilder.Entity<TriggerType>(entity =>
            {
                entity.ToTable("TriggerType", "WorkFlow");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserActivityLog", "User");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(50)
                    .HasColumnName("IPAddress");

                entity.Property(e => e.LoginDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserCred>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserCred", "User");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles", "User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");
            });

            modelBuilder.Entity<ValidationDefination>(entity =>
            {
                entity.ToTable("ValidationDefination", "WorkFlow");

                entity.Property(e => e.ValidationDefinationId).HasColumnName("ValidationDefinationID");

                entity.Property(e => e.ValidationTypeId).HasColumnName("ValidationTypeID");

                entity.HasOne(d => d.ValidationType)
                    .WithMany(p => p.ValidationDefinations)
                    .HasForeignKey(d => d.ValidationTypeId);
            });

            modelBuilder.Entity<ValidationFieldDefination>(entity =>
            {
                entity.ToTable("ValidationFieldDefination", "WorkFlow");

                entity.Property(e => e.ValidationFieldDefinationId).HasColumnName("ValidationFieldDefinationID");

                entity.Property(e => e.ValidationDefinationId).HasColumnName("ValidationDefinationID");

                entity.HasOne(d => d.ValidationDefination)
                    .WithMany(p => p.ValidationFieldDefinations)
                    .HasForeignKey(d => d.ValidationDefinationId);
            });

            modelBuilder.Entity<ValidationType>(entity =>
            {
                entity.ToTable("ValidationType", "WorkFlow");

                entity.Property(e => e.ValidationTypeId).HasColumnName("ValidationTypeID");
            });

            modelBuilder.Entity<WorkflowDefinition>(entity =>
            {
                entity.ToTable("WorkflowDefinition", "WorkFlow");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<WorkflowProcessInstance>(entity =>
            {
                entity.ToTable("WorkflowProcessInstance", "WorkFlow");

                entity.Property(e => e.WorkflowProcessInstanceId).HasColumnName("WorkflowProcessInstanceID");

                entity.Property(e => e.ProcessInstanceId).HasColumnName("ProcessInstanceID");

                entity.Property(e => e.WorkflowDefinitionId).HasColumnName("WorkflowDefinitionID");

                entity.HasOne(d => d.WorkflowDefinition)
                    .WithMany(p => p.WorkflowProcessInstances)
                    .HasForeignKey(d => d.WorkflowDefinitionId);
            });

            modelBuilder.Entity<WorkflowProcessInstancePersistence>(entity =>
            {
                entity.ToTable("WorkflowProcessInstancePersistence", "WorkFlow");

                entity.Property(e => e.WorkflowProcessInstancePersistenceId)
                    .ValueGeneratedNever()
                    .HasColumnName("WorkflowProcessInstancePersistenceID");

                entity.Property(e => e.ProcessInstanceId).HasColumnName("ProcessInstanceID");

                entity.Property(e => e.WorkFlowDefinationId).HasColumnName("WorkFlowDefinationID");
            });

            modelBuilder.Entity<WorkflowProcessInstanceStatus>(entity =>
            {
                entity.ToTable("WorkflowProcessInstanceStatus", "WorkFlow");

                entity.Property(e => e.WorkflowProcessInstanceStatusId).HasColumnName("WorkflowProcessInstanceStatusID");

                entity.Property(e => e.ProcessInstanceId).HasColumnName("ProcessInstanceID");

                entity.Property(e => e.WorkFlowDefinationId).HasColumnName("WorkFlowDefinationID");
            });

            modelBuilder.Entity<WorkflowProcessTimer>(entity =>
            {
                entity.ToTable("WorkflowProcessTimer", "WorkFlow");

                entity.Property(e => e.WorkflowProcessTimerId)
                    .ValueGeneratedNever()
                    .HasColumnName("WorkflowProcessTimerID");

                entity.Property(e => e.ProcessInstanceId).HasColumnName("ProcessInstanceID");

                entity.Property(e => e.WorkFlowDefinationId).HasColumnName("WorkFlowDefinationID");
            });

            modelBuilder.Entity<WorkflowProcessTransitionHistory>(entity =>
            {
                entity.ToTable("WorkflowProcessTransitionHistory", "WorkFlow");

                entity.Property(e => e.WorkflowProcessTransitionHistoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("WorkflowProcessTransitionHistoryID");

                entity.Property(e => e.ProcessInstanceId).HasColumnName("ProcessInstanceID");

                entity.Property(e => e.WorkFlowDefinationId).HasColumnName("WorkFlowDefinationID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
