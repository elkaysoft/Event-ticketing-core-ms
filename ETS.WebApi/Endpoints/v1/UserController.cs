using ETS.Application.Authentication.Commands.Login;
using ETS.Application.Users.Commands.RegisterUser;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]    
    public class UserController : AuthControllerBase<UserController>
    {
        public UserController(ILogger<UserController> 
            logger, 
            IConfiguration config,
            IUserContext userService,
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(AddUserRequest request)
        {
            var command = new RegisterAdminUserCommand(request.FullName,
                request.EmailAddress
                , request.PhoneNumber!
                , request.Role);

            var result = await _mediator.Send(command);
            if (result.IsSuccess)            
                return Ok(result);
            return BadRequest(result);            
        }
    }
}
