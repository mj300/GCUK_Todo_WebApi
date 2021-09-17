using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using todo_webApi.Database.Models;
using todo_webApi.Utils.Models;

namespace todo_webApi.Utils
{
   public static class AppFunc
   {

      internal static int GetUserId(HttpContext httpContext)
      {
         if (httpContext.Items["User"] is null)
            return -1;
         return (httpContext.Items["User"] as User).Id;
      }
      internal static User GetUser(HttpContext httpContext)
      {
         if (httpContext.Items["User"] is null)
            return null;
         return (httpContext.Items["User"] as User);
      }
      internal static string HashPassword(string password)
      {
         var hmacMD5 = new HMACMD5(Encoding.ASCII.GetBytes(AppConst.Settings.SaltKey));
         var saltedHash = hmacMD5.ComputeHash(Encoding.ASCII.GetBytes(password));
         return Encoding.ASCII.GetString(saltedHash);
      }

      internal static bool IsDatabaseConnected(string connectionString)
      {
         try
         {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            return true;
         }
         catch (Exception)
         {
            return false;
         }
      }
      public static void Error(ref List<Error> errors, string value, string key = "")
      {
         if (string.IsNullOrWhiteSpace(key))
            key = new Random().Next(1, 20).ToString();
         errors.Add(new Error(key, value));
      }


      /// <summary>
      /// This method is used to extract the errors form model state
      /// </summary>
      /// <param name="ModelState">The instance of model state which contains the errors</param>
      /// <param name="errorList">the reference of error list to add the errors to </param>
      public static void ExtractErrors(ModelStateDictionary ModelState, ref List<Error> errorList)
      {
         int count = 0;
         // Loop through the Model state values
         foreach (var prop in ModelState.Values)
         {
            /// add each error in the model state to the error list
            Error(ref errorList,
                  prop.Errors.FirstOrDefault()?.ErrorMessage,
                  ModelState.Keys.ToArray()[count]);
            count++;
         }
      }


      /// <summary>
      ///     Used to extract the difference between the parameter dateTime and UtcNow.
      /// </summary>
      /// <param name="timeDate">DateTime object to compare with the current UTC time</param>
      /// <returns>Returns a string type which states the time difference. (dd hh mm ss)</returns>
      public static string CompareWithCurrentTime(DateTimeOffset? timeDate)
      {
         if (timeDate == null)
            return "";
         /// checks the difference between the parameter dateTime and
         /// the current UTC time which would return the following format
         /// 00.00:00:00.0000 (days.hours:minutes:seconds.milliseconds)
         var comparedTime = (timeDate - DateTimeOffset.UtcNow);

         /// convert the difference to string and split it at "."
         var initSplit = comparedTime.ToString().Split('.');

         /// switch the split length and split then at correct
         switch (initSplit.Length)
         {
            case 3: // Contains both days and time
               /// since number of days is available then the time would be located at the position "1"
               /// within the initSplit with position 0 = hours, 1 = minutes, 2 = seconds
               var timeSplit3 = initSplit[1].Split(':');
               return string.Format("Day(s): {0}, Hours): {1}, Minute(s): {2}, Second(s): {3}",
                   initSplit[0], timeSplit3[0], timeSplit3[1], timeSplit3[2]);

            case 2:// contains only time and milliseconds
               /// since number of days is not available then the time would be located at the position "0"
               /// within the initSplit with position 0 = hours, 1 = minutes, 2 = seconds
               var timeSplit1 = initSplit[0].Split(':');
               if (timeSplit1[0] != "00")// if hours is not 0
                  return string.Format("{0} hours, {1} minutes and {2} seconds", timeSplit1[0], timeSplit1[1], timeSplit1[2]);
               if (timeSplit1[1] != "00") // if minutes is not 0
                  return string.Format("{0} minutes and {1} seconds.", timeSplit1[1], timeSplit1[2]);
               // if only the seconds is left
               return string.Format("{0} seconds.", timeSplit1[2]);
            default:// show the entire time difference 
               return timeDate.ToString();
         }
      }



      /// <summary>
      /// Get File path relative to root of application
      /// e.g Folder\File.json
      /// </summary>
      /// <returns>file path or if not found string.Empty</returns>
      internal static string GetFilePath(string filePath)
      {
         string path = @$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{filePath}";
         if (!File.Exists(path))
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @$"{filePath}");
         if (!File.Exists(path))
            return string.Empty;
         return path;
      }

      internal static void Log(string txt)
      {
         try
         {

            if (!string.IsNullOrWhiteSpace(txt))
               File.AppendAllText(Path.Combine(@$"{AppDomain.CurrentDomain.BaseDirectory}\StaticFiles\log.txt"), txt + Environment.NewLine);
         }
         catch { }
      }
   }
}
