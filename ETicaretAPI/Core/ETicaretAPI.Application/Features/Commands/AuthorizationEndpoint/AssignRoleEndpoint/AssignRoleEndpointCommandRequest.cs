using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint
{     //bu roller yoksa ata onceden roller varsa onları at bunları ata , hiç rol yoksa bunları ata dmek için bu propertyleri yazdık
    public class AssignRoleEndpointCommandRequest:IRequest<AssignRoleEndpointCommandResponse>
    {
        public string[] Roles { get; set; }
        public string EndpointCode { get; set; }  //hangi endpointlere karşılık bu roller gelecek onun içim
        public string Menu { get; set; }
        public Type? Type { get; set; }
    }
}
