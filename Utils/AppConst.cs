using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Utils.Models;

namespace todo_webApi.Utils
{
   internal static class AppConst
   {
      public const string GetAllRecords = "***GET-ALL***";

      private static Settings _settings;
      /// <summary>
      /// Get the information from the appSettings json file
      /// </summary>
      internal static Settings Settings
      {
         get
         {
            if (_settings == null || string.IsNullOrWhiteSpace(_settings.DbConnectionString))
               SetSettings();
            return _settings;
         }
      }

      internal static void SetSettings()
      {
         _settings = new Settings();
         string settingsPath = AppFunc.GetFilePath(@"StaticFiles\Settings.json");

         JsonConvert.PopulateObject(File.ReadAllText(settingsPath), _settings);

         if (!AppFunc.IsDatabaseConnected(_settings.DbConnectionString))
            _settings.DbConnectionString = "";
      }

      public struct CommonErrors
      {
         public static string ServerError(int logID) =>
            $"Your request cannot be processed (Log ID: {logID})";

      }
   }
}
