using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("Products")]
    public class Product:Base<int>
    {
        [Required]
        [StringLength(maximumLength:100, MinimumLength =2, ErrorMessage ="Ürün adı 2 ile 100 karakter aralığında olmalıdır!")]
        [Display(Name ="Ürün Adı")]
        public string ProductName { get; set; }


        [Required]
        [StringLength(maximumLength: 500,  ErrorMessage = "Ürün açıklaması en fazla 500 karakter olmalıdır!")]
        [Display(Name = "Ürün Açıklaması")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Ürün Kodu")]
        [StringLength(maximumLength: 8, ErrorMessage = "Ürün kodu en fazla 8 karakter olmalıdır!")]
        [Index(IsUnique =true)] // benzersiz tekrarsız olmasını sağlar.
        public string ProductCode { get; set; }


        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public double Discount { get; set; }

        // her ürünün bir kategorisi olur.
        public int CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }  // ProductOfCategory de denebilir.

        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual List<ProductPicture> ProductPictures { get; set; }




    }
}
