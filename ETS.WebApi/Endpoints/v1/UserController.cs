using ETS.Domain.Contracts;
using ETS.WebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController(ILogger<UserController> logger,
        IConfiguration config,
        IUserContext userService) : AuthControllerBase<UserController>(logger, config, userService)
    {

        public async Task<IActionResult> CreateUser(AddUserRequest request)
        {

        }
    }
}
