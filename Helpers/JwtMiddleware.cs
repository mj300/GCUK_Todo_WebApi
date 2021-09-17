using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Database;
using todo_webApi.Repositories;
using todo_webApi.Repositories.Interfaces;
using todo_webApi.Utils;

namespace todo_webApi.Helpers
{
   public class JwtMiddleware
   {
      private readonly RequestDelegate _next;

      public JwtMiddleware(RequestDelegate next)
      {
         _next = next;

      }
      IUnitOfWork UnitOfWork;

      public async Task Invoke(HttpContext context, AppDbContext dbContext)
      {
         var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
         UnitOfWork = new UnitOfWork(dbContext);

         if (token != null)
            await AttachUserToContext(context, token);

         await _next(context);
      }

      private async Task AttachUserToContext(HttpContext context, string token)
      {
         try
         {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConst.Settings.Secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(key),
               ValidateIssuer = false,
               ValidateAudience = false,
               // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
               ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // attach user to context on successful jwt validation
            context.Items["User"] = await UnitOfWork.Users.GetById(userId);
         }
         catch
         {
            // do nothing if jwt validation fails
            // user is not attached to context so request won't have access to secure routes
         }
      }
   }
}
