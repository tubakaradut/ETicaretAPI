using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identiy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class Basket : BaseEntity
    {

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; }


        public Order Order { get; set; }
    }
}
