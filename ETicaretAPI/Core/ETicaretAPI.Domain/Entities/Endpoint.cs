using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identiy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{  //actionlardan bahsediyoruz endpoint diterek
    public class Endpoint:BaseEntity
    {  //HashSet içerisinde eklenen eleman tekrarlayamaz, Contains fonksiyonu mevcut. Geri dönüsü bool.,Count property'si ile toplam eleman sayisini ögrenebiliriz.
        public Endpoint()
        {
            AppRoles=new HashSet<AppRole>();
        }
        public  Menuu Menuu { get; set; }
        public string ActionType { get; set; }
        public string  HttpType { get; set; }
        public string  Definition { get; set; }
        public string  EndpointCode { get; set; }

        public ICollection<AppRole> AppRoles { get; set; }

    }
}
