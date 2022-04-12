using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Account;
using ECommerceLiteBLL.Repository;
using ECommerceLiteBLL.Settings;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteEntity.Models;
using ECommerceLiteUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ECommerceLiteEntity.Enums;
using System.Threading.Tasks;
using ECommerceLiteEntity.ViewModels;
using Microsoft.Owin.Security;

namespace ECommerceLiteUI.Controllers
{
    public class AccountController : BaseController
    {
        //Global alan 
        //Not: Bir sonraki projede repoları UI'ın içinde NEW'lemeyeceğiz!
        //Çünkü bu bağımlılık oluşturur! Bir sonraki projede bağımlılıkları tersine çevirme işlemi olarak bilinen Dependency Injection işlemleri yapacağız.

        CustomerRepo myCustomerRepo = new CustomerRepo();
        PassiveUserRepo myPassiveUserRepo = new PassiveUserRepo();
        UserManager<ApplicationUser> myUserManager = MembershipTools.NewUserManager();
        UserStore<ApplicationUser> myUserStore = MembershipTools.NewUserStore();
        RoleManager<ApplicationRole> myRoleManager = MembershipTools.NewRoleManager();


        [HttpGet]
        public ActionResult Register()
        {

            //Zaten giriş yapmış biri bu sayfayı tekrar çağırdığında Home-Indexe gitsin
            if (MembershipTools.GetUser() != null)
            {
                // TO DO: Acaba kişinin gittiği URL'i tutup oraya
                // geri gönderme nasıl yaparız?
                return RedirectToAction("Index", "Home");

            }



            //Kayıt ol sayfası 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) // model validasyonları sağladı mı?
                {
                    return View(model);
                }
                var checkUserTC = myUserStore.Context.Set<Customer>()
                    .FirstOrDefault(x => x.TCNumber == model.TCNumber)?.TCNumber;
                if (checkUserTC != null) // Buldu!!!
                {
                    ModelState.AddModelError("", "Bu TC numarası ile daha önceden sisteme kayıt yapılmıştır!");
                    return View(model);
                }
                //To Do: soru işareti silinip debuglanacak
                var checkUserEmail = myUserStore.Context.Set<ApplicationUser>()
                    .FirstOrDefault(x => x.Email == model.Email)?.Email;
                if (checkUserEmail != null)
                {
                    ModelState.AddModelError("", "Bu email ile daha önceden sisteme kayıt yapılmıştır!");
                    return View(model);
                }


                // aktivasyon kodu üretelim
                var activationCode = Guid.NewGuid().ToString().Replace("-", "");

                // Artık sisteme kayıt olabilir...
                var newUser = new ApplicationUser()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    UserName = model.Email,
                    ActivationCode = activationCode
                };

                // artık ekleyelim
                var createResult = myUserManager.CreateAsync(newUser, model.Password);

