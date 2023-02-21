using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using TextKeyAPI.Application.Features.Commands.AppUser.UpdatePassword;

namespace ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommandRequest, UpdatePasswordCommandResponse>
    {
        readonly IUserService _userService;
        readonly ILogger<UpdatePasswordCommandHandler> _logger;

        public UpdatePasswordCommandHandler(IUserService userService, ILogger<UpdatePasswordCommandHandler> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<UpdatePasswordCommandResponse> Handle(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            if (!request.Password.Equals(request.ConfirmPassword)) 
            {
                throw new PasswordChangeFailedException("Lütfen doğrulam işlemi hatalı!!!!");  
            }
                await _userService.UpdatePasswordAsync(request.UserId,request.ResetToken, request.Password);
            _logger.LogInformation($"{ request.UserId} şifresini güncellendi..."  );
            return new();
        }
    }
}
