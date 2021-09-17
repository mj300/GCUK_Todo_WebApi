using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using todo_webApi.Database.Models;
using todo_webApi.Utils.Models;

namespace todo_webApi.Repositories.Interfaces
{
   public interface ITodoItemRepository : IRepository<TodoItem>
   {
      Task<List<TodoItem>> GetAllTodoItemUserId(int userId);
      Task<TodoItem> GetTodoItem(string id, int userId);

      Task<GetAllResponseBody<TodoItem>> Search(Expression<Func<TodoItem, bool>> predicate, int userId, bool isSortAsce, string sortName, int selectedPage, int maxNumberPerItemsPage);

      Task<TodoItem> Toggle(string id, int userId);

   }
}
