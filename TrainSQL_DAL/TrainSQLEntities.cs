namespace TrainSQL_DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TrainSQLEntities : DbContext
    {
        public TrainSQLEntities()
            : base("name=TrainSQLConnection")
        {
        }

        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<Progress> Progresses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TestDatabas> TestDatabases { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Task>()
                .HasMany(e => e.Complaints)
                .WithRequired(e => e.Task)
                .HasForeignKey(e => new { e.TaskID, e.ThemeID })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TestDatabas>()
                .Property(e => e.dbName)
                .IsUnicode(false);

            modelBuilder.Entity<TestDatabas>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.TestDatabas)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Theme>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Theme)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Progresses)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
