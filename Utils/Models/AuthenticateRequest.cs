using System.ComponentModel.DataAnnotations;

namespace todo_webApi.Utils.Models
{
   public class AuthenticateRequest
   {
      [Required]
      public string Username { get; set; }

      [Required]
      public string Password { get; set; }
   }
}
