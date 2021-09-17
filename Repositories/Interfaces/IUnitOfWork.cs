using System;
using System.Threading.Tasks;
using todo_webApi.Database.Models;

namespace todo_webApi.Repositories.Interfaces
{
   public interface IUnitOfWork : IDisposable
   {
      ITodoItemRepository Todos { get; }
      IUserRepository Users { get; }
      IRepository<AppLog> AppLogs { get; }
      Task<int> CompleteAsync();
   }
}
