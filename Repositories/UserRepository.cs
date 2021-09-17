using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Database;
using todo_webApi.Database.Models;
using todo_webApi.Repositories.Interfaces;
using todo_webApi.Utils;
using todo_webApi.Utils.Models;

namespace todo_webApi.Repositories
{
   public class UserRepository : Repository<User>, IUserRepository
   {
      public UserRepository(AppDbContext context)
            : base(context)
      {
      }

      public async Task<AuthenticateResponse> AddUser(UserRegistrationInfo model)
      {
         User user = new User()
         {
            FirstName = model.FirstName,
            Password = AppFunc.HashPassword(model.Username.ToLower() + model.Password),
            LastName = model.LastName,
            Username = model.Username
         };
         var result = await _DbContext.Users.AddAsync(user);
         await _DbContext.SaveChangesAsync();
         // return null if user not found
         if (result.Entity == null) return null;

         // authentication successful so generate jwt token
         var token = generateJwtToken(result.Entity);

         return new AuthenticateResponse(result.Entity, token);
      }

      public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
      {
         var user = await _DbContext.Users.SingleOrDefaultAsync(x => x.Username == model.Username && x.Password == AppFunc.HashPassword(model.Username.ToLower() + model.Password));

         // return null if user not found
         if (user == null) return null;

         // authentication successful so generate jwt token
         var token = generateJwtToken(user);

         return new AuthenticateResponse(user, token);
      }

      public async Task<bool> CheckUserName(string userName)
      {
         var user = await _DbContext.Users.SingleOrDefaultAsync(u => u.Username.ToLower() == userName.ToLower());

         return !(user is null);
      }

      public async Task<User> GetById(int id)
      {
         var user = await _DbContext.Users.AsTracking().SingleOrDefaultAsync(u => u.Id == id);
         return user;
      }

      private string generateJwtToken(User user)
      {
         // generate token that is valid for 7 days
         var tokenHandler = new JwtSecurityTokenHandler();
         var key = Encoding.ASCII.GetBytes(AppConst.Settings.Secret);
         var tokenDescriptor = new SecurityTokenDescriptor
         {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
         };
         var token = tokenHandler.CreateToken(tokenDescriptor);
         return tokenHandler.WriteToken(token);
      }


   }
}
