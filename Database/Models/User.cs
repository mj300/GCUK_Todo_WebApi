using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace todo_webApi.Database.Models
{
   [Table("Users")]
   public class User
   {
      [Key]
      public int Id { get; set; }

      [StringLength(120, ErrorMessage = "Must be less than 120 Characters \n")]
      [Required(ErrorMessage = "FirstName is Required \n")]
      public string FirstName { get; set; }

      [StringLength(120, ErrorMessage = "Must be less than 120 Characters \n")]
      [Required(ErrorMessage = "LastName is Required \n")]
      public string LastName { get; set; }

      [StringLength(120, ErrorMessage = "Must be less than 120 Characters \n")]
      [Required(ErrorMessage = "Username is Required \n")]
      public string Username { get; set; }

      [JsonIgnore]
      public string Password { get; set; }

      [InverseProperty("User")]
      [JsonIgnore]
      public ICollection<TodoItem> Todos { get; set; }
   }
}
