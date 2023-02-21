using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using Y = ETicaretAPI.Application.DTOs;
using System.Text;
using System.Threading.Tasks;
using X = ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly IAuthService _authService;
        readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(IAuthService authService, ILogger<LoginUserCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
           var token =await _authService.LoginAsync(request.UserNameOrEmail, request.Password,15);
            _logger.LogInformation("Giriş işlemi yapıldı...");
            return new LoginUserCommandResponse()
            {
                Token = token
            };
        }
    }
}
