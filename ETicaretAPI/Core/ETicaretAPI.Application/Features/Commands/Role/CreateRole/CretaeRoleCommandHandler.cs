using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Role.CreateRole
{
    public class CretaeRoleCommandHandler : IRequestHandler<CretaeRoleCommandRequest, CreateRoleCommandResponse>
    {
        readonly IRoleService _roleService;

        public CretaeRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<CreateRoleCommandResponse> Handle(CretaeRoleCommandRequest request, CancellationToken cancellationToken)
        {
           var result=await _roleService.CreateRole(request.Name);

            return new()
            {
                Succeeded= result
            };

        }
    }
}
