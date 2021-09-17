using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using todo_webApi.Database.Models;
using todo_webApi.Utils.Models;

namespace todo_webApi.Repositories.Interfaces
{
   public interface IUserRepository : IRepository<User>
   {
      Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
      Task<AuthenticateResponse> AddUser(UserRegistrationInfo model);
      Task<bool> CheckUserName(string userName);
      Task<User> GetById(int id);

   }
}
