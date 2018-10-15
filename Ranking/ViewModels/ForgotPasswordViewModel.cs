﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ranking.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "{0} musi mieć co najmniej {2} i nie więcej niż {1} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name ="Hasło")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Hasła nie pasują do siebie")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź Hasło")]
        public string ConfirmPassword { get; set; }
    }
}