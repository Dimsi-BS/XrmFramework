using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XrmFramework.RemoteDebugger.Client.Tests.Resources;

namespace XrmFramework.RemoteDebugger.Client.Tests
{
    [TestClass]
    public class RemoteDebuggerMessageTests
    {
        public RemoteDebuggerMessageTests() { }

        [TestMethod]
        public void SerializationBackAndForthTest()
        {
            // Arrange
            var json = JsonExamples.RemoteDebugExecutionContext_Example1;

            var debugMessageContext = new RemoteDebuggerMessage()
            {
                MessageType = RemoteDebuggerMessageType.Context,
                PluginExecutionId = Guid.NewGuid(),
                Content = json
            };

            // Act
            var context = debugMessageContext.GetContext<RemoteDebugExecutionContext>();

            var reSerializeMessage =
                new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, context, Guid.NewGuid());

            // Assert
            Assert.AreEqual(json, reSerializeMessage.Content);
        }
    }
}
