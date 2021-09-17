using System;
using System.Threading.Tasks;
using todo_webApi.Database;
using todo_webApi.Database.Models;
using todo_webApi.Repositories.Interfaces;

namespace todo_webApi.Repositories
{
   public class UnitOfWork : IUnitOfWork
   {
      private readonly AppDbContext _context;

      public UnitOfWork(AppDbContext context)
      {
         _context = context;
         Todos = new TodoItemRepository(_context);
         Users = new UserRepository(_context);
         AppLogs = new Repository<AppLog>(_context);
      }

      public ITodoItemRepository Todos { get; private set; }
      public IUserRepository Users { get; private set; }

      public IRepository<AppLog> AppLogs { get; private set; }


      public Task<int> CompleteAsync()
      {
         return _context.SaveChangesAsync();
      }

      public void Dispose()
      {
         _context.Dispose();
         GC.SuppressFinalize(this);
      }
   }
}
