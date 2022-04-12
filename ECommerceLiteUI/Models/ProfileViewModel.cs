using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProfileViewModel 
    {
        //Ad, Soyad, Email, UserName, Şifre ve Şifre Tekrarı, Yeni Şifre 

        //TODO: Zaman kalırsa proje bitince buraya geri dön ve TCKimliği, Emaili de
        //güncellemeye izin verelim

        [Required]
        [Display(Name = "Ad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "İsminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Soyisminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
The password's first character must be a letter, it must contain at least 5 characters and no more than 15 characters and no characters other than letters, numbers and the underscore may be used")]

        public string Password { get; set; }
        // [Required] --> TODO yapılınca required yorum satırı olmasın
        [EmailAddress]
        public string Email { get; set; }


    }
}