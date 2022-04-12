using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteUI.Models
{
   public class PasswordChangeViewModel
    {
        [Required]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
     The password's first character must be a letter, it must contain at least 5 characters and no more than 15 characters and no characters other than letters, numbers and the underscore may be used")]

        public string Password { get; set; }


        [Required]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
      The password's first character must be a letter, it must contain at least 5 characters and no more than 15 characters and no characters other than letters, numbers and the underscore may be used")]

        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler Uyuşmuyor!")]
        public string ConfirmPassword { get; set; }

    }
}
