using Microsoft.Extensions.Logging;

namespace type_lookup_service.Utils
{
    /// <summary>
    /// An implementation of <see cref="ILogger"> that adds context identifiers to the to the log message.
    /// </summary>
    /// <typeparam name="TCategoryName"></typeparam>
    public interface IContextLogger<out TCategoryName> : ILogger<TCategoryName> { }
}
