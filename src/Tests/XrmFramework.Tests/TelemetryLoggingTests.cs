#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.Tests;

[TestClass]
public class TelemetryLoggingTests
{
    private readonly List<(string message, object[] args)> _traces = new();
    private readonly FakeTelemetrySink _sink = new();

    private DefaultLogHelper CreateLogger(XrmLogLevel minimumLevel = XrmLogLevel.Information)
        => new(null, new FakeLoggerContext(), (message, args) => _traces.Add((message, args)))
        {
            TelemetrySink = _sink,
            MinimumTelemetryLevel = minimumLevel
        };

    [TestMethod]
    public void Log_WritesToBothChannels()
    {
        CreateLogger().Log("Hello {0}", "world");

        Assert.AreEqual("Hello {0}", _traces.Single().message);
        Assert.AreEqual("world", _traces.Single().args.Single());

        Assert.AreEqual(XrmLogLevel.Information, _sink.Entries.Single().Level);
        Assert.AreEqual("Hello world", _sink.Entries.Single().Message);
    }

    [TestMethod]
    public void Log_BelowMinimumLevel_WritesToTracingChannelOnly()
    {
        CreateLogger().Log(XrmLogLevel.Debug, "Dump");

        Assert.AreEqual("Dump", _traces.Single().message);
        Assert.AreEqual(0, _sink.Entries.Count);
    }

    [TestMethod]
    public void Log_WithNoneAsMinimumLevel_NeverWritesTelemetry()
    {
        var logger = CreateLogger(XrmLogLevel.None);

        logger.Log(XrmLogLevel.Critical, "Boom");

        Assert.AreEqual(1, _traces.Count);
        Assert.AreEqual(0, _sink.Entries.Count);
    }

    [TestMethod]
    public void Log_WithUnmatchedBraces_DoesNotThrowAndKeepsRawMessage()
    {
        CreateLogger().Log("{\"name\":\"value\"} {0}", "suffix");

        Assert.AreEqual("{\"name\":\"value\"} {0}", _sink.Entries.Single().Message);
    }

    [TestMethod]
    public void LogWithMethodName_WritesTelemetryAtDebugLevel()
    {
        var logger = CreateLogger(XrmLogLevel.Debug);

        logger.LogWithMethodName("GetAccount", "Start");

        Assert.AreEqual("GetAccount : Start", _traces.Single().message);
        Assert.AreEqual(XrmLogLevel.Debug, _sink.Entries.Single().Level);
    }

    [TestMethod]
    public void LogError_WritesExceptionToTelemetry()
    {
        var exception = new InvalidOperationException("failed");

        CreateLogger().LogError(exception, "ERROR");

        Assert.AreEqual("ERROR : {0}\r\n{1}", _traces.Single().message);

        Assert.AreEqual(XrmLogLevel.Error, _sink.Entries.Single().Level);
        Assert.AreSame(exception, _sink.Entries.Single().Exception);
    }

    [TestMethod]
    public void LogCollection_WritesTelemetryAtDebugLevel()
    {
        var logger = CreateLogger(XrmLogLevel.Debug);

        logger.LogCollection(new Dictionary<string, object> {{"Target", "value"}});

        Assert.AreEqual(1, _sink.Entries.Count);
        Assert.IsTrue(_sink.Entries.All(e => e.Level == XrmLogLevel.Debug));
    }

    [TestMethod]
    public void IsTelemetryEnabled_FollowsMinimumLevelAndSink()
    {
        var logger = CreateLogger();

        Assert.IsTrue(logger.IsTelemetryEnabled(XrmLogLevel.Warning));
        Assert.IsFalse(logger.IsTelemetryEnabled(XrmLogLevel.Trace));

        _sink.Enabled = false;

        Assert.IsFalse(logger.IsTelemetryEnabled(XrmLogLevel.Warning));
    }

    [TestMethod]
    public void BeginScope_IsForwardedToSink()
    {
        using (CreateLogger().BeginScope("MyScope"))
        {
            Assert.AreEqual("MyScope", _sink.Scopes.Single());
        }
    }

