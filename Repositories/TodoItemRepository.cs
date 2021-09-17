using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using todo_webApi.Database;
using todo_webApi.Database.Models;
using todo_webApi.Repositories.Interfaces;
using todo_webApi.Utils.Extensions;
using todo_webApi.Utils.Models;

namespace todo_webApi.Repositories
{
   public class TodoItemRepository : Repository<TodoItem>, ITodoItemRepository
   {
      public TodoItemRepository(AppDbContext context)
        : base(context)
      {
      }
      public async Task<List<TodoItem>> GetAllTodoItemUserId(int userId)
      {
         return await _DbContext.Todos.Include(a => a.User)
                  .Include(a => a.User)
                  .Where(a => a.User.Id == userId).ToListAsync();
      }

      public async Task<TodoItem> GetTodoItem(string id, int userId)
      {
         return await _DbContext.Todos.AsTracking().Include(t => t.User).SingleOrDefaultAsync(t => t.Id == id && t.User.Id == userId);
      }

      public async Task<GetAllResponseBody<TodoItem>> Search(Expression<Func<TodoItem, bool>> predicate, int userId, bool isSortAsce, string sortName, int selectedPage, int maxNumberPerItemsPage)
      {

         int totalCount = await _DbContext.Todos.Include(t => t.User)
                 .Where(c => c.User.Id == userId).CountAsync(predicate);
         var list = await _DbContext.Todos.Include(t => t.User)
                 .Where(c => c.User.Id == userId).Where(predicate)
                 .OrderByDynamic(sortName, isSortAsce)
                 .Skip((selectedPage - 1) * maxNumberPerItemsPage)
                 .Take(maxNumberPerItemsPage)
                 .ToListAsync();

         return new GetAllResponseBody<TodoItem>(list, totalCount);
      }

      public async Task<TodoItem> Toggle(string id, int userId)
      {
         var todoItem = await _DbContext.Todos.AsTracking().Include(a => a.User).SingleOrDefaultAsync(t => t.Id == id && t.User.Id == userId);
         if (todoItem != null)
         {
            todoItem.IsCompelete = !todoItem.IsCompelete;

            _DbContext.SaveChanges();
         }
         return todoItem;
      }
   }
}
