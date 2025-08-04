using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using time_track_service.Model;

namespace time_track_service.Controllers
{
    internal static class ControllerExtensions
    {
        public static IActionResult HandleDbResponse(this ControllerBase controller, DbResponse dbResponse)
        {
            switch (dbResponse)
            {
                case DbResponse.Conflict:
                    return controller.Conflict();
                case DbResponse.Deleted:
                case DbResponse.Error:
                case DbResponse.Reverted:
                    return controller.StatusCode(StatusCodes.Status500InternalServerError);
                case DbResponse.Forbidden:
                    return controller.Forbid();
                case DbResponse.Invalid:
                    return controller.BadRequest();
                case DbResponse.NotFound:
                    return controller.NotFound();
                default:
                    return controller.Ok();
            }
        }

        public static ActionResult<TData> HandleDbResponse<TData>(this ControllerBase controller, DbResponse dbResponse, TData data)
        {
            switch (dbResponse)
            {
                case DbResponse.Conflict:
                    return controller.Conflict();
                case DbResponse.Deleted:
                case DbResponse.Error:
                case DbResponse.Reverted:
                    return controller.StatusCode(StatusCodes.Status500InternalServerError);
                case DbResponse.Forbidden:
                    return controller.Forbid();
                case DbResponse.Invalid:
                    return controller.BadRequest();
                case DbResponse.NotFound:
                    return controller.NotFound();
                default:
                    return controller.Ok(data);
            }
        }
    }
}
