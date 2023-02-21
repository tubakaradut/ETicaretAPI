using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System.Reflection.Metadata.Ecma335;

namespace ETicaretAPI.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQueryRequest, GetRolesQueryResponse>
    {
        readonly IRoleService _roleService;

        public GetRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetRolesQueryResponse> Handle(GetRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var (datas,count)=_roleService.GetAllRoles();
            return new()
            {
                Datas = datas,
                TotalCount=count
            };

        }
        
    }
}
