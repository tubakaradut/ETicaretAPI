using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommandRequest, AssignRoleToUserCommandResponse>
    {
        readonly IUserService _userService;
        readonly ILogger<AssignRoleToUserCommandHandler> _logger;

        public AssignRoleToUserCommandHandler(IUserService userService, ILogger<AssignRoleToUserCommandHandler> logger = null)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<AssignRoleToUserCommandResponse> Handle(AssignRoleToUserCommandRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rol atama işlemi yapıldı...");
            await _userService.AssignRoleToUser(request.Roles,request.UserId);

            return new();
        }
    }
}
