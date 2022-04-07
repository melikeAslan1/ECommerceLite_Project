using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class LoginViewModel  //LoginVM olarak da yazılabilir.
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }


        //Authorize istenen bir action a gitmek isterse sayfa login atıcak kullanıcıyı
        //Kullanıcı bilgilerini girerse onu istediği Authorize lı sayfaya direk göndermek için gitmek istediği
        //url bilgisini bu property'de tutuyoruz.
        public string ReturnUrl { get; set; }




    }
}