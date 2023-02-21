using ETicaretAPI.Application.Conts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using ETicaretAPI.Application.Features.Queries.AppUser.GetRolesToUser;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TextKeyAPI.Application.Features.Commands.AppUser.UpdatePassword;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = ("Admin"))]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeDefinition(Menu =AuthorizeDefinitionConstant.Users,ActionType = ActionType.Writing, Definition = "CreateUser ")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommandRequest createUserCommandRequest)
        {
            CreateUserCommandResponse response = await _mediator.Send(createUserCommandRequest);
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Users, ActionType = ActionType.Reading, Definition = "Get All Users")]
        public async Task<IActionResult> GetAllUser([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }

        [HttpPost("assign-role-user")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Users, ActionType = ActionType.Writing, Definition = "Assign Role To User")]
        public async Task<IActionResult> AssignRoleUser(AssignRoleToUserCommandRequest assignRoleUserCommandRequest)
        {
            AssignRoleToUserCommandResponse response = await _mediator.Send(assignRoleUserCommandRequest);
            return Ok(response);
        }

        [HttpGet("get-roles-to-user/{UserId}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Users, ActionType = ActionType.Reading, Definition = "Get Role To User")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest getRolesToUserQueryRequest)
        {
            GetRolesToUserQueryResponse response = await _mediator.Send(getRolesToUserQueryRequest);
            return Ok(response);
        }

        [HttpPost("UpdatePassword")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstant.Users, ActionType = ActionType.Reading, Definition = "Get Role To User")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            UpdatePasswordCommandResponse response = await _mediator.Send(updatePasswordCommandRequest);
            return Ok(response);
        }
    }
}
