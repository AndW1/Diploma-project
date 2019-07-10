using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserApp> UserApps { get; set; }
        public DbSet<BoardTask> BoardTasks { get; set; }
        public DbSet<StageTask> StageTasks { get; set; }
        public DbSet<NodeStage> NodeStages { get; set; }
        public DbSet<SomeData> SomeDatas { get; set; }
      

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
           : base(options)
        {
        }

    }
}
