using ETicaretAPI.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.CustomAttributes
{
    public class AuthorizeDefinitionAttribute:Attribute
    {
        //endpointler için oluşturuldu menü dediğimiz yapı controllor ve action 
        public string Menu { get; set; }
        public string Definition { get; set; }
        public ActionType ActionType { get; set; }

    }
}
