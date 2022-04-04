using ECommerceLiteWEntity.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteWEntity.Models
{
     public class Customer : PersonBase
    {
        // Identity Model ile bize verilen tablodaki Id buraya foreignKey olacaktır.
        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual List<Order> Orders { get; set; }



    }
    
}
