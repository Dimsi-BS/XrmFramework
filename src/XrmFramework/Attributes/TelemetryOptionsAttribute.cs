
using System;

namespace XrmFramework
{
    /// <summary>
    /// Sets the minimum level written to the telemetry channel (Application Insights) for the
    /// whole assembly. Use <see cref="XrmLogLevel.None"/> to disable telemetry completely.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class TelemetryOptionsAttribute : Attribute
    {
        public XrmLogLevel MinimumLevel { get; }

        public TelemetryOptionsAttribute(XrmLogLevel minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }
    }
}
