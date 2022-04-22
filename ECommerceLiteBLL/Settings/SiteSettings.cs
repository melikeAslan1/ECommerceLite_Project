using ECommerceLiteEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceLiteBLL.Settings
{
    public class SiteSettings
    {
        // To do: Mail adresini webconfig dosyasından çekmeyi de öğrenelim.

        public static string SiteMail { get; set; } = "nayazilim303@gmail.com";

        public static string SiteMailPassword { get; set; } = "betul303303";

        public static string SiteMailSmtpHost { get; set; } = "smtp.gmail.com";
        public static int SiteMailSmtpPort { get; set; } = 587;

        public static bool SiteMailEnableSSl = true;

        public async static Task SendMail(MailModel model)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(model.To));
                    message.From = new MailAddress(SiteMail);
                    message.Subject = model.Subject;
                    message.IsBodyHtml = true;
                    message.Body = model.Message;
                    message.BodyEncoding = Encoding.UTF8;

                    if (!string.IsNullOrEmpty(model.Cc))
                    {//modeldeki cc boş değilse.

                        message.CC.Add(new MailAddress(model.Cc));

                    }
                    if (!string.IsNullOrEmpty(model.Bcc))   //modeldeki bcc boş değilse.
                    {//modeldeki cc boş değilse.

                        message.Bcc.Add(new MailAddress(model.Bcc));

                    }

                    var networkCredentials = new NetworkCredential()
                    {
                        UserName = SiteMail,
                        Password = SiteMailPassword
                    };

                    smtp.Credentials = networkCredentials;
                    smtp.Host = SiteMailSmtpHost;
                    smtp.Port = SiteMailSmtpPort;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);



                }
            }
            catch (Exception ex)
            {

                // To Do: ex loglanacak
                // loglama: kayıt tutmak.
            }
        }

        //Bu metod bir hata veriyor hataya bakıp çözmeye çalışacağız.

        //public async static Task SendMail(byte[] array, MailModel model)
        //{
        //    try
        //    {
        //        System.IO.MemoryStream bitmap = new System.IO.MemoryStream(array);
        //        LinkedResource resource = new LinkedResource(bitmap, MediaTypeNames.Image.Jpeg);

        //        resource.ContentId = "Icon";

        //        string htmlBody = @"<html><head><style>"
        //               + "body{font-family:'calibri',sans-serif;}</style></head>"
        //               + "<body>" + model.Message
        //               + "<img style='float:left' width:'250px' height='250px' src='cid:" + resource.ContentId + "'/>"
        //               + "</body></html>";

        //        var message = new MailMessage();
        //        message.To.Add(new MailAddress(model.To));
        //        message.From = new MailAddress(SiteMail);
        //        message.Subject = model.Subject;
        //        message.IsBodyHtml = true;
        //        message.Body = htmlBody;
        //        message.BodyEncoding = Encoding.UTF8;

        //        if (!string.IsNullOrEmpty(model.Cc))
        //        {//modeldeki cc boş değilse.

        //            message.CC.Add(new MailAddress(model.Cc));

        //        }
        //        if (!string.IsNullOrEmpty(model.Bcc))   //modeldeki bcc boş değilse.
        //        {//modeldeki cc boş değilse.

        //            message.Bcc.Add(new MailAddress(model.Bcc));

        //        }

        //        var networkCredentials = new NetworkCredential()
        //        {
        //            UserName = SiteMail,
        //            Password = SiteMailPassword
        //        };

        //        //
        //        AlternateView alternetiveView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

        //        alternetiveView.LinkedResources.Add(resource);

        //        message.AlternateViews.Add(alternetiveView);

        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Credentials = networkCredentials;
        //        smtp.Host = SiteMailSmtpHost;
        //        smtp.Port = SiteMailSmtpPort;
        //        smtp.EnableSsl = true;
        //        await smtp.SendMailAsync(message);
        //    }
        //    catch (Exception ex)
        //    {

        //        // ex loglanacak.
        //    }
        //}




        public static void SendMail(byte[] array, MailModel model)
        {
            try
            {
                System.IO.MemoryStream bitmap = new System.IO.MemoryStream(array);
                LinkedResource resource = new LinkedResource(bitmap, MediaTypeNames.Image.Jpeg);

                resource.ContentId = "Icon";

                string htmlBody = @"<html><head><style>"
                       + "body{font-family:'calibri',sans-serif;}</style></head>"
                       + "<body>" + model.Message
                       + "<img style='float:left' width:'250px' height='250px' src='cid:" + resource.ContentId + "'/>"
                       + "</body></html>";

                var message = new MailMessage();
                message.To.Add(new MailAddress(model.To));
                message.From = new MailAddress(SiteMail);
                message.Subject = model.Subject;
                message.IsBodyHtml = true;
                message.Body = htmlBody;
                message.BodyEncoding = Encoding.UTF8;

                if (!string.IsNullOrEmpty(model.Cc))
                {//modeldeki cc boş değilse.

                    message.CC.Add(new MailAddress(model.Cc));

                }
                if (!string.IsNullOrEmpty(model.Bcc))   //modeldeki bcc boş değilse.
                {//modeldeki cc boş değilse.

                    message.Bcc.Add(new MailAddress(model.Bcc));

                }

                var networkCredentials = new NetworkCredential()
                {
                    UserName = SiteMail,
                    Password = SiteMailPassword
                };

                //
                AlternateView alternetiveView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

                alternetiveView.LinkedResources.Add(resource);

                message.AlternateViews.Add(alternetiveView);

                SmtpClient smtp = new SmtpClient();
                smtp.Credentials = networkCredentials;
                smtp.Host = SiteMailSmtpHost;
                smtp.Port = SiteMailSmtpPort;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
            catch (Exception ex)
            {

                // ex loglanacak.
            }
        }



        public static string StringCharacterConverter(string name)
        {
            string resultString=
              name.Replace("'", "")
            .Replace(" ", "-")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("&", "")
            .Replace("[", "")
            .Replace("!", "")
            .Replace("]", "")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("İ", "I")
            .Replace("Ö", "O")
            .Replace("Ü", "U")
            .Replace("Ş", "S")
            .Replace("Ç", "C")
            .Replace("Ğ", "G")
            .Replace("|", "")
            .Replace(".", "-")
            .Replace("?", "-")
            .Replace(";", "-");

            return resultString.ToLower();
        } 




    }
}
