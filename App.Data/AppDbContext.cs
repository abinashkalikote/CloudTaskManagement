using App.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<CloudTask> CloudTasks { get; set; }
        public DbSet<AuditTask> AuditTasks { get; set; }
    }
}
