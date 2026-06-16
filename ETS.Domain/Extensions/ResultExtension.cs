using ETS.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETS.Domain.Extensions
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return new ObjectResult(result.Error)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        // Overload for non-generic Result
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return new ObjectResult(result.Error)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

    }
}
