using System;

namespace XrmFramework.Utils
{
    public interface ILoggerContext
    {
        Guid UserId { get; }

        Guid InitiatingUserId { get; }

        Guid CorrelationId { get; }

        string OrganizationName { get; }
    }
}
