using App.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<CloudTask> CloudTasks { get; set; }
        public DbSet<AuditTask> AuditTasks { get; set; }
        public DbSet<CloudTaskLog> CloudTasksLog { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CloudTask>()
                .HasOne(e => e.RecBy)
                .WithMany()
                .HasForeignKey(e => e.RecById)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CloudTask>()
                .HasOne(e => e.ProccedBy)
                .WithMany()
                .HasForeignKey(e => e.ProccedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CloudTask>()
                .HasOne(e => e.CompletedBy)
                .WithMany()
                .HasForeignKey(e => e.CompletedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CloudTask>()
                .HasOne(e => e.TaskType)
                .WithMany()
                .HasForeignKey(e => e.TaskTypeId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<AuditTask>()
                .HasOne(e => e.RecBy)
                .WithMany()
                .HasForeignKey(e => e.RecById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AuditTask>()
                .HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CloudTaskLog>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
