using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_webApi.Database.Models
{
   [Table("Todos")]
   public class TodoItem
   {
      [Key]
      public string Id { get; set; } = Guid.NewGuid().ToString();

      public bool IsCompelete { get; set; }

      [StringLength(120, ErrorMessage = "Must be less than 120 Characters")]
      [Required(ErrorMessage = "Name is Required")]
      public string Name { get; set; }

      [JsonIgnore]
      [ForeignKey("UserId")]
      public User User { get; set; }

      [NotMapped]
      public int UserId
      {
         get { return User is null ? 0 : User.Id; }
      }
   }
}
