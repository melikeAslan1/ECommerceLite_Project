using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProfileViewModel : RegisterViewModel
    {
        //ad, soyad, email, username, şifre ve şifre tekrarı, yeni şifre

        [Required]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
                            The password's first character must be a letter, it must contain at least 5 
                                characters and no more than 15 characters and no characters other than 
                                letters, numbers and the underscore may be used")]
        public string NewPassword { get; set; }


    }
}