    [TestMethod]
    public void DefaultLogger_WithoutSink_StaysSilent()
    {
        var logger = new DefaultLogHelper(null, new FakeLoggerContext(), (message, args) => _traces.Add((message, args)));

        logger.Log("Hello");

        Assert.AreEqual(1, _traces.Count);
        Assert.IsFalse(logger.IsTelemetryEnabled(XrmLogLevel.Critical));
        Assert.IsNotNull(logger.BeginScope("MyScope"));
    }

    [TestMethod]
    public void CustomLoggerOverridingLogInternal_StillWritesTelemetry()
    {
        var logger = new CustomLogHelper(new FakeLoggerContext()) {TelemetrySink = _sink};

        logger.Log("Hello");

        Assert.AreEqual("Hello", logger.OwnTraces.Single());
        Assert.AreEqual(0, _traces.Count);
        Assert.AreEqual("Hello", _sink.Entries.Single().Message);
    }

    [TestMethod]
    public void ServiceContextExtensions_UseTheLeveledLoggerWhenAvailable()
    {
        var context = new FakeServiceContext(CreateLogger());

        context.LogWarning("Careful");

        Assert.AreEqual(XrmLogLevel.Warning, _sink.Entries.Single().Level);
        Assert.AreEqual(0, context.FallbackMessages.Count);
    }

    [TestMethod]
    public void ServiceContextExtensions_FallBackToLogWhenContextExposesNoLogger()
    {
        var context = new FakeServiceContext(null);

        context.LogWarning("Careful");
        context.LogError(new InvalidOperationException("failed"), "Oops");

        Assert.AreEqual(2, context.FallbackMessages.Count);
        Assert.AreEqual("Careful", context.FallbackMessages[0]);
        Assert.IsFalse(context.IsTelemetryEnabled(XrmLogLevel.Critical));
        Assert.IsNotNull(context.BeginScope("MyScope"));
    }

    private sealed class CustomLogHelper : DefaultLogHelper
    {
        public CustomLogHelper(ILoggerContext context) : base(null, context, null)
        {
        }

        public List<string> OwnTraces { get; } = new();

        protected override void LogInternal(string message, params object[] args) => OwnTraces.Add(message);
    }

    private sealed class FakeTelemetrySink : ITelemetrySink
    {
        public bool Enabled { get; set; } = true;

        public List<(XrmLogLevel Level, string Message, Exception Exception)> Entries { get; } = new();

        public List<string> Scopes { get; } = new();

        public bool IsEnabled(XrmLogLevel level) => Enabled;

        public void Write(XrmLogLevel level, string message, Exception exception = null)
            => Entries.Add((level, message, exception));

        public IDisposable BeginScope(string name)
        {
            Scopes.Add(name);
            return NullTelemetrySink.Instance.BeginScope(name);
        }
    }

    private sealed class FakeLoggerContext : ILoggerContext
    {
        public Guid UserId => Guid.Empty;

        public Guid InitiatingUserId => Guid.Empty;

        public Guid CorrelationId => Guid.Empty;

        public string OrganizationName => "fake";
    }

    private sealed class FakeServiceContext : IServiceContext, ILoggerAccessor
    {
        public FakeServiceContext(ILogger logger) => Logger = logger;

        public ILogger Logger { get; }

        public List<string> FallbackMessages { get; } = new();

        public IOrganizationService AdminOrganizationService => null;

        public IOrganizationService OrganizationService => null;

        public EntityReference BusinessUnitRef => null;

        public Guid UserId => Guid.Empty;

        public Guid InitiatingUserId => Guid.Empty;

        public Guid CorrelationId => Guid.Empty;

        public string OrganizationName => "fake";

        public LogServiceMethod LogServiceMethod => (methodName, message, args) => FallbackMessages.Add(message);

        public void Log(string message, params object[] paramsObject) => FallbackMessages.Add(message);

        public IOrganizationService GetService(Guid userId) => null;
    }
}
