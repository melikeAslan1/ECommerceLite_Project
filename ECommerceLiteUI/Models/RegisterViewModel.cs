using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class RegisterViewModel
    {   //Kayıt Modeli içinde siteye kayıt olmak isteyen kişilerden hangi bilgileri alacağımız belirleyeceğiz
        // TCKimlik, İsim soyisim email (eğer yazdıysak telefon, cinsiyet vb)  alanlarını tanımlayalım

        //Not: Data Annotation'ları kullanarak validation kurallarını belirlediğimiz için
        //kapsüllemeye gerek kalmadı. 

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Tc kimlik Numarası 11 haneli olmalıdır!")]
        [Display(Name = "Tc Kimlik")]
        public string TCNumber { get; set; }
        [Required]
        [Display(Name = "Ad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "İsminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Soyisminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z]\w{4,14}$", ErrorMessage = @"	
           The password's first character must be a letter, it must contain at least 5 characters and no more than 
          15 characters and no characters other than letters, numbers and the underscore may be used")]
                     
        public string Password { get; set; }

        [Required]
        [Display(Name = "Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Şifreler Uyuşmuyor.")]
        public string ConfirmPassword { get; set; }


    }
}