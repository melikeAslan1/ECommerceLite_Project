using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("Order Details")]
    public class OrderDetail: Base<int>
    {
        //İlişkiler

        //Her ürün detayı bir siparişe bağlıdır.

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        // her sipariş detayının bir ürünü olmalıdır.

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }



        //özellikler

        [Required]
        
        public int Quantity { get; set; }


        [Required]
        [DataType(DataType.Currency)]
        public decimal ProductPrice { get; set; }

        public double Discount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}
