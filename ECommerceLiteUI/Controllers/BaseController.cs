using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    public class BaseController : Controller
    {
        [NonAction] // sayfa göndermiyecek mesela string gibi bişey gönderecek.
        public string CreateRandomNewPassword()
        {
            //abcd1234
            Random rnd = new Random();
            int number = rnd.Next(1000, 5000);
            char[] guidString = Guid.NewGuid().ToString().Replace("-", "").ToArray();

            string newPassword = string.Empty;

            for (int i = 0; i < guidString.Length; i++)
            {
                if (newPassword.Length == 5)
                    break;
                if(char.IsLetter(guidString[i]))
                {
                    newPassword += guidString[i];
                }

            }

            newPassword += number;
            return newPassword;

        }

        [NonAction]
        public static byte [] BitmapToByArray(Bitmap bitmap)
        {
          //BitMap şeklindeki detayı belleğe byte serisi şeklinde kaydediyor.
                using (MemoryStream ms=new MemoryStream())
                {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
                }
            
        }

    }
}