using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace time_track_service.Extensions
{
    public static class HealthReportExtensions
    {
        public static IEndpointConventionBuilder MapLocalHealthChecks(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {

            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions()
            {
                Predicate = check => !check.Tags.Contains("startup"),
                ResultStatusCodes =
                    {
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = (httpContext, healthReport) => WriteHealthCheckResponse(httpContext, healthReport, configuration)
            });

            var healthChecks = endpoints.MapHealthChecks("/api/startup", new HealthCheckOptions()
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = (httpContext, healthReport) => WriteHealthCheckResponse(httpContext, healthReport, configuration)
            });

            return healthChecks;
        }

        private static string BuildResponse(this HealthReport report, IConfiguration configuration)
        {

            string returnValue;

            var contextRoot = configuration.GetValue<string>("ContextRoot", "");
            var serviceName = configuration.GetValue<string>("ServiceName", "Unknown");
            var myNamespace = configuration.GetValue<string>("Namespace", "Local");
            var appVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            var templateVersion = configuration.GetValue<string>("TemplateVersion", "0.0.0");
            var medCompassRelease = configuration.GetValue<string>("MedCompassRelease", "");

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
        private static Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result, IConfiguration configuration)
        {
            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(result.BuildResponse(configuration));
        }
    }
}
