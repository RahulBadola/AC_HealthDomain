using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;


namespace time_track_service.Utils
{
    public static class ApplicationExceptionHandler
    {
        public static void ConfigureExceptionHandler(IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var errorId = Guid.NewGuid();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"500 errorId={errorId.ToString()} Error: {contextFeature.Error}");

                        var errorResponse = new
                        {
                            statusCode = context.Response.StatusCode,
                            message = "Internal Server Error.",
                            errorId = errorId.ToString()
                        };

                        var jsonText = JsonSerializer.Serialize(errorResponse);
                        await context.Response.WriteAsync(jsonText);
                    }
                });
            });
        }
    }
}

