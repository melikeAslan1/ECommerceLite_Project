using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;
using ECommerceLiteBLL.Settings;
using ECommerceLiteEntity.Models;
using ECommerceLiteUI.Models;
using Mapster;

namespace ECommerceLiteUI.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductRepo myProductRepo = new ProductRepo();
        ProductPictureRepo myProductPictureRepo = new ProductPictureRepo();
        private const int pageSize = 5;

        //Bu controllera Admin gibi yetkili kişiler erişecektir
        // Burada ürünlerin listelenmesi, ekleme, silme, güncelleme işlemleri yapılacaktır.
        public ActionResult ProductList(int? page = 1, string search = "")
        {
            //Alt Kategorileri repo aracılığıyla dbden çektik
            ViewBag.SubCategories = myCategoryRepo
                     .AsQueryable().Where(x => x.BaseCategoryId != null).ToList();

            // Sayfaya bazı bilgiler göndereceğiz
            var totalProduct = myProductRepo.GetAll().Count; // toplam ürün sayısı
            ViewBag.TotalProduct = totalProduct; // toplam ürün sayısını sayfaya gönderceğiz
            ViewBag.TotalPages = (int)Math.Ceiling(totalProduct / (double)pageSize);

            // Toplamürün/sayfada gösterilecek üründen kaç sayfa olduğu bilgisi
            ViewBag.PageSize = pageSize; // Her sayfada kaç ürün gözükecek bilgisini html sayfasına gönderelim

            //frenleme 
            //1. yöntem
            if (page < 1)
            {
                page = 1;
            }
            if (page > ViewBag.TotalPages)
            {
                page = ViewBag.TotalPages;
            }

            //2. yöntem
            page = page < 1 ? // eğer page 1'den küçükse
                          1 : // page'in değeirini 1 yap 
                          page > ViewBag.TotalPages ?// değilse bak bakalım page toplam sayfadan büyük mü
                                                    ViewBag.TotalPages // page değeri= toplamsayfa
                                                    :
                                                    page; // page'e dokunma page aynı değerinden devamke


            ViewBag.CurrentPage = page; // View'de kaçıncı sayfada olduğum bilgisini tutsun

            List<Product> allProducts = new List<Product>();
            //var allProducts = myProductRepo.GetAll();
            if (string.IsNullOrEmpty(search))
            {
                allProducts = myProductRepo.GetAll();
            }
            else
            {
                allProducts = myProductRepo.GetAll()
                    .Where(x => x.ProductName.ToLower().Contains(search.ToLower())
                    || x.Description.ToLower().Contains(search.ToLower())).ToList();
            }

            //Paging--> 1. yöntem bu yöntem en klasik yöntemdir.
            allProducts = allProducts.Skip(
                (page.Value < 1 ? 1 : page.Value - 1) // 
                * pageSize
                )
                .Take(pageSize) // 10 take al neden 10? Çünkü yukarıdaki pageSize 10'a eşitlenmiş
                .ToList();


            return View(allProducts);
        }


        [HttpGet]
        public ActionResult Create()
        {
            //Sayfayı çağırırken ürünün kategorisinin ne olduğu seçmesi lazım
            //Bu nedenle sayfaya kategoriler gitmeli
            List<SelectListItem> subCategories = new List<SelectListItem>();
            //Linq
            //select * from Categories where BaseCategoryId is not null

            myCategoryRepo.AsQueryable().Where(x => x.BaseCategoryId != null)
                .ToList().ForEach(x => subCategories.Add(
                    new SelectListItem()
                    {
                        Text = x.CategoryName,
                        Value = x.Id.ToString()
                    }));

            ViewBag.SubCategories = subCategories;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {
                List<SelectListItem> subCategories = new List<SelectListItem>();
                myCategoryRepo.AsQueryable().Where(x => x.BaseCategoryId != null)
                    .ToList().ForEach(x => subCategories.Add(
                        new SelectListItem()
                        {
                            Text = x.CategoryName,
                            Value = x.Id.ToString()
                        }));

                ViewBag.SubCategories = subCategories;

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Veri girişleri düzgün olmalıdır");
                    return View(model);
                }

                if (model.CategoryId <= 0)
                {
                    ModelState.AddModelError("", "Ürüne ait kategori seçilmelidir!");
                    return View(model);
                }

                // Burada kontrol lazım
                // Acaba girdiği ürün kodu bizim db zaten var mı?

                if (myProductRepo.IsSameProductCode(model.ProductCode))
                {
                    ModelState.AddModelError("", "Dikkat! Girdiğiniz ürün kodu sistemdeki bir başka ürüne aittir. Ürün kodları benzersiz olmalıdır");
                    return View(model);
                }

                //Ürün tabloya kayıt olacak.
                //TODO: Mapleme yapılacak

                Product product = new Product()
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    ProductCode = model.ProductCode,
                    CategoryId = model.CategoryId,
                    Discount = model.Discount,
                    Quantity = model.Quantity,
                    RegisterDate = DateTime.Now,
                    Price = model.Price
                };

                //mapleme yapıldı.
                //Mapster paketi indirildi. Mapster bir objedeki verilri diğer bir objeye zahmetsizce aktarır.
                //Aktarım yapabilmesi için A objesiyle B objesinin içindeki propertylerin 
                //isimleri ve tipleri birebir aynı olmalıdır.
                //Bu projede Mapster kullandık
                //Core projesinde daha profesyonel olan AutoMapper'ı kullanacağız
                // Bir dto objesinin içindeki verileri alır asıl objenin içine aktarır. Asıl objenin verilerini dto objesinin içindeki propertylere aktarır.
                //Product product = model.Adapt<Product>();
                //Product product2 = model.Adapt<ProductViewModel,Product>();

                int insertResult = myProductRepo.Insert(product);
                if (insertResult > 0)
                {
                    // Sıfırdan büyükse product tabloya eklendi
                    // Acaba bu producta resim seçmiş mi? resim seçtiyse o
                    // resimlerin yollarını kayıt et
                    if (model.Files.Any() && model.Files[0] != null)
                    {
                        int pictureinsertResult = 0;
                        foreach (var item in model.Files)
                        {
                            if (item != null && item.ContentType.Contains("image")
                                && item.ContentLength > 0)
                            {
                                string productName = SiteSettings.StringCharacterConverter(model.ProductName).ToLower().Replace("-", "");
                                string extensionName = Path.GetExtension(item.FileName);
                                //Klasör adresi : ProductPictures/iphone13/20202020
                                string directoryPath = Server.MapPath
                                    ($"~/ProductPictures/{productName}/{model.ProductCode}");
                                string guid = Guid.NewGuid().ToString().Replace("-", "");
                                //ProductPictures/iphone13/20202020/iphone13-guid.jpg
                                string filePath = Server.MapPath
                                    ($"~/ProductPictures/{productName}/{model.ProductCode}/" +
                                    $"{productName}-{guid}{extensionName}");
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                //resmi o klasöre kayıt edelim
                                item.SaveAs(filePath);
                                //İşlem bitti DB'ye kayıt olacak
                                ProductPicture picture = new ProductPicture()
                                {
                                    ProductId = product.Id,
                                    RegisterDate = DateTime.Now,
                                    Picture = $"/ProductPictures/{productName}/{model.ProductCode}/" +
                                    $"{productName}-{guid}{extensionName}",
                                    IsDeleted = false
                                };
                                pictureinsertResult += myProductPictureRepo.Insert(picture);
                            }

                        }
                        // pictureinsertResult kontrol edilecektir
                        if (pictureinsertResult > 0 && model.Files.Count == pictureinsertResult)
                        {
                            //bütün resimler eklenmiş
                            TempData["ProductInsertSuccess"] = "Yeni ürün eklenmiştir";
                            return RedirectToAction("ProductList", "Product");
                        }
                        else if (pictureinsertResult > 0 && model.Files.Count != pictureinsertResult)
                        {
                            //eksik eklemiş
                            TempData["ProductInsertWarning"] = "Yeni ürün eklendi ama resimlerden bazıları beklenmedik bir sorun yüzünden eklenemedi! Eklenilemeyen resimleri daha sonra tekrar ekleyiniz.";

                            return RedirectToAction("ProductList", "Product");
                        }
                        else
                        {
                            // ürünü ekledi ama resimlerini eklemedi
                            TempData["ProductInsertWarning"] = "Yeni ürün eklendi ama ürüne ait resimler eklenemedi. Resimleri daha sonra tekrar eklemeyi deneyiniz";
                            return RedirectToAction("ProductList", "Product");
                        }

                    }
                    else
                    {
                        TempData["ProductInsertSuccess"] = "Yeni ürün eklenmiştir";
                        return RedirectToAction("ProductList", "Product");
                    }
                }
                else
                {
                    ModelState.AddModelError("",
                        "HATA: Ürün ekleme işleminde bir hata oluştu! Tekrar deneyiniz!");
                    return View(model);
                }




            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmekdik bir hata oluştu");
                //ex loglanacak
                return View(model);
            }
        }

        public JsonResult GetProductDetails(int id)
        {
            try
            {
                var product = myProductRepo.GetById(id);
                if (product != null)
                {
                    //var data = product.Adapt<ProductViewModel>();
                    var data = new ProductViewModel()
                    {
                        Id = product.Id,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        ProductCode = product.ProductCode,
                        CategoryId = product.CategoryId,
                        Discount = product.Discount,
                        Quantity = product.Quantity,
                        RegisterDate = product.RegisterDate,
                        Price = product.Price

                    };
                    return Json(new { isSuccess = true, data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isSuccess = false });
                }

            }
            catch (Exception ex)
            {

                //ex loglansın
                return Json(new { isSuccess = false });
            }
        }

        public ActionResult Edit(ProductViewModel model)
        {
            try
            {
                var product = myProductRepo.GetById(model.Id);
                if (product != null)
                {
                    product.ProductName = model.ProductName;
                    product.Description = model.Description;
                    product.Discount = model.Discount;
                    product.Quantity = model.Quantity;
                    product.ProductCode = model.ProductCode;
                    product.Price = model.Price;
                    product.CategoryId = model.CategoryId;

                    int updateResult = myProductRepo.Update();
                    if (updateResult > 0)
                    {
                        TempData["EditSuccess"] = "Bilgiler başarıyla güncellenmiştir.";
                        return RedirectToAction("ProductList", "Product");
                    }
                    else
                    {
                        TempData["EditFailed"] = "Beklenmedik bir hata olduğu için ürüne bilgileri sisteme aktarılamadı!";
                        return RedirectToAction("ProductList", "Product");
                    }
                }
                else
                {
                    TempData["EditFailed"] = "Ürün bulunamadığı için bilgileri güncellenemedi!";
                    return RedirectToAction("ProductList", "Product");
                }
            }
            catch (Exception ex)
            {

                // ex loglanacak
                TempData["EditFailed"] = "Beklenmedik bir hata nedeniyle ürün bilgileri güncellenemedi!";
                return RedirectToAction("ProductList", "Product");
            }
        }
    }
}