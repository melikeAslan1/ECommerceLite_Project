using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    public class PersonBase : IPersonBase
    {
        [Key]
        [Column(Order=1)]
        [MinLength(11)]
        [StringLength(11, ErrorMessage ="Tc kimlik numarası 11 haneli olmalıdır!")]
        [Display(Name ="TC Kimlik Numarası")]
        public string TCNumber { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastActiveTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
