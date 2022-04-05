using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteEntity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceLiteEntity.Models
{
    public class PassiveUser : PersonBase
    {
        // Identity Model ile bize verilen tablodaki Id buraya foreignKey olacaktır.
        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
    
}
