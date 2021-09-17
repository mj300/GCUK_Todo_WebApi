using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using todo_webApi.Database.Models;


namespace todo_webApi.Database
{

   public class AppDbContext : DbContext
   {

      public DbSet<TodoItem> Todos { get; set; }
      public DbSet<User> Users { get; set; }
      public DbSet<AppLog> AppLogs { get; set; }




      protected override void OnModelCreating(ModelBuilder builder)
      {
         base.OnModelCreating(builder);



         builder.Entity<AppLog>().HasOne(l => l.User).WithMany().OnDelete(DeleteBehavior.Cascade);
      }
      internal void DetachAllEntities()
      {
         var changedEntriesCopy = this.ChangeTracker.Entries()
             .Where(e => e.State == EntityState.Added ||
                         e.State == EntityState.Modified ||
                         e.State == EntityState.Deleted)
             .ToList();

         foreach (var entry in changedEntriesCopy)
            entry.State = EntityState.Detached;
      }
      public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
      {
      }
   }


}
