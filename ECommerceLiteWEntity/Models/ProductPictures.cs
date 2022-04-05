using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    [Table("ProductPictures")]
    public class ProductPicture : Base<int>
    {
        public int ProductId { get; set; }

        [StringLength(400, ErrorMessage = "Ürün resim yolu en fazla 400 karakter olmalıdır!")]
        public string ProductPicture1 { get; set; }
        [StringLength(400, ErrorMessage = "Ürün resim yolu en fazla 400 karakter olmalıdır!")]
        public string ProductPicture2 { get; set; }

        [StringLength(400, ErrorMessage = "Ürün resim yolu en fazla 400 karakter olmalıdır!")]
        public string ProductPicture3 { get; set; }
        [StringLength(400, ErrorMessage = "Ürün resim yolu en fazla 400 karakter olmalıdır!")]
        public string ProductPicture4 { get; set; }
        [StringLength(400, ErrorMessage = "Ürün resim yolu en fazla 400 karakter olmalıdır!")]
        public string ProductPicture5 { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }

}
