using System;
using Microsoft.Extensions.Logging;

namespace time_track_service.Utils
{
    public class ContextLogger<T> : ILogger<T>
    {
        private readonly ILogger _targetLogger;
        private readonly IRequestContextAccessor _requestAccessor;

        public ContextLogger(IRequestContextAccessor requestAccessor, ILogger<T> targetLogger)
        {
            _targetLogger = targetLogger ?? throw new ArgumentNullException(nameof(targetLogger));
            _requestAccessor = requestAccessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var request = _requestAccessor?.RequestContext;
            var correlationId = request?.CorrelationId ?? string.Empty;
            var tenantId = request?.TenantId ?? Guid.Empty;
            var segmentId = request?.SegmentId ?? Guid.Empty;

            var msg = formatter.Invoke(state, exception);
            var finalText = $"[cor:{correlationId}  tnt:{tenantId}  seg:{segmentId}  ] {msg}";

            _targetLogger.Log(logLevel, eventId, exception, finalText);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _targetLogger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _targetLogger.BeginScope(state);
        }
    }
}