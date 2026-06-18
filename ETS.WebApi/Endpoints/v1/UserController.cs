using ETS.Application.Authentication.Commands.Login;
using ETS.Application.Events.Queries.Events;
using ETS.Application.Events.Queries.GetPagedEvents;
using ETS.Application.Events.Queries.GetSingleEvent;
using ETS.Application.Users.Commands.DeleteUser;
using ETS.Application.Users.Commands.RegisterUser;
using ETS.Application.Users.Commands.Update;
using ETS.Application.Users.Queries.Dto;
using ETS.Application.Users.Queries.GetPagedUsers;
using ETS.Application.Users.Queries.GetSingleUser;
using ETS.Domain.Common;
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
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(AddUserRequest request)
        {
            var command = new RegisterAdminUserCommand(request.FullName,
                request.EmailAddress,
                request.PhoneNumber!,
                request.Role);

            var result = await _mediator.Send(command);
            return result.ToActionResult();       
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<GetUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsers([FromQuery] GetPaginatedUserFilter filter)
        {
            var query = new GetPagedUsersQuery(
                filter.SearchText,
                filter.StartDate.HasValue ? DateOnly.FromDateTime(filter.StartDate.Value) : (DateOnly?)null,
                filter.EndDate.HasValue ? DateOnly.FromDateTime(filter.EndDate.Value) : (DateOnly?)null,
                filter.Role,
                filter.SortField,
                filter.IsAscending)
            { 
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet("{userId:long}")]
        [ProducesResponseType(typeof(GetUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(long userId)
        {
            var query = new GetSingleUserQuery(userId);
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }


        [HttpPut("{userId:long}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(long userId, UpdateUserRequest request)
        {
            var command = new UpdateUserCommand(userId,
                request.FullName,
                request.EmailAddress,
                request.Role);

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }


        [HttpDelete("{userId:long}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            var command = new DeleteUserCommand(userId);

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }



    }
}
