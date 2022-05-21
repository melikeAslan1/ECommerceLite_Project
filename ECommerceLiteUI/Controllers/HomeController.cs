using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;
using ECommerceLiteUI.Models;
using Mapster;
using System.Threading.Tasks;
using ECommerceLiteBLL.Account;
using ECommerceLiteEntity.Models;
using QRCoder;
using System.Drawing;
using ECommerceLiteBLL.Settings;
using ECommerceLiteEntity.ViewModels;
using ECommerceLiteUI.LogManaging;

namespace ECommerceLiteUI.Controllers
{
    public class HomeController : BaseController
    {
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductRepo myProductRepo = new ProductRepo();
        AdminRepo myAdminRepo = new AdminRepo();
        CustomerRepo myCustomerRepo = new CustomerRepo();
        OrderRepo myOrderRepo = new OrderRepo();
        OrderDetailRepo myOrderDetailRepo = new OrderDetailRepo();

        public ActionResult Index()
        {
            // Ana kategorilerden 4 tanesini viewbag ile sayfaya gönderelim
            var categoryList = myCategoryRepo.AsQueryable()
                .Where(x => x.BaseCategoryId == null).Take(3).ToList();

            ViewBag.CategoryList = categoryList.OrderByDescending(x => x.Id).ToList();

            //ürünler
            var productList = myProductRepo.AsQueryable()
                .Where(x => !x.IsDeleted && x.Quantity >= 1).Take(10).ToList();
            List<ProductViewModel> model = new List<ProductViewModel>();

            //Mapster ile mapledik.

            productList.ForEach(x =>
            {
                var item = x.Adapt<ProductViewModel>();
                item.GetCategory();
                item.GetProductPictures();
                model.Add(item);
            });

            //üstteki ile aynı, LİNQ suz.
            //foreach (var item in productList)
            //{
            //    var item = x.Adapt<ProductViewModel>();
            //    item.GetCategory();
            //    item.GetProductPictures();
            //    model.Add(item);
            //}


            // mapstersiz.
            //foreach (var item in productList)
            //{

            //    var product = new ProductViewModel()
            //    {
            //        Id = item.Id,
            //        CategoryId = item.CategoryId,
            //        ProductName = item.ProductName,
            //        Description = item.Description,
            //        Quantity = item.Quantity,
            //        Discount = item.Discount,
            //        RegisterDate = item.RegisterDate,
            //        Price = item.Price,
            //        ProductCode = item.ProductCode
            //        //isDeleted alanını viewmodelin içine eklemeyi unuttuk. Çünkü
            //        // isDeleted alanını daha dün ekledik. Viewmodeli geçen hafta oluşturduk
            //    };
            //    product.GetCategory();
            //    product.GetProductPictures();
            //    model.Add(product);
            //}


            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddToCart(int id)
        {
            try
            {
                // Session a eklenecek DB ye eklenmeyecek. Asp.Net mvc session nedir araştır.

                var shoppingCart = Session["ShoppingCart"] as List<ProductViewModel>;

                if (shoppingCart == null)
                {
                    shoppingCart = new List<ProductViewModel>();
                }
                if (id > 0)
                {
                    var product = myProductRepo.GetById(id);
                    if (product == null)
                    {
                        TempData["AddToCartFailed"] = "Ürün eklemesi başarısızdır. Lütfen tekrar deneyiniz";
                        //product null geldi logla.
                        return RedirectToAction("Index", "Home");
                    }
                    // tamam ekleme yapılacak

                    //  var productAddToCart = product.Adapt<ProductViewModel>();

                    var productAddToCart = new ProductViewModel()
                    {
                        Id = product.Id,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        Discount = product.Discount,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        RegisterDate = product.RegisterDate,
                        ProductCode = product.ProductCode,

                    };

                    if (shoppingCart.Count(x => x.Id == productAddToCart.Id) > 0)
                    {
                        shoppingCart.FirstOrDefault(x => x.Id == productAddToCart.Id).Quantity++;
                    }
                    else
                    {
                        productAddToCart.Quantity = 1;
                        shoppingCart.Add(productAddToCart);
                    }
                    //Önemli !! --> Session'a bu listeyi ataamamız gerekli.

                    Session["ShoppingCart"] = shoppingCart;
                    TempData["AddToCartSuccess"] = "Ürün sepete eklendi";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["AddToCartFailed"] = "Ürün eklemesi başarısızdır. Lütfen tekrar deneyiniz";
                    //Loglama yap id düzgün gelmedi

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {

                // ex loglanacak.
                TempData["AddToCartFailed"] = "Ürün eklemesi başarısızdır. Lütfen tekrar deneyiniz";

                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<ActionResult> Buy()
        {
            try
            {
                // 1) Eğer admin isen alışveriş yapamazsın.
                var user = MembershipTools.GetUser();

                //select * from Admins 
                //where UserId= 'buraya userın id si gelir'

                var customer = myCustomerRepo.AsQueryable().FirstOrDefault(x => x.UserId == user.Id);
                if (customer == null)
                {
                    TempData["BuyFailed"] = "Alışverişi yapabilmeniz için müşteri bilgileriniz ile giriş yapamanız gerekmektedir.";
                    return RedirectToAction("Index", "Home");
                }

                // 2) shoppingcart null mı değil mi?

                var shoppingcart = Session["ShoppingCart"] as List<ProductViewModel>;

                if (shoppingcart == null)
                {
                    TempData["BuyFailed"] = "Alışveriş yapmak için sepetinize ürün ekleyiniz!";
                    return RedirectToAction("Index", "Home");
                }
                //// 3) shoppingcart içinde ürün var mı ?
                //if(shoppingcart.Count==0)
                //{
                //    TempData["BuyFailed"] = "Alışveriş sepetinizde ürün bulunmamaktadır.";
                //    return RedirectToAction("Index", "Home");
                //}


                //Artık alışveriş tamamlansın
                Order customerOrder = new Order()
                {
                    CustomerTcNumber = customer.TCNumber,
                    IsDeleted = false,
                    OrderNumber = customer.TCNumber  //Burayı düzelteceğiz
                };

                //insert yapılsın.
                int orderInsertResult = myOrderRepo.Insert(customerOrder);
                if (orderInsertResult > 0)
                {
                    //siparişin detayları orderdetaile eklenmeli.
                    int orderDetailsInsertResult = 0;
                    foreach (var item in shoppingcart)
                    {
                        OrderDetail customerOrderDetail = new OrderDetail()
                        {
                            OrderId = customerOrder.Id,
                            IsDeleted = false,
                            ProductId = item.Id,
                            ProductPrice = item.Price,
                            Quantity = item.Quantity,
                            Discount = item.Discount

                        };
                        //TotalCount hesabı
                        if (item.Discount > 0)
                        {
                            customerOrderDetail.TotalPrice = customerOrderDetail.Quantity * (customerOrderDetail.ProductPrice - (customerOrderDetail.ProductPrice * (decimal)customerOrderDetail.Discount / 100));
                        }
                        else
                        {
                            // 3* telefonun fiyatı
                            customerOrderDetail.TotalPrice = customerOrderDetail.Quantity * customerOrderDetail.ProductPrice;
                        }
                        //orderdetail tabloya insert edilsin
                        orderDetailsInsertResult += myOrderDetailRepo.Insert(customerOrderDetail);

                    }

                    //orderDetailsInsertResult büyükse sıfırdan 

                    if (orderDetailsInsertResult > 0 && orderDetailsInsertResult == shoppingcart.Count)
                    {
                        //QR kodu eklenmiş email gönderilecek

                        #region SendOrderEmailWithQR

                        string siteUrl =
                                Request.Url.Scheme + Uri.SchemeDelimiter
                               + Request.Url.Host
                            + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                        siteUrl += "/Home/Order/" + customerOrder.Id;

                        QRCodeGenerator QRGenerator = new QRCodeGenerator();
                        QRCodeData QRData = QRGenerator.CreateQrCode(siteUrl, QRCodeGenerator.ECCLevel.Q);
                        QRCode QRCode = new QRCode(QRData);
                        Bitmap QRBitmap = QRCode.GetGraphic(60);
                        byte[] bitmapArray = BitmapToByArray(QRBitmap);

                        List<OrderDetail> orderDetailList =
                        new List<OrderDetail>();
                        orderDetailList = myOrderDetailRepo.AsQueryable()
                        .Where(x => x.OrderId == customerOrder.Id).ToList();

                        string message = $"Merhaba {user.Name} {user.Surname} <br/><br/>" +
                        $"{orderDetailList.Sum(x=> x.Quantity)} adet ürünlerinizin siparişini aldık.<br/><br/>" +
                        $"Toplam Tutar:{orderDetailList.Sum(x => x.TotalPrice).ToString()} ₺ <br/> <br/>" +
                        $"<table><tr><th>Ürün Adı</th><th>Adet</th><th>Birim Fiyat</th><th>Toplam</th></tr>";
                        foreach (var item in orderDetailList)
                        {
                            message += $"<tr><td>{myProductRepo.GetById(item.ProductId).ProductName}</td><td>{item.Quantity}</td><td>{item.TotalPrice}</td></tr>";
                        }

                        message += "</table><br/>Siparişinize ait QR kodunuz aşağıdadır. <br/><br/>";

                        SiteSettings.SendMail(bitmapArray, new MailModel()
                        {
                            To = user.Email,
                            Subject = "ECommerceLite - Siparişiniz alındı.",
                            Message = message
                        });

                        #endregion

                        TempData["BuySuccess"] = "Siparişiniz oluşturuldu. Sipariş Numarası: " + customerOrder.OrderNumber;

                        //temizlik
                        Session["ShoppingCart"] = null;
                        Logger.LogMessage($"{user.Name} {user.Surname} {orderDetailList.Sum(x => x.TotalPrice)} liralık alışveriş yaptı.", "Home/Buy");

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //Sistem yöneticisine orderId detayı verilerek email gönderilsin.
                        //Eklenmeyen ürünleri acilen eklesinler.

                        var message = $"Merhaba Admin, <br/>" +
                            $"Aşağıdaki bilgileri verilen siparişin kendisi oluşturulmasına rağmen detaylarından bazıları oluşturulmadı." +
                            $"Acilen müdahale edelim. <br/> <br/>" +
                            $"OrderId:{customerOrder.Id} <br/>" +
                            $"Sipariş detayları <br/>";

                        for (int i = 1; i <= shoppingcart.Count; i++)
                        {
                            message += $"{i}- Id: {shoppingcart[i].Id}-" +
                                $"Birim Fiyat: {shoppingcart[i].Price}-" +
                                 $"Sipariş adedi: {shoppingcart[i].Quantity}-" +
                                  $"İndirimi: {shoppingcart[i].Discount}-" +
                                  $"<br/><br/>";

                        }
                        await SiteSettings.SendMail(new MailModel()
                        {

                            To = "303asms@gmail.com",
                            Subject = "ACİL - ECommerceLite 303 Sipariş Detay Sorunu",
                            Message = message

                        });
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // ex loglanacak.

                Logger.LogMessage($"Satın alma işlemine hata: " + ex.ToString(), "Home/Buy", MembershipTools.GetUser().Id);

                TempData["BuyFailed"] = "Beklenmedik bir hata nedeniyle siparişiniz oluşturulamadı";
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ProductDetail(int? id)
        {
            try
            {
                if(id==null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if(id>0)
                {
                    //ürünü bulacağız
                    ProductViewModel model = myProductRepo.GetById(id.Value).Adapt<ProductViewModel>();

                    model.GetCategory();
                    model.GetProductPictures();
                    return View(model);


                }
                else
                {

                ModelState.AddModelError("", "Ürün bulunamadı!");

                return View(new ProductViewModel());
                }

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu!");

                return View(new ProductViewModel());
            }
        }




    }
}