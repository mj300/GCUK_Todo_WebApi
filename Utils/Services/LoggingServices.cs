using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Database.Models;
using todo_webApi.Repositories.Interfaces;

namespace todo_webApi.Utils.Services
{
   public class LoggingService
   {
      private IUnitOfWork UnitOfWork { get; set; }

      public LoggingService() { }

      public LoggingService(IUnitOfWork unitOfWork) =>
         UnitOfWork = unitOfWork;

      internal async Task<AppLog> Log(string message, AppLogType type, dynamic obj = null, User user = null)
      {
         AppLog log = new AppLog(message, type, obj, user);
         await UnitOfWork.AppLogs.AddAsync(log);
         await UnitOfWork.CompleteAsync();
         return log;
      }
      internal async Task<string> LogException(string message, dynamic obj = null, User user = null, AppLogType type = AppLogType.Exception)
      {
         AppLog log = new AppLog(message, type, obj, user);
         await UnitOfWork.AppLogs.AddAsync(log);
         await UnitOfWork.CompleteAsync();
         return AppConst.CommonErrors.ServerError(log.Id);
      }

      internal void AddDbContext(IUnitOfWork dbContext) =>
         UnitOfWork = dbContext;
   }
}
