using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{   //controllerden bahsediyoruz menü diyerek
    public class Menuu:BaseEntity
    {
        public string MenuName { get; set; }

        public ICollection<Endpoint> Endpoints { get; set; }
    }
}
