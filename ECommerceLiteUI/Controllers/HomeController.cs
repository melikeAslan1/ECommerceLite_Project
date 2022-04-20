using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteBLL.Repository;
using ECommerceLiteUI.Models;
using Mapster;

namespace ECommerceLiteUI.Controllers
{
    public class HomeController : Controller
    {
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductRepo myProductRepo = new ProductRepo();

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

                if (shoppingCart==null)
                {
                    shoppingCart = new List<ProductViewModel>();
                }
                if(id>0)
                {
                    var product = myProductRepo.GetById(id);
                    if(product==null)
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

                    if(shoppingCart.Count(x=> x.Id==productAddToCart.Id)>0)
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




    }
}