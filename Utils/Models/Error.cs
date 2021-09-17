namespace todo_webApi.Utils.Models
{
   public class Error
   {
      public Error()
      {
      }

      public Error(string key, string value)
      {
         Key = key;
         Value = value;
      }
      public string Key { get; set; } = "0";
      public string Value { get; set; }
   }
}
