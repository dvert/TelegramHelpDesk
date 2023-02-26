using System.Data.Entity;

namespace TelegramHelpDesk.Models
{
    public class HelpdeskContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Lifecycle> Lifecycles { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<AppTask> AppTasks { get; set; }
        public DbSet<TelegramTask> TelegramTasks { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}