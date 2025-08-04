using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using System;
using type_lookup_service.Context;
using type_lookup_service.Data;
using type_lookup_service.Factories;
using type_lookup_service.Repositories;
using type_lookup_service.Services;
using type_lookup_service.Services.Internal;
using type_lookup_service.Utils;

namespace type_lookup_service.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            return services
                .AddSingleton<ConfigAsCodeService>()
                .AddSingleton<IDataService, DataService>()
                .AddSingleton<IAuthorizationService, AuthorizationService>()
                .AddSingleton<IAssessmentTemplateService, AssessmentTemplateService>()
                .AddSingleton<IMedicationService, MedicationService>()
                .AddSingleton<ILookup, JsonData>()
                .AddSingleton<IJsonRepository, JsonRepository>()
                .AddSingleton<IDbContext, DbContext>()
                .AddSingleton<IRepository, Repository>();
        }

        public static IServiceCollection AddRequestContext(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpContextAccessor();
            services.TryAddSingleton(typeof(IContextLogger<>), typeof(ContextLogger<>));

            services.TryAddSingleton<IRequestContextAccessor, RequestContextAccessor>();
            services.TryAddScoped(svc => svc.GetRequiredService<IRequestContextAccessor>().RequestContext);

            return services;
        }

        public static IServiceCollection AddResilientHttpClientPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var maxTimeBetweenRetries = configuration.GetSection("ResilientMessaging").GetValue<double>("MaxTimeBetweenRetriesMs");

            /*
             * Polly library adds resilience to our services. Within a micro service framework, it is possible that one or more dependent services
             * can be down for many different reasons. One example is a container being moved by the container management systems.
             * Retry Forever - Continue to try, increasing the time between tries until a configured maximum time between tries is reached.
             *
             * Details: https://github.com/App-vNext/Polly
             */
            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add(RetryHelper.RetryPolicyName,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryForeverAsync(attempt => TimeSpan.FromMilliseconds(RetryHelper.CalculateRetryTimeMs(attempt, maxTimeBetweenRetries))));

            return services;
        }

    }
}
