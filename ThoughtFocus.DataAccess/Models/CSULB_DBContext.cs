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

        public virtual DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
        public virtual DbSet<AuthenticationType> AuthenticationTypes { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormUser> FormUsers { get; set; }
        public virtual DbSet<FormsHistory> FormsHistories { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramUser> ProgramUsers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<StudentDocument> StudentDocuments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<UserCred> UserCreds { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

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

                entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");

                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");

                entity.Property(e => e.Form1)
                    .IsRequired()
                    .HasColumnName("Form");

                entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Forms)
                    .HasForeignKey(d => d.ApplicationId)
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

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log", "Application");

                entity.Property(e => e.Level).HasMaxLength(128);

                entity.Property(e => e.Properties).HasColumnType("xml");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
