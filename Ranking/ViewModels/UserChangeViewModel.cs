using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Ranking.ViewModels
{
    public class UserChangeViewModel
    {
        [Required]
        [StringLength(30)]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [StringLength(30, ErrorMessage = "{0} musi mieć co najmniej {2} i nie więcej niż {1} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Hasła nie pasują do siebie")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź Hasło")]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Kapitan")]
        public string Captain { get; set; }
    }
}