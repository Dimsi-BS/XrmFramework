// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace XrmFramework.Tests.Utils
{
    [TestFixture]
    public class TelemetryLoggingTests
    {
        private List<TraceEntry> _traces = null!;
        private FakeTelemetrySink _sink = null!;

        [SetUp]
        public void InitTests()
        {
            _traces = new List<TraceEntry>();
            _sink = new FakeTelemetrySink();
        }

        private DefaultLogHelper CreateLogger(XrmLogLevel minimumLevel = XrmLogLevel.Information)
            => new DefaultLogHelper(null, new FakeLoggerContext(), (message, args) => _traces.Add(new TraceEntry(message, args)))
            {
                TelemetrySink = _sink,
                MinimumTelemetryLevel = minimumLevel
            };

        // ─────────────────────────────────────────────────────────────
        //  Double write — tracing channel and telemetry channel
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Log_WritesToBothChannels()
        {
            CreateLogger().Log("Hello {0}", "world");

            Assert.That(_traces.Single().Message, Is.EqualTo("Hello {0}"));
            Assert.That(_traces.Single().Args.Single(), Is.EqualTo("world"));

            Assert.That(_sink.Entries.Single().Level, Is.EqualTo(XrmLogLevel.Information));
            Assert.That(_sink.Entries.Single().Message, Is.EqualTo("Hello world"));
        }

        [Test]
        public void Log_BelowMinimumLevel_WritesToTracingChannelOnly()
        {
            CreateLogger().Log(XrmLogLevel.Debug, "Dump");

            Assert.That(_traces.Single().Message, Is.EqualTo("Dump"));
            Assert.That(_sink.Entries, Is.Empty);
        }

        [Test]
        public void Log_WithNoneAsMinimumLevel_NeverWritesTelemetry()
        {
            CreateLogger(XrmLogLevel.None).Log(XrmLogLevel.Critical, "Boom");

            Assert.That(_traces, Has.Count.EqualTo(1));
            Assert.That(_sink.Entries, Is.Empty);
        }

        [Test]
        public void Log_WithUnmatchedBraces_DoesNotThrowAndKeepsRawMessage()
        {
            CreateLogger().Log("{\"name\":\"value\"} {0}", "suffix");

            Assert.That(_sink.Entries.Single().Message, Is.EqualTo("{\"name\":\"value\"} {0}"));
        }

        [Test]
        public void LogWithMethodName_WritesTelemetryAtDebugLevel()
        {
            CreateLogger(XrmLogLevel.Debug).LogWithMethodName("GetAccount", "Start");

            Assert.That(_traces.Single().Message, Is.EqualTo("GetAccount : Start"));
            Assert.That(_sink.Entries.Single().Level, Is.EqualTo(XrmLogLevel.Debug));
        }

        [Test]
        public void LogError_WritesExceptionToTelemetry()
        {
            var exception = new InvalidOperationException("failed");

            CreateLogger().LogError(exception, "ERROR");

            Assert.That(_traces.Single().Message, Is.EqualTo("ERROR : {0}\r\n{1}"));

            Assert.That(_sink.Entries.Single().Level, Is.EqualTo(XrmLogLevel.Error));
            Assert.That(_sink.Entries.Single().Exception, Is.SameAs(exception));
        }

        [Test]
        public void LogCollection_WritesTelemetryAtDebugLevel()
        {
            CreateLogger(XrmLogLevel.Debug).LogCollection(new Dictionary<string, object> {{"Target", "value"}});

            Assert.That(_sink.Entries, Has.Count.EqualTo(1));
            Assert.That(_sink.Entries.Single().Level, Is.EqualTo(XrmLogLevel.Debug));
        }

        // ─────────────────────────────────────────────────────────────
        //  Level gate and scopes
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void IsTelemetryEnabled_FollowsMinimumLevelAndSink()
        {
            var logger = CreateLogger();

            Assert.That(logger.IsTelemetryEnabled(XrmLogLevel.Warning), Is.True);
            Assert.That(logger.IsTelemetryEnabled(XrmLogLevel.Trace), Is.False);

            _sink.Enabled = false;

            Assert.That(logger.IsTelemetryEnabled(XrmLogLevel.Warning), Is.False);
        }

        [Test]
        public void BeginScope_IsForwardedToSink()
        {
            using (CreateLogger().BeginScope("MyScope"))
            {
                Assert.That(_sink.Scopes.Single(), Is.EqualTo("MyScope"));
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  Backward compatibility
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void DefaultLogger_WithoutSink_StaysSilent()
        {
            var logger = new DefaultLogHelper(null, new FakeLoggerContext(), (message, args) => _traces.Add(new TraceEntry(message, args)));

            logger.Log("Hello");

            Assert.That(_traces, Has.Count.EqualTo(1));
            Assert.That(logger.IsTelemetryEnabled(XrmLogLevel.Critical), Is.False);
            Assert.That(logger.BeginScope("MyScope"), Is.Not.Null);
        }

        [Test]
        public void CustomLoggerOverridingLogInternal_StillWritesTelemetry()
        {
            var logger = new CustomLogHelper(new FakeLoggerContext()) {TelemetrySink = _sink};

            logger.Log("Hello");

            Assert.That(logger.OwnTraces.Single(), Is.EqualTo("Hello"));
            Assert.That(_traces, Is.Empty);
            Assert.That(_sink.Entries.Single().Message, Is.EqualTo("Hello"));
        }

        // ─────────────────────────────────────────────────────────────
        //  Context extension methods
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ServiceContextExtensions_UseTheLeveledLoggerWhenAvailable()
        {
            var fallbackMessages = new List<string>();
            var context = BuildServiceContext(CreateLogger(), fallbackMessages);

            context.LogWarning("Careful");

            Assert.That(_sink.Entries.Single().Level, Is.EqualTo(XrmLogLevel.Warning));
            Assert.That(fallbackMessages, Is.Empty);
        }

        [Test]
        public void ServiceContextExtensions_FallBackToLogWhenContextExposesNoLogger()
        {
            var fallbackMessages = new List<string>();
            var context = BuildServiceContext(null, fallbackMessages);

            context.LogWarning("Careful");
            context.LogError(new InvalidOperationException("failed"), "Oops");

            Assert.That(fallbackMessages, Has.Count.EqualTo(2));
            Assert.That(fallbackMessages[0], Is.EqualTo("Careful"));
            Assert.That(context.IsTelemetryEnabled(XrmLogLevel.Critical), Is.False);
            Assert.That(context.BeginScope("MyScope"), Is.Not.Null);
        }

        /// <summary>
        /// Mocked rather than hand written: <see cref="IServiceContext"/> grows over time and only
        /// <c>Log</c> and the logger accessor matter here.
        /// </summary>
        private static IServiceContext BuildServiceContext(ILogger? logger, List<string> fallbackMessages)
        {
            var contextMock = new Mock<IServiceContext>();

            contextMock
                .Setup(c => c.Log(It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<string, object[]>((message, args) => fallbackMessages.Add(message));

            contextMock.As<ILoggerAccessor>().Setup(a => a.Logger).Returns(logger!);

            return contextMock.Object;
        }

        // ─────────────────────────────────────────────────────────────
        //  Fakes
        // ─────────────────────────────────────────────────────────────

        private class TraceEntry
        {
            public TraceEntry(string message, object[] args)
            {
                Message = message;
                Args = args;
            }

            public string Message { get; }

            public object[] Args { get; }
        }

        private class SinkEntry
        {
            public SinkEntry(XrmLogLevel level, string message, Exception? exception)
            {
                Level = level;
                Message = message;
                Exception = exception;
            }

            public XrmLogLevel Level { get; }

            public string Message { get; }

            public Exception? Exception { get; }
        }

        private class CustomLogHelper : DefaultLogHelper
        {
            public CustomLogHelper(ILoggerContext context) : base(null, context, null)
            {
            }

            public List<string> OwnTraces { get; } = new List<string>();

            protected override void LogInternal(string message, params object[] args) => OwnTraces.Add(message);
        }

        private class FakeTelemetrySink : ITelemetrySink
        {
            public bool Enabled { get; set; } = true;

            public List<SinkEntry> Entries { get; } = new List<SinkEntry>();

            public List<string> Scopes { get; } = new List<string>();

            public bool IsEnabled(XrmLogLevel level) => Enabled;

            public void Write(XrmLogLevel level, string message, Exception? exception = null)
                => Entries.Add(new SinkEntry(level, message, exception));

            public IDisposable BeginScope(string name)
            {
                Scopes.Add(name);
                return NullTelemetrySink.Instance.BeginScope(name);
            }
        }

        private class FakeLoggerContext : ILoggerContext
        {
            public Guid UserId => Guid.Empty;

            public Guid InitiatingUserId => Guid.Empty;

            public Guid CorrelationId => Guid.Empty;

            public string OrganizationName => "fake";
        }

    }
}
