using System;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Moq;
using NUnit.Framework;

namespace XrmFramework.Tests;

[TestFixture]
public class LocalContextTests
{
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IPluginExecutionContext7> _executionContextMock = null!;
    private Mock<ITracingService> _tracingServiceMock = null!;
    private Mock<IOrganizationServiceFactory> _serviceFactoryMock = null!;
    private Mock<IOrganizationService> _organizationServiceMock = null!;
    private Mock<IOrganizationService> _adminServiceMock = null!;

    private Guid _userId;
    private Guid _initiatingUserId;
    private Guid _correlationId;
    private Guid _businessUnitId;
    private string _organizationName;

    [SetUp]
    public void InitTests()
    {
        _userId = Guid.NewGuid();
        _initiatingUserId = Guid.NewGuid();
        _correlationId = Guid.NewGuid();
        _businessUnitId = Guid.NewGuid();
        _organizationName = "TestOrg";

        _serviceProviderMock = new Mock<IServiceProvider>();
        _executionContextMock = new Mock<IPluginExecutionContext7>();
        _tracingServiceMock = new Mock<ITracingService>();
        _serviceFactoryMock = new Mock<IOrganizationServiceFactory>();
        _organizationServiceMock = new Mock<IOrganizationService>();
        _adminServiceMock = new Mock<IOrganizationService>();

        _executionContextMock.Setup(e => e.UserId).Returns(_userId);
        _executionContextMock.Setup(e => e.InitiatingUserId).Returns(_initiatingUserId);
        _executionContextMock.Setup(e => e.CorrelationId).Returns(_correlationId);
        _executionContextMock.Setup(e => e.BusinessUnitId).Returns(_businessUnitId);
        _executionContextMock.Setup(e => e.OrganizationName).Returns(_organizationName);
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Synchronous);
        _executionContextMock.Setup(e => e.PreEntityImages).Returns(new EntityImageCollection());
        _executionContextMock.Setup(e => e.PostEntityImages).Returns(new EntityImageCollection());
        _executionContextMock.Setup(e => e.InputParameters).Returns(new ParameterCollection());
        _executionContextMock.Setup(e => e.OutputParameters).Returns(new ParameterCollection());
        _executionContextMock.Setup(e => e.SharedVariables).Returns(new ParameterCollection());

        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext2))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext3))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext4))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext5))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext6))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IPluginExecutionContext7))).Returns(_executionContextMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(ITracingService))).Returns(_tracingServiceMock.Object);
        _serviceProviderMock.Setup(s => s.GetService(typeof(IOrganizationServiceFactory))).Returns(_serviceFactoryMock.Object);

        _serviceFactoryMock.Setup(f => f.CreateOrganizationService(_userId)).Returns(_organizationServiceMock.Object);
        _serviceFactoryMock.Setup(f => f.CreateOrganizationService(null)).Returns(_adminServiceMock.Object);
    }

    [Test]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalContext(null));
    }

    [Test]
    public void Constructor_ValidServiceProvider_InitializesCorrectly()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        Assert.IsNotNull(context);
        Assert.IsNotNull(context.ExecutionContext);
        Assert.IsNotNull(context.TracingService);
        Assert.IsNotNull(context.OrganizationService);
        Assert.AreEqual(_userId, context.UserId);
        Assert.AreEqual(_initiatingUserId, context.InitiatingUserId);
        Assert.AreEqual(_correlationId, context.CorrelationId);
        Assert.AreEqual(_organizationName, context.OrganizationName);
    }

    [Test]
    public void UserRef_ReturnsCorrectEntityReference()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var userRef = context.UserRef;

        Assert.IsNotNull(userRef);
        Assert.AreEqual(SystemUserDefinition.EntityName, userRef.LogicalName);
        Assert.AreEqual(_userId, userRef.Id);
    }

    [Test]
    public void BusinessUnitRef_ReturnsCorrectEntityReference()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var businessUnitRef = context.BusinessUnitRef;

        Assert.IsNotNull(businessUnitRef);
        Assert.AreEqual("businessunit", businessUnitRef.LogicalName);
        Assert.AreEqual(_businessUnitId, businessUnitRef.Id);
    }

    [Test]
    public void AdminOrganizationService_ReturnsAdminService()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var adminService = context.AdminOrganizationService;

        Assert.IsNotNull(adminService);
        Assert.AreEqual(_adminServiceMock.Object, adminService);
        _serviceFactoryMock.Verify(f => f.CreateOrganizationService(null), Times.Once);
    }

    [Test]
    public void AdminOrganizationService_CalledMultipleTimes_ReturnsCachedInstance()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var adminService1 = context.AdminOrganizationService;
        var adminService2 = context.AdminOrganizationService;

        Assert.AreSame(adminService1, adminService2);
        _serviceFactoryMock.Verify(f => f.CreateOrganizationService(null), Times.Once);
    }

    [Test]
    public void MessageName_ReturnsCorrectMessage()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Create");
        var context = new LocalContext(_serviceProviderMock.Object);

        var messageName = context.MessageName;

        Assert.AreEqual(Messages.Create, messageName);
    }

    [Test]
    public void Log_CallsLoggerLog()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        context.Log("Test message with {0}", "parameter");

        _tracingServiceMock.Verify(t => t.Trace(It.IsAny<string>(), new object[] {It.IsAny<string>()}), Times.AtLeastOnce);
    }

    [Test]
    public void LogError_CallsLoggerLogError()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        var exception = new Exception("Test exception");

        context.LogError(exception);

        _tracingServiceMock.Verify(t => t.Trace(It.IsAny<string>(), new object[]{"ERROR", It.IsAny<Exception>()}), Times.AtLeastOnce);
    }

    #region Image Helpers Tests

    [Test]
    public void HasPreImage_ImageExists_ReturnsTrue()
    {
        var images = new EntityImageCollection { { "PreImage", new Entity() } };
        _executionContextMock.Setup(e => e.PreEntityImages).Returns(images);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasPreImage("PreImage");

        Assert.IsTrue(result);
    }

    [Test]
    public void HasPreImage_ImageDoesNotExist_ReturnsFalse()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasPreImage("NonExistent");

        Assert.IsFalse(result);
    }

    [Test]
    public void GetPreImage_ImageExists_ReturnsImage()
    {
        var entity = new Entity("account");
        var images = new EntityImageCollection { { "PreImage", entity } };
        _executionContextMock.Setup(e => e.PreEntityImages).Returns(images);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetPreImage("PreImage");

        Assert.AreSame(entity, result);
    }

    [Test]
    public void GetPreImage_ImageDoesNotExist_ThrowsArgumentNullException()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        Assert.Throws<ArgumentNullException>(() => context.GetPreImage("NonExistent"));
    }

    [Test]
    public void GetPreImageOrDefault_ImageExists_ReturnsImage()
    {
        var entity = new Entity("account");
        var images = new EntityImageCollection { { "PreImage", entity } };
        _executionContextMock.Setup(e => e.PreEntityImages).Returns(images);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetPreImageOrDefault("PreImage");

        Assert.AreSame(entity, result);
    }

    [Test]
    public void GetPreImageOrDefault_ImageDoesNotExist_ReturnsNull()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetPreImageOrDefault("NonExistent");

        Assert.IsNull(result);
    }

    [Test]
    public void HasPostImage_ImageExists_ReturnsTrue()
    {
        var images = new EntityImageCollection { { "PostImage", new Entity() } };
        _executionContextMock.Setup(e => e.PostEntityImages).Returns(images);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasPostImage("PostImage");

        Assert.IsTrue(result);
    }

    [Test]
    public void HasPostImage_ImageDoesNotExist_ReturnsFalse()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasPostImage("NonExistent");

        Assert.IsFalse(result);
    }

    [Test]
    public void GetPostImage_ImageExists_ReturnsImage()
    {
        var entity = new Entity("account");
        var images = new EntityImageCollection { { "PostImage", entity } };
        _executionContextMock.Setup(e => e.PostEntityImages).Returns(images);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetPostImage("PostImage");

        Assert.AreSame(entity, result);
    }

    [Test]
    public void GetPostImage_ImageDoesNotExist_ThrowsArgumentNullException()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        Assert.Throws<ArgumentNullException>(() => context.GetPostImage("NonExistent"));
    }

    #endregion

    #region Message/Stage/Mode Helpers Tests

    [Test]
    public void IsCreate_MessageIsCreate_ReturnsTrue()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Create");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsCreate();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsCreate_MessageIsNotCreate_ReturnsFalse()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Update");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsCreate();

        Assert.IsFalse(result);
    }

    [Test]
    public void IsUpdate_MessageIsUpdate_ReturnsTrue()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Update");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsUpdate();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsUpdate_MessageIsNotUpdate_ReturnsFalse()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Create");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsUpdate();

        Assert.IsFalse(result);
    }

    [Test]
    public void IsMessage_MatchingMessage_ReturnsTrue()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Delete");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsMessage(Messages.Delete);

        Assert.IsTrue(result);
    }

    [Test]
    public void IsMessage_NonMatchingMessage_ReturnsFalse()
    {
        _executionContextMock.Setup(e => e.MessageName).Returns("Create");
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsMessage(Messages.Delete);

        Assert.IsFalse(result);
    }

    [Test]
    public void IsSynchronous_ModeIsSynchronous_ReturnsTrue()
    {
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Synchronous);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsSynchronous();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsSynchronous_ModeIsAsynchronous_ReturnsFalse()
    {
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Asynchronous);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsSynchronous();

        Assert.IsFalse(result);
    }

    [Test]
    public void IsAsynchronous_ModeIsAsynchronous_ReturnsTrue()
    {
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Asynchronous);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsAsynchronous();

        Assert.IsTrue(result);
    }

    [Test]
    public void IsAsynchronous_ModeIsSynchronous_ReturnsFalse()
    {
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Synchronous);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.IsAsynchronous();

        Assert.IsFalse(result);
    }

    [Test]
    public void Mode_ValidMode_ReturnsMode()
    {
        _executionContextMock.Setup(e => e.Mode).Returns((int)Modes.Synchronous);
        var context = new LocalContext(_serviceProviderMock.Object);

        var mode = context.Mode;

        Assert.AreEqual(Modes.Synchronous, mode);
    }

    #endregion

    #region Parameters Helpers Tests

    [Test]
    public void GetInputParameter_ParameterExists_ReturnsValue()
    {
        var parameters = new ParameterCollection { { "Target", new Entity("account") } };
        _executionContextMock.Setup(e => e.InputParameters).Returns(parameters);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetInputParameter<Entity>(InputParameters.Target);

        Assert.IsNotNull(result);
        Assert.AreEqual("account", result.LogicalName);
    }

    [Test]
    public void GetInputParameter_ParameterDoesNotExist_ThrowsArgumentNullException()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        Assert.Throws<ArgumentNullException>(() => context.GetInputParameter<Entity>(InputParameters.Target));
    }

    [Test]
    public void SetInputParameter_SetsParameterValue()
    {
        var entity = new Entity("account");
        var context = new LocalContext(_serviceProviderMock.Object);

        context.SetInputParameter(InputParameters.Target, entity);

        Assert.IsTrue(context.ExecutionContext.InputParameters.Contains("Target"));
        Assert.AreSame(entity, context.ExecutionContext.InputParameters["Target"]);
    }

    [Test]
    public void GetOutputParameter_ParameterExists_ReturnsValue()
    {
        var id = Guid.NewGuid();
        var parameters = new ParameterCollection { { "BusinessEntityCollection", id } };
        _executionContextMock.Setup(e => e.OutputParameters).Returns(parameters);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetOutputParameter<Guid>(OutputParameters.BusinessEntityCollection);

        Assert.AreEqual(id, result);
    }

    [Test]
    public void SetOutputParameter_SetsParameterValue()
    {
        var id = Guid.NewGuid();
        var context = new LocalContext(_serviceProviderMock.Object);

        context.SetOutputParameter(OutputParameters.BusinessEntityCollection, id);

        Assert.IsTrue(context.ExecutionContext.OutputParameters.Contains("BusinessEntityCollection"));
        Assert.AreEqual(id, context.ExecutionContext.OutputParameters["BusinessEntityCollection"]);
    }

    [Test]
    public void HasSharedVariable_VariableExists_ReturnsTrue()
    {
        var variables = new ParameterCollection { { "TestVar", "TestValue" } };
        _executionContextMock.Setup(e => e.SharedVariables).Returns(variables);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasSharedVariable("TestVar");

        Assert.IsTrue(result);
    }

    [Test]
    public void HasSharedVariable_VariableDoesNotExist_ReturnsFalse()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.HasSharedVariable("NonExistent");

        Assert.IsFalse(result);
    }

    [Test]
    public void SetSharedVariable_StringValue_SetsVariable()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        context.SetSharedVariable("TestVar", "TestValue");

        Assert.IsTrue(context.ExecutionContext.SharedVariables.Contains("TestVar"));
        Assert.AreEqual("TestValue", context.ExecutionContext.SharedVariables["TestVar"]);
    }

    [Test]
    public void GetSharedVariable_VariableExists_ReturnsValue()
    {
        var variables = new ParameterCollection { { "TestVar", "TestValue" } };
        _executionContextMock.Setup(e => e.SharedVariables).Returns(variables);
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetSharedVariable<string>("TestVar");

        Assert.AreEqual("TestValue", result);
    }

    [Test]
    public void GetSharedVariable_VariableDoesNotExist_ReturnsDefault()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetSharedVariable<string>("NonExistent");

        Assert.IsNull(result);
    }

    #endregion


    [Test]
    public void GetInitiatingUserId_NoParent_ReturnsInitiatingUserId()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetInitiatingUserId();

        Assert.AreEqual(_initiatingUserId, result);
    }

    [Test]
    public void GetRootUserId_NoParent_ReturnsUserId()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        var result = context.GetRootUserId();

        Assert.AreEqual(_userId, result);
    }

    [Test]
    public void LogFields_CallsLoggerLogCollection()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        var entity = new Entity("account");
        entity["name"] = "Test Account";
        entity["revenue"] = new Money(1000);

        context.LogFields(entity, "name", "revenue");

        _tracingServiceMock.Verify(t => t.Trace(It.IsAny<string>(), new object[]{ It.IsAny<string>(), It.IsAny<string>()}), Times.AtLeast(2));
    }

    [Test]
    public void InvokeMethod_NoParameters_InvokesSuccessfully()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        var testObject = new TestClass();
        var method = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutParameters));

        context.InvokeMethod(testObject, method);

        Assert.IsTrue(testObject.MethodCalled);
    }

    [Test]
    public void InvokeMethod_WithParameters_InvokesSuccessfully()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        var testObject = new TestClass();
        var method = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithContext));

        context.InvokeMethod(testObject, method);

        Assert.IsTrue(testObject.MethodWithContextCalled);
    }

    [Test]
    public void InvokeMethod_TaskMethod_WaitsForCompletion()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        var testObject = new TestClass();
        var method = typeof(TestClass).GetMethod(nameof(TestClass.AsyncMethod));

        context.InvokeMethod(testObject, method);

        Assert.IsTrue(testObject.AsyncMethodCalled);
    }

    [Test]
    public void DumpSharedVariables_CallsLoggerLogCollection()
    {
        var context = new LocalContext(_serviceProviderMock.Object);

        _executionContextMock
            .Setup(context => context.SharedVariables)
            .Returns(new ParameterCollection()
            {
                { "TestVar", "TestValue" }
            });
        
        context.DumpSharedVariables();

        _tracingServiceMock.Verify(t => t.Trace(It.IsAny<string>(), new object[]{It.IsAny<string>(), It.IsAny<string>()}), Times.AtLeastOnce);
    }

    [Test]
    public void DumpInputParameters_CallsLoggerLogCollection()
    {
        var context = new LocalContext(_serviceProviderMock.Object);
        
        _executionContextMock
            .Setup(context => context.InputParameters)
            .Returns(new ParameterCollection()
            {
                { "Target", new Entity("account")
                {
                    ["name"] = "Test Account"
                } }
            });
        

        context.DumpInputParameters();

        _tracingServiceMock.Verify(t => t.Trace(It.IsAny<string>(), new object[]{It.IsAny<string>(), It.IsAny<string>()}), Times.AtLeastOnce);
    }

    // Helper test class
    private class TestClass
    {
        public bool MethodCalled { get; private set; }
        public bool MethodWithContextCalled { get; private set; }
        public bool AsyncMethodCalled { get; private set; }

        public void MethodWithoutParameters()
        {
            MethodCalled = true;
        }

        public void MethodWithContext(IServiceContext context)
        {
            MethodWithContextCalled = true;
        }

        public async Task AsyncMethod()
        {
            await Task.Delay(1);
            AsyncMethodCalled = true;
        }
    }
}
