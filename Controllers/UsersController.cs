using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
   [Route("user")]
   public class UsersController : ControllerBase
   {
      private readonly IUnitOfWork UnitOfWork;
      private LoggingService LoggingService { get; }
      private List<Error> ErrorsList = new List<Error>();

      public UsersController(AppDbContext dbContext)
      {
         UnitOfWork = new UnitOfWork(dbContext);
         LoggingService = new LoggingService(UnitOfWork);
      }



      [HttpPost("register")]
      public async Task<IActionResult> Register([FromBody] UserRegistrationInfo model)
      {
         try
         {
            if (await UnitOfWork.Users.CheckUserName(model.Username))
            {
               AppFunc.Error(ref ErrorsList, "The username is exist.");
               return StatusCode(417, ErrorsList);
            }

            var response = await UnitOfWork.Users.AddUser(model);

            return Ok(response);

         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }

      }

      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
      {
         try
         {
            var response = await UnitOfWork.Users.Authenticate(model);

            if (response == null)
            {
               AppFunc.Error(ref ErrorsList, "Username or Password is incorect");
               return StatusCode(417, ErrorsList);
            }


            return Ok(response);
         }
         catch (Exception ex)
         {
            AppFunc.Error(ref ErrorsList, await LoggingService.LogException(Request.Path, ex, AppFunc.GetUser(HttpContext)));
            return StatusCode(417, ErrorsList);
         }
      }

   }
}
