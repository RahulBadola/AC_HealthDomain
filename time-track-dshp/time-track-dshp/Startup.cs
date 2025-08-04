using time_track_dshp.Data;
using time_track_dshp.Extensions;
using time_track_dshp.Models.Configuration;
using time_track_dshp.Services;
using time_track_dshp.Services.Hosted;
using time_track_dshp.Utils;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace time_track_dshp
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {   
                    Version = "v1",
                    Title = "time-track-dshp API",
                    Description = "time-track-dshp API",
                    Contact  = new OpenApiContact { Name = "Not Me", Email = "unknown@assurecare.com", Url = new Uri("https://www.assurecare.com") }
                });                
            });

            AssureCareBindings.AddResilientHttpClientPolicies(services, Configuration);

            services.AddHealthChecks();

            // Config
            var messagingConfiguration = new MessagingConfiguration();
            var domainConfiguration = new DomainConfiguration();
            var authConfiguration = new AuthConfiguration();
            var ruleExecutionConfiguration = new RuleExecutionConfiguration();

            Configuration.Bind("Messaging", messagingConfiguration);
            Configuration.Bind("DomainMicroService", domainConfiguration);
            Configuration.Bind("Auth", authConfiguration);
            Configuration.Bind("RuleExecutionService", ruleExecutionConfiguration);

            services.AddSingleton(messagingConfiguration);
            services.AddSingleton(domainConfiguration);
            services.AddSingleton(authConfiguration);
            services.AddSingleton(ruleExecutionConfiguration);

            // Local Services
            var sqlConnection = new SqlConnection(Environment.ExpandEnvironmentVariables(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IDbConnection>(x => sqlConnection);
            services.AddSingleton<IRepository, Repository>();
            services.AddSingleton<ISqlService, SqlService>();
            services.AddSingleton<IHydrationService, HydrationService>();
            services.AddSingleton<ISyncBackMessageProducerService, SyncBackMessageProducerService>();
            services.AddSingleton<ISyncBackService, SyncBackService>();

            services.AddHttpClient<IDomainMicroService, DomainMicroService>()
                .AddPolicyHandlerFromRegistry(RetryHelper.RetryPolicyName);
            services.AddHttpClient<IAuthService, AuthService>()
                .AddPolicyHandlerFromRegistry(RetryHelper.RetryPolicyName);
            services.AddHttpClient<IRuleExecutionService, RuleExecutionService>()
                .AddPolicyHandlerFromRegistry(RetryHelper.RetryPolicyName);
            services.AddHostedService<HydrationMessageConsumerService>();
            services.AddHostedService<SyncBackMessageConsumerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> log,
            IConfiguration configuration)
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
                c.SwaggerEndpoint($"{contextRoot}/swagger/v1/swagger.json", "time-track-dshp Service V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapLocalHealthChecks();
            });
        } 
    }
}
