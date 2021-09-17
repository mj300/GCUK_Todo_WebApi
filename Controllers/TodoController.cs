using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Database;
using todo_webApi.Database.Models;
using todo_webApi.Repositories;
using todo_webApi.Repositories.Interfaces;
using todo_webApi.Utils;
using todo_webApi.Utils.Models;
using todo_webApi.Utils.Services;

namespace todo_webApi.Controllers
{
   [ApiController]
   [Route("todo")]
   public class TodoController : ControllerBase
   {
      private readonly IUnitOfWork UnitOfWork;

      private LoggingService LoggingService { get; }
      private List<Error> ErrorsList = new List<Error>();

      public TodoController(AppDbContext dbContext)
      {
         UnitOfWork = new UnitOfWork(dbContext);
         LoggingService = new LoggingService(UnitOfWork);
      }

      #region *** ***
      [ProducesResponseType(typeof(GetAllResponseBody<TodoItem>), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status417ExpectationFailed)]
      #endregion
      [HttpGet("{selectedPage}/{maxNumberPerItemsPage}/{searchValue}/{isCompelete}/{isSortAsce}/{sortName}")]
      [Authorize]
      public async Task<IActionResult> Search(
           int selectedPage,
           int maxNumberPerItemsPage,
           string searchValue = AppConst.GetAllRecords,
           string isCompelete = AppConst.GetAllRecords,
           bool isSortAsce = true,
           string sortName = "Name")
      {
         try
         {


            var result = await UnitOfWork.Todos.Search(
               (t => (isCompelete.Equals(AppConst.GetAllRecords) || t.IsCompelete.Equals(Convert.ToBoolean(isCompelete))) &&
                     (searchValue.Equals(AppConst.GetAllRecords) || t.Name.Contains(searchValue))),
               AppFunc.GetUserId(HttpContext),
               isSortAsce,
               sortName,
               selectedPage,
               maxNumberPerItemsPage);
            return Ok(result);
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }


      #region *** ***
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(typeof(TodoItem), StatusCodes.Status201Created)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status412PreconditionFailed)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status417ExpectationFailed)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status422UnprocessableEntity)]
      #endregion
      [HttpPost]
      [Authorize]
      /// Ready For Test
      public async Task<IActionResult> Post([FromBody] TodoItem newTodoItem)
      {
         try
         {
            newTodoItem.User = await UnitOfWork.Users.GetById(AppFunc.GetUserId(HttpContext));
            if (!ModelState.IsValid)
            {
               AppFunc.ExtractErrors(ModelState, ref ErrorsList);
               return UnprocessableEntity(ErrorsList);
            }




            await UnitOfWork.Todos.AddAsync(newTodoItem);
            await UnitOfWork.CompleteAsync();

            return Created("Success", newTodoItem);
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }

      #region *** ***
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status422UnprocessableEntity)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status417ExpectationFailed)]
      #endregion
      [HttpPut("{id}")]
      [Authorize]  /// Ready For Test         
      public async Task<IActionResult> Put(string id, [FromBody] TodoItem modifiedTodoItem)
      {
         try
         {
            if (!ModelState.IsValid)
            {
               AppFunc.ExtractErrors(ModelState, ref ErrorsList);
               return UnprocessableEntity(ErrorsList);
            }

            TodoItem originalTodoItem = await UnitOfWork.Todos
                                       .GetTodoItem(id, AppFunc.GetUserId(HttpContext));
            if (originalTodoItem is null)
            {
               AppFunc.Error(ref ErrorsList, "Todo Item not found.");
               return NotFound(ErrorsList);
            }
            originalTodoItem.Name = modifiedTodoItem.Name;
            originalTodoItem.IsCompelete = modifiedTodoItem.IsCompelete;

            await UnitOfWork.CompleteAsync();

            return Ok(originalTodoItem);
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }
      #region *** ***
      [Consumes(MediaTypeNames.Application.Json)]
      [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status412PreconditionFailed)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status422UnprocessableEntity)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status417ExpectationFailed)]
      #endregion
      [HttpPut("toggle/{id}")]
      [Authorize]  /// Ready For Test         
      public async Task<IActionResult> Toggle(string id)
      {
         try
         {
            if (!ModelState.IsValid)
            {
               AppFunc.ExtractErrors(ModelState, ref ErrorsList);
               return UnprocessableEntity(ErrorsList);
            }

            TodoItem toggledTodo = await UnitOfWork.Todos
                                       .Toggle(id, AppFunc.GetUserId(HttpContext));

            if (toggledTodo is null)
            {
               AppFunc.Error(ref ErrorsList, "Todo Item not found.");
               return NotFound(ErrorsList);
            }

            return Ok(toggledTodo);
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }


      #region *** ***
      [ProducesResponseType(typeof(TodoItem), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status404NotFound)]
      [ProducesResponseType(typeof(List<Error>), StatusCodes.Status417ExpectationFailed)]
      #endregion
      [HttpDelete("{id}")]
      [Authorize]  /// Ready For Test
      public async Task<IActionResult> Delete(string id)
      {
         try
         {
            TodoItem todoItem = await UnitOfWork.Todos
                     .GetTodoItem(id, AppFunc.GetUserId(HttpContext));
            if (todoItem is null)
            {
               AppFunc.Error(ref ErrorsList, "TodoItem not found");
               return NotFound(ErrorsList);
            }

            UnitOfWork.Todos.Remove(todoItem);
            await UnitOfWork.CompleteAsync();

            return Ok($"'{todoItem.Name}' was deleted");
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }
   }

}
