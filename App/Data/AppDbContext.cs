using App.Base.Configuration;
using App.Base.Entities;
using App.CloudTask.Configuration;
using App.CloudTask.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskType> TaskTypes { get; set; }
    public DbSet<AppClient> AppClients { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .AddBase()
            .AddCloudTask();
        
        
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CloudTask.Entity.CloudTask>()
            .HasOne(e => e.RecBy)
            .WithMany()
            .HasForeignKey(e => e.RecById)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CloudTask.Entity.CloudTask>()
            .HasOne(e => e.ProccedBy)
            .WithMany()
            .HasForeignKey(e => e.ProccedById)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CloudTask.Entity.CloudTask>()
            .HasOne(e => e.CompletedBy)
            .WithMany()
            .HasForeignKey(e => e.CompletedById)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CloudTask.Entity.CloudTask>()
            .HasOne(e => e.TaskType)
            .WithMany()
            .HasForeignKey(e => e.TaskTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CloudTaskLog>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}