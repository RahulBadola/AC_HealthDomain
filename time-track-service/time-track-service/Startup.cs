using AssureCare.MedCompass.DataAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using Newtonsoft.Json.Serialization;
using time_track_service.Context;
using time_track_service.Data;
using time_track_service.Extensions;
using time_track_service.Model;
using time_track_service.Model.Dto;
using time_track_service.Services;
using time_track_service.Utils;
using member_allergy_service.Data;

namespace time_track_service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson( options => options.SerializerSettings.ContractResolver = new DefaultContractResolver() )
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
                    Title = "time-track-service API",
                    Description = "time-track-service API",
                    Contact  = new OpenApiContact { Name = "Not Me", Email = "unknown@assurecare.com", Url = new Uri("https://www.assurecare.com") }
                });                
            });

            AssureCareBindings.AddRequestContext(services);
            AssureCareBindings.AddResilientHttpClientPolicies(services, Configuration);

            // register and configure DataAuthorization services
            services.AddMedCompassDataAuthorization(Configuration, (options) =>
            {
                // specify object models that will require Field or Operation authorization
                options.Objects = new[]
                {
                    "TimeTrack"
                };
            });

            var mongoClientConnectionSettings = new MongoClientConnectionSettings();
            var serviceSettings = new ServicesSettings();
            Configuration.Bind("MongoClientConnectionSettings", mongoClientConnectionSettings);
            Configuration.Bind("Services", serviceSettings);
            services.AddSingleton(mongoClientConnectionSettings);
            services.AddSingleton(serviceSettings);

            services
				.AddHealthChecks()
				.AddMongoDb(mongoClientConnectionSettings.GenerateSettings(), mongoClientConnectionSettings.Database, "MongoDb", HealthStatus.Unhealthy, timeout: new TimeSpan(0, 0, 5), tags: new[] { "startup" });

            // Local Services
            services.AddSingleton<ISharedService,SharedService>();
            services.AddSingleton<IDomainService, DomainService>();
            services.AddSingleton<IHydrationService, HydrationService>();
            services.AddSingleton<ITestRepository, TestRepository>();
            services.AddSingleton<ITimeTrackRepository, TimeTrackRepository>();
            services.AddSingleton<ILoadTestDataService, LoadTestDataService>();
            services.AddSingleton<IStaffTimeTrackingService, StaffTimeTrackingService>();
            services.AddSingleton<IDBContext,DBContext>();
            services.AddSingleton<IMemberTimeTrackingService, MemberTimeTrackingService>();
            services.AddSingleton<ISyncService, SyncService>();
            services.AddSingleton<IGenericRepository, GenericRepository>();
            services.AddHttpClient<IOtherServices, OtherServices>()
                .AddPolicyHandlerFromRegistry(RetryHelper.RetryPolicyName);
            services.AddHttpClient<ISyncBackNotificationService, SyncBackNotificationService>()
                .AddPolicyHandlerFromRegistry(RetryHelper.RetryPolicyName);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app            
            ,IWebHostEnvironment env
            ,ILogger<Startup> log
            ,IConfiguration configuration
            ,IDBContext dBContext
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
                c.SwaggerEndpoint($"{contextRoot}/swagger/v1/swagger.json", "time-track-service Service V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                 endpoints.MapLocalHealthChecks(configuration);
            });

            dBContext.Initialize();
        } 
    }
}
