﻿using ECommerceLiteBLL.Repository;
using ECommerceLiteEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ECommerceLiteUI.Models
{
    public class ProductViewModel
    {

        CategoryRepo myCategoryRepo = new CategoryRepo();

        ProductPictureRepo myProductPictureRepo = new ProductPictureRepo();

        public int Id { get; set; }

        public DateTime RegisterDate { get; set; }


        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "Ürün adı 2 ile 100 karakter aralığında olmalıdır!")]
        [Display(Name = "Ürün Adı")]
        public string ProductName { get; set; }


        [Required]
        [StringLength(maximumLength: 500, ErrorMessage = "Ürün açıklaması en fazla 500 karakter olmalıdır!")]
        [Display(Name = "Ürün Açıklaması")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Ürün Kodu")]
        [StringLength(maximumLength: 8, ErrorMessage = "Ürün kodu en fazla 8 karakter olmalıdır!")]
        [Index(IsUnique = true)] // benzersiz tekrarsız olmasını sağlar.
        public string ProductCode { get; set; }


        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public double Discount { get; set; }

        // her ürünün bir kategorisi olur. ilişki kurduk.
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<ProductPicture> ProductPictureList { get; set; }

        //Ürün eklenirken ürüne ait resimler secilebilir. Secilen Resimleri hafızada tutacak property.
        public List<HttpPostedFileBase> File { get; set; } = new List<HttpPostedFileBase>();

        public void GetProductPicture()
        {
            if(Id> 0)
            {
                ProductPictureList = myProductPictureRepo.AsQueryable().Where(x => x.ProductId == Id).ToList();
   
            }

            //Product p = new Product();
            //ProductViewModel m = new ProductViewModel()
            //{
            //    ProductName = p.ProductName(),

            //}

            
        }





    }
}
