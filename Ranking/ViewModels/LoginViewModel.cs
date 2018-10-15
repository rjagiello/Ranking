using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ranking.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Wprowadź login")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Wprowadź hasło")]
        [DataType(DataType.Password)]
        [Display(Name ="Hasło")]
        public string Password { get; set; }
        public UserType userType { get; set; }
    }
}
namespace Ranking
{
    public enum UserType
    {
        Team = 0,
        Fan = 1,
        Admin = 2
    }
}