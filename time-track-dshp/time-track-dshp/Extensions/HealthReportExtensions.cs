using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace time_track_dshp.Extensions
{
    public static class HealthReportExtensions
    {
        public static IEndpointConventionBuilder MapLocalHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions()
            {
                Predicate = check => false,
                ResultStatusCodes = ResultStatusCodes,
                ResponseWriter = WriteHealthCheckResponse
            });

            endpoints.MapHealthChecks("/api/health/all", new HealthCheckOptions()
            {
                ResultStatusCodes = ResultStatusCodes,
                ResponseWriter = WriteHealthCheckResponse
            });

            endpoints.MapHealthChecks($"/api/health/{HealthCheckTags.Live}", new HealthCheckOptions()
            {
                Predicate = check => check.Tags.Contains(HealthCheckTags.Live),
                ResultStatusCodes = ResultStatusCodes,
                ResponseWriter = WriteHealthCheckResponse
            });

            endpoints.MapHealthChecks($"/api/health/{HealthCheckTags.Ready}", new HealthCheckOptions()
            {
                Predicate = check => check.Tags.Contains(HealthCheckTags.Ready),
                ResultStatusCodes = ResultStatusCodes,
                ResponseWriter = WriteHealthCheckResponse
            });

            var returnValue = endpoints.MapHealthChecks($"/api/health/{HealthCheckTags.Startup}", new HealthCheckOptions()
            {
                Predicate = check => check.Tags.Contains(HealthCheckTags.Startup),
                ResultStatusCodes = ResultStatusCodes,
                ResponseWriter = WriteHealthCheckResponse
            });

            return returnValue;
        }

        private static readonly IDictionary<HealthStatus, int> ResultStatusCodes = new Dictionary<HealthStatus, int>()
        {
            [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        };

        private static string BuildResponse(this HealthReport report, IConfiguration configuration)
        {
            string returnValue;

            var contextRoot = configuration.GetValue("ContextRoot", "");
            var serviceName = configuration.GetValue("ServiceName", "Unknown");
            var myNamespace = configuration.GetValue("Namespace", "Local");
            var appVersion = Assembly.GetEntryAssembly()?.GetName().Version!.ToString();
            var templateVersion = configuration.GetValue("TemplateVersion", "0.0.0");
            var medCompassRelease = configuration.GetValue("MedCompassRelease", "");

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();

                    writer.WriteStartObject("ServiceDetails");
                    writer.WriteString("ContextRoot", contextRoot);
                    writer.WriteString("ServiceName", serviceName);
                    writer.WriteString("Namespace", myNamespace);
                    writer.WriteString("AppVersion", appVersion);
                    writer.WriteString("TemplateVersion", templateVersion);
                    writer.WriteString("MedCompassRelease", medCompassRelease);
                    writer.WriteEndObject();

                    writer.WriteStartObject("Health");
                    writer.WriteString("OverallStatus", report.Status.ToString());
                    writer.WriteString("TotalDuration", report.TotalDuration.TotalSeconds.ToString("0:0.00"));

                    if (report.Entries.Any())
                    {
                        writer.WriteStartObject("DependencyHealthChecks");
                        foreach (var item in report.Entries)
                        {
                            writer.WriteStartObject(item.Key);
                            writer.WriteString("Status", item.Value.Status.ToString());
                            writer.WriteString("Duration", item.Value.Duration.TotalSeconds.ToString("0:0.00"));
                            writer.WriteString("Exception", item.Value.Exception != null ? item.Value.Exception.ToString() : "");
                            writer.WriteEndObject();
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();

                    writer.WriteEndObject();
                }

                returnValue = Encoding.UTF8.GetString(stream.ToArray());
            }

            return returnValue;

        }

        private static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            var configuration = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            return httpContext.Response.WriteAsync(result.BuildResponse(configuration));
        }
    }

    internal static class HealthCheckTags
    {
        public static readonly string Live = nameof(Live).ToLower();
        public static readonly string Ready = nameof(Ready).ToLower();
        public static readonly string Startup = nameof(Startup).ToLower();
    }
}
