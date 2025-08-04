using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using System;

namespace time_track_service.Utils
{
    public class AssureCareBindings
    {
        public static IServiceCollection AddRequestContext(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpContextAccessor();
            services.TryAddSingleton(typeof(ContextLogger<>));

            services.TryAddSingleton<IRequestContextAccessor, RequestContextAccessor>();
            services.TryAddScoped(svc => svc.GetRequiredService<IRequestContextAccessor>().RequestContext);

            return services;
        }

        public static IServiceCollection AddResilientHttpClientPolicies(IServiceCollection services, IConfiguration configuration)
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