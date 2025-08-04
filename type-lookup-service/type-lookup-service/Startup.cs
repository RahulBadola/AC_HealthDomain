using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using type_lookup_service.Context;
using type_lookup_service.Extensions;
using type_lookup_service.Model;
using type_lookup_service.Utils;

namespace type_lookup_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHealthChecks();

            services.AddControllers()
                .AddNewtonsoftJson(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });


            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "type-lookup-service API",
                    Description = "type-lookup-service API",
                    Contact = new OpenApiContact { Name = "Not Me", Email = "unknown@assurecare.com", Url = new Uri("https://www.assurecare.com") }
                });

                if (Configuration.GetValue("TypeLookup:IsLocalDebug", false))
                {
                    c.OperationFilter<HeaderFilter>();
                }
            });


            services.AddHealthChecks();
            services.AddMemoryCache();
            services.AddRequestContext();
            services.AddResilientHttpClientPolicies(Configuration);

            // Configuration
            var mongoClientConnectionSettings = new MongoClientConnectionSettings();
            Configuration.Bind("MongoClientConnectionSettings", mongoClientConnectionSettings);
            services.AddSingleton(mongoClientConnectionSettings);

            services.AddLocalServices();
        }

        public void Configure(IApplicationBuilder app
            , IWebHostEnvironment env
            , ILogger<Startup> log
            , IConfiguration configuration
            , IDbContext dbContext
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ApplicationExceptionHandler.ConfigureExceptionHandler(app, log);

            var contextRoot = configuration.GetValue<string>("ContextRoot", "");
            log.LogInformation($"Context Root set : {contextRoot}");

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer {  Url = $"http://{httpReq.Host.Value}{contextRoot}" },
                        new OpenApiServer {  Url = $"https://{httpReq.Host.Value}{contextRoot}" }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{contextRoot}/swagger/v1/swagger.json", "type-lookup-service Service V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/api/health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = WriteHealthCheckResponse
                });
            });

            dbContext.Initialize();
        }

        private Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(result.BuildResponse(Configuration));
        }
    }
}
