using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUser.RefreshTokenLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TextKeyAPI.Application.Features.Commands.AppUser.PasswordReset;
using TextKeyAPI.Application.Features.Commands.AppUser.VerifyResetToken;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest loginUserCommandRequest)
        {
            LoginUserCommandResponse response = await _mediator.Send(loginUserCommandRequest);


            return Ok(response);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> RefreshToken([FromForm] RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
        {
           RefreshTokenLoginCommandResponse response=await _mediator.Send(refreshTokenLoginCommandRequest);
            return Ok(response);
        }



        [HttpPost("google-login")] //Google üzerinden login olabilmek için oluşturuldu.
        public async Task<IActionResult> GoogleLogin(GoogleLoginCommandRequest googleLoginCommandRequest)
        {
            GoogleLoginCommandResponse response = await _mediator.Send(googleLoginCommandRequest);

            return Ok(response);
        }

        [HttpPost("PasswordReset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommandRequest passwordResetCommandRequest)
        {
            PasswordResetCommandResponse response = await _mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }

        [HttpPost("VerifyResetToken")]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenCommandRequest verifyResetTokenCommandRequest)
        {
            VerifyResetTokenCommandResponse response = await _mediator.Send(verifyResetTokenCommandRequest);
            return Ok(response);
        }



    }
}