                //To Do: createResult.Isfault ne acaba? debuglarken bakalım 
                if (createResult.Result.Succeeded)
                {
                    // görev başarıyla tamamlandıysa kişi aspnetusers tablosuna eklenmiştir!
                    // Yeni kayıt olduğu için bu kişiye pasif rol verilecektir!
                    // Kişi emailine gelen aktivasyon koduna tıklarsa pasiflikten çıkığ customer olabilir.

                    await myUserManager.AddToRoleAsync(newUser.Id, Roles.Passive.ToString());
                    PassiveUser myPassiveUser = new PassiveUser()
                    {
                        UserId = newUser.Id,
                        TCNumber = model.TCNumber,
                        IsDeleted = false,
                        LastActiveTime = DateTime.Now
                    };
                    //  myPassiveUserRepo.Insert(myPassiveUser);
                    await myPassiveUserRepo.InsertAsync(myPassiveUser);
                    // email gönderilecek
                    // site adresini alıyoruz.
                    var siteURL = Request.Url.Scheme + Uri.SchemeDelimiter
                        + Request.Url.Host +
                        (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    await SiteSettings.SendMail(new MailModel()
                    {
                        To = newUser.Email,
                        Subject = "ECommerceLite Site Aktivasyon Emaili",
                        Message = $"Merhaba {newUser.Name} {newUser.Surname}," +
                        $"<br/>Hesabınızı aktifleştirmek için <b>" +
                        $"<a href='{siteURL}/Account/Activation?" +
                        $"code={activationCode}'>Aktivasyon Linkine</a></b> tıklayınız..."
                    });
                    // işlemler bitti...
                    return RedirectToAction("Login", "Account",
                        new { email = $"{newUser.Email}" });
                }
                else
                {
                    ModelState.AddModelError("", "Kayıt işleminde beklenmedik bir hata oluştu!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                //To Do: loglama yapılacak
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! Tekrar deneyiniz!");
                return View(model);

            }
        }

        [HttpGet]
        public async Task<ActionResult> Activation(string code)
        {
            try
            {
                //select * from AspNetUsers where Activationcode='xzfdgdhgdsfgdsfgds'
                var user =
                    myUserStore.Context.Set<ApplicationUser>()
                    .FirstOrDefault(x => x.ActivationCode == code);
                if (user == null)
                {
                    ViewBag.ActivationResult = "Aktivasyon işlemi başarısız! Sistem yöneticisinden yeniden email isteyiniz..";
                    return View();
                }
                //user bulundu!
                if (user.EmailConfirmed) // zaten aktifleşmiş mi?
                {
                    ViewBag.ActivationResult = "Aktivasyon işleminiz zaten gerçekleşmiştir! Giriş yaparak sistemi kullanabilisiniz";
                    return View();
                }
                user.EmailConfirmed = true;
                await myUserStore.UpdateAsync(user);
                await myUserStore.Context.SaveChangesAsync();
                //Bu kişi artık aktif! 
                PassiveUser passiveUser = myPassiveUserRepo
                    .AsQueryable().FirstOrDefault(x => x.UserId == user.Id);
                if (passiveUser != null)
                {
                    //TODO:PassiveUser tablosuna TargetRole ekleme işlemini daha sonra yapalım. Kafalarındaki soru işareti gittikten sonra...

                    passiveUser.IsDeleted = true;
                    myPassiveUserRepo.Update();

                    Customer customer = new Customer()
                    {
                        UserId = user.Id,
                        TCNumber = passiveUser.TCNumber,
                        IsDeleted = false,
                        LastActiveTime = DateTime.Now
                    };

                    await myCustomerRepo.InsertAsync(customer);

                    //Aspnetuserrole tablosuna bu kişinin artık customer mertebesine ulaştığını bildirelim
                    myUserManager.RemoveFromRole(user.Id, Roles.Passive.ToString());
                    myUserManager.AddToRole(user.Id, Roles.Customer.ToString());
                    //işlem bitti başarılı old. dair mesajı gönderelim.

                    ViewBag.ActivationResult = $"Merhaba Sayın {user.Name} {user.Surname}, aktifleştirme işleminiz başarılıdır! Giriş yapıp sistemi kullanabilirsiniz";
                    return View();
                }

                //NOT: Müsait olduğunuzda  bireysel bir beyin fırtınası yapabilirsiniz.
                //Kendinize şu soruyu sorun! PassiveUser null gelirse nasıl bir yol izlenilebilir??
                //PassiveUser null gelmesi çok büyük bir problem mi?
                //Customerda bu kişi kayıtlı mı? Customerda bir problem yok.. Customer kayıtlı değilse PROBLEM VAR! 
                // Buraya yazılması gereken mini minnacık kodları şimdilik size bırakmış gibi yapıyporum sonra birlikte tekrar bakacağız.

                return View();


            }
            catch (Exception ex)
            {

                //TODO: loglama yapılacak 
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                return View();
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult UserProfile()
        {
            //login olmuş kişinin id bilgisini alalım
            var user = myUserManager.FindById(HttpContext.User.Identity.GetUserId());
            if (user != null)
            {
                // kişiyi bulacağız ve mevcut bilgilerini ProfileViewModele atayıp sayfayta göndereceğiz.
                ProfileViewModel model = new ProfileViewModel()
                {
                    Name = user.Name,
                    Surname = user.Surname
                };
                return View(model);
            }
            //User null ise ( temkinli davrandık...)
            ModelState.AddModelError("", "Beklenmedik bir sorun oluşmuş olabilir mi? Giriş yapıp, tekrar deneyiniz! Sizinle tekrar buluşalım! ");
            return View();

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(ProfileViewModel model)
        {
            try
            {
                //Sisteme kayıt olmuş ve login ile giriş yapmış kişi Hesabıma tıkladı
                //Bilgilerini gördü. Bilgilerinde değişiklik yaptı.Biz burada kontrol edeceğiz. Yapılan değişiklikleri tespit edip db'mizi güncelleyeceğiz...
                var user = myUserManager.FindById(HttpContext.User.Identity.GetUserId());
                if (user == null)
                {
                    ModelState.AddModelError("", "Mevcut kullanıcı bilgilerinize ulaşılamadığı için işlem yapamıyoruz");
                    return View(model);
                }
                // Bir user herhangi bir bilgisini değişecekse PAROLASINI girmek zorunda
                // Bu nedenle model ile gelen parola DB'edki parola ile eşleşiyor mu diye bakmak lazım...
                if (myUserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("", "Mevcut şifrenizi yanlış girdiğiniz için bilgilerinizi güncelleyemedik! Lütfen tekrar deneyiniz!");
                    return View(model);
                }
                //başarılıysa yani parolayı doğru yazdı!
                // bilgilerini güncelleyeceğiz

                user.Name = model.Name;
                user.Surname = model.Surname;
                await myUserManager.UpdateAsync(user);
                ViewBag.Result = "Bilgileriniz güncellendi";
                var updatedModel = new ProfileViewModel()
                {
                    Name = user.Name,
                    Surname = user.Surname
                };
                return View(updatedModel);

            }
            catch (Exception ex)
            {

                // ex loglanacak 
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! Tekrar deneyiniz");
                return View(model);
            }

        }


        [HttpGet]
        [Authorize]
        public ActionResult UpdatePassword()
        {
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(PasswordChangeViewModel model)
        {
            try
            {
                //Mevcutt login olmuş kişinin Id'sini veriyor. O id ile manager kişiyi db'den bulup getiriyor
                var user = myUserManager.FindById(HttpContext.User.Identity.GetUserId());

                //Ya şifreler aynısıydı??
                if (myUserManager.PasswordHasher
                    .VerifyHashedPassword(user.PasswordHash, model.NewPassword) == PasswordVerificationResult.Success)
                {
                    //Bu kişi mevcut şifresinin aynısını yeni şifre olarak yutturmaya çalışıyor.
                    ModelState.AddModelError("", "Yeni şifreniz mevcut şifrenizle aynı olmasın madem değiştirmek istedin!!");
                    return View(model);
                }

                //Yeni şifre ile şifre tekrarı uyuşuyor mu?
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Şifreler uyuşmuyor!");
                    return View(model);
                }

                //Acaba mevcut şifresini doğru yazdı mı?
                var checkCurrent = myUserManager.Find(user.UserName, model.Password);
                if (checkCurrent == null)
                {
                    // Mevcut şifresini yanlış yazmış!
                    ModelState.AddModelError("", "Mevcut şifrenizi yanlış girdiğiniz yeni şifre oluşturma işleminiz başarısız oldu! Tekrar deneyiniz");
                    return View(model);
                }
                // Artık şifresini değiştirebilir. 

                await myUserStore.SetPasswordHashAsync(user, myUserManager.PasswordHasher.HashPassword(model.NewPassword));

                await myUserManager.UpdateAsync(user);
                //Şifre değiştirdikten sonra sistemden atalım!
                TempData["PasswordUpdated"] = "Parolanız değiştirildi";
                HttpContext.GetOwinContext().Authentication.SignOut();
                return RedirectToAction("Login", "Account",
                    new { email = user.Email });

            }
            catch (Exception ex)
            {
                //ex loglanacak
                ModelState.AddModelError("", "Beklenmedik hata oldu! Tekrar deneyiniz");
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoverPassword(ProfileViewModel model)
        {
            try
            {
                //Şifresini unutmuş. 

                var user = myUserStore.Context.Set<ApplicationUser>()
                    .FirstOrDefault(x => x.Email == model.Email);


                if (user == null)
                {
                    ViewBag.RecoverPassword = "Sistemde böyle bir kullanıcı olmadığı için size yeni şifre gönderemiyoruz! Lütfen önce sisteme kayıt olunuz";
                    return View(model);
                }
                //Random şifre oluştur!
                var randomPassword = CreateRandomNewPassword();
                await myUserStore.SetPasswordHashAsync(user, myUserManager
                    .PasswordHasher.HashPassword(randomPassword));
                await myUserStore.UpdateAsync(user);

                // email gönderilecek
                // site adresini alıyoruz.
                var siteURL = Request.Url.Scheme + Uri.SchemeDelimiter
                    + Request.Url.Host +
                    (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                await SiteSettings.SendMail(new MailModel()
                {
                    To = user.Email,
                    Subject = "ECommerceLite - Şifre Yenilendi!",
                    Message = $"Merhaba {user.Name} {user.Surname}," +
                    $"<br/>Yeni şifreniz:<b> {randomPassword} </b> Sisteme Giriş" +
                    $"yapmak için <b>" +
                    $"<a href='{siteURL}/Account/Login?" +
                    $"email={user.Email}'>BURAYA</a></b> tıklayınız..."
                });
                // işlemler bitti...
                ViewBag.RecoverPassword = "Email adresinize şifre gönderilmiştir.";
                return View();

            }
            catch (Exception ex)
            {
                //Todo ex loglanacak
                ViewBag.RecoverPasswordResult = "Sistemsel bir hata oluştu! Tekrar deneyiniz!";
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl, string email)
        {
            try
            {
                //Zaten giriş yapmış biri bu sayfayı tekrar çağırdığında Home-Indexe gitsin
                if (MembershipTools.GetUser() != null)
                {
                    // TO DO: Acaba kişinin gittiği URL'i tutup oraya
                    // geri gönderme nasıl yaparız?
                    return RedirectToAction("Index", "Home");

                }

                //TO DO: sayfa patlamazsa if kontrolüne gerek yok! Test ederken bakacağız
                var model = new LoginViewModel()
                {
                    ReturnUrl = ReturnUrl,
                    Email = email
                };
                return View(model);
            }
            catch (Exception ex)
            {

                //ex loglanacak
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await myUserManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Emailinizi ya da şifrenizi yanlış girdiniz!");
                    return View(model);
                }
                //User'ı buldu ama rolü pasif ise sisteme giremesin!
                if (user.Roles.FirstOrDefault().RoleId == myRoleManager.FindByName(Enum.GetName(typeof(Roles), Roles.Passive)).Id)

                {
                    ViewBag.Result = "Sistemi kullanmak için aktivasyon yapmanız gerekmektedir! Emailinize gönderilen aktivasyon linkine tıklayınız!";
                    //TO DO: Zaman kalırsa Email Gönder adında küçük bir buton burada olsun
                    return View(model);
                }
                //Artık login olabilir
                var authManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity =
                    await myUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                //2. yol
                AuthenticationProperties authProperties = new AuthenticationProperties();
                authProperties.IsPersistent = model.RememberMe;
                authManager.SignIn(authProperties, userIdentity);

                //1. yol
                //authManager.SignIn(new AuthenticationProperties()
                //{
                //    IsPersistent=model.RememberMe
                //}, userIdentity);


                //Giriş yaptı! Peki nereye gidecek? 
                //Herkes rolüne uygun default bir sayfaya gitsin
                if (user.Roles.FirstOrDefault().RoleId ==
                    myRoleManager.FindByName(
                     Enum.GetName(typeof(Roles), Roles.Admin)
                    ).Id)

                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                if (user.Roles.FirstOrDefault().RoleId ==
                    myRoleManager.FindByName(
                     Enum.GetName(typeof(Roles), Roles.Customer)
                    ).Id)

                {
                    return RedirectToAction("Index", "Home");
                }
                if (string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }

                //ReturnURL dolu ise??? 
                var url = model.ReturnUrl.Split('/'); // Split ettik
                if (url.Length == 4)
                {
                    return RedirectToAction(url[2], url[1], new { id = url[3] });
                }
                else
                {
                    //ÖRN: return RedirectToAction("UserProfile", "Account");
                    return RedirectToAction(url[2], url[1]);
                }
            }
            catch (Exception ex)
            {
                //ex loglanacak
                ModelState.AddModelError("", "Beklenmedik hata oluştu! Tekrar deneyiniz!");
                return View(model);
            }
        }


        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();
            var user = MembershipTools.GetUser();
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "Account"
                , new { email = user.Email });
        }

    }
}