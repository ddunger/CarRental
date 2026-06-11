using CarRental.Application.Common.Results;
using CarRental.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.Ok(new { data = result.Value });

            var error = new { message = result.Error };

            return result.ErrorType switch
            {
                ResultError.NotFound => controller.NotFound(error),
                ResultError.Unauthorized => controller.Unauthorized(error),
                ResultError.Conflict => controller.Conflict(error),
                ResultError.Validation => controller.BadRequest(error),
                ResultError.Forbidden => controller.StatusCode(403, error),
                _ => controller.StatusCode(500, error)
            };
        }

        //Delete actions have nothing to return, so no <T> is required resulgin in 204
        public static IActionResult ToActionResult(this Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.NoContent();

            var error = new { message = result.Error };

            return result.ErrorType switch
            {
                ResultError.NotFound => controller.NotFound(error),
                ResultError.Unauthorized => controller.Unauthorized(error),
                ResultError.Conflict => controller.Conflict(error),
                ResultError.Validation => controller.BadRequest(error),
                ResultError.Forbidden => controller.StatusCode(403, error),
                _ => controller.StatusCode(500, error)
            };
        }
    }
}