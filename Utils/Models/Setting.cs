using Newtonsoft.Json;

namespace todo_webApi.Utils.Models
{
   internal class Settings
   {
      /// <summary>
      /// The array of allowed CORs (Cross-Origin Request) URL
      /// which are allowed to connect to the web API
      /// </summary>
      [JsonProperty(PropertyName = "OpenCors")]
      internal string[] OpenCors { get; set; }

      /// <summary>
      /// Exclude specific routes from CORs check.
      /// Must provide the URI <br/>
      /// e.g. "/Images/test.png" specific file or "/Images" Folder or "/user/post" Route
      /// </summary>
      [JsonProperty(PropertyName = "ExcludedRoutesFromCORS")]
      internal string[] ExcludedRoutesFromCORS { get; set; }



      [JsonProperty(PropertyName = "DbConnectionString")]
      internal string DbConnectionString { get; set; }

      [JsonProperty(PropertyName = "Secret")]
      internal string Secret { get; set; }

      [JsonProperty(PropertyName = "SaltKey")]
      internal string SaltKey { get; set; }

   }

}
