using ETS.Application.Authentication.Commands.ChangePassword;
using ETS.Application.Authentication.Commands.Login;
using ETS.Application.Authentication.Queries.GetProfileDetails;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.Domain.Models;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]    
    public class AuthenticationController : AuthControllerBase<AuthenticationController>
    {
        public AuthenticationController(ILogger<AuthenticationController> 
            logger, 
            IConfiguration config, 
            IUserContext userService,
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand(request.Username, request.Password, request.Platform);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("profile-details")]
        [ProducesResponseType(typeof(TokenUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProfileDetails()
        {
            var query = new GetProfileDetailsQuery();
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }


        [HttpPost("change-password")]        
        [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            var command = new ChangePasswordCommand(_userService.UserEmail!,
                request.CurrentPassword, 
                request.NewPassword, 
                request.ConfirmNewPassword);

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
