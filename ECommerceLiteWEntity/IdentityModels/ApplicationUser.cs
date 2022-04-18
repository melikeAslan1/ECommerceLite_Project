using ECommerceLiteEntity.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        //identtiyUser'dan kalıtım alındı. Identity User Microsoft'un identity şemasına ait bir classtır.
        // IdentityUser classı ile bize sunulan AspNetUsers tablosundaki kolonların genişletmek için kalıtım aldık.
        //Aşağıya ihtiyacımız olan kolonları ekledik

        [Required]
        [Display(Name = "Ad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "İsminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyad")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Soyisminizin uzunluğu 2 ile 30 karakter aralığında olmalıdır!")]
        public string Surname { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        //ToDo: Guid'in kaç haneli olduğuna bakıp buraya string length ile attribute tanımlanacaktır
        public string ActivationCode { get; set; }
        public bool IsDeleted { get; set; } = false;


        // İsteyen birthDate gibi bir alan da ekleyebilir.

        public virtual List<Admin> AdminList { get; set; }
        public virtual List<Customer> CustomerList { get; set; }
        public virtual List<PassiveUser> PassiveUserList { get; set; }

    }
}
