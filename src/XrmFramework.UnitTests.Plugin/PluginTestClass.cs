namespace XrmFramework.UnitTests.Plugin;

[TestClass]
public abstract class PluginTestClass<TPlugin>
    where TPlugin : XrmFramework.Plugin
{
    protected IPluginContextBuilder PluginContextBuilder { get; private set; }
    public TestContext TestContext { get; set; }

    protected void ExecutePlugin()
    {
        
    }

    [TestInitialize]
    public void TestInitialize()
    {
        var method = Type.GetType(TestContext.ManagedType)?.GetMethod(TestContext.ManagedMethod);
        var method2 = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == TestContext.ManagedType)?
            .GetMethod(TestContext.ManagedMethod);
        
        PluginContextBuilder = new PluginContextBuilder();
        
        
        
        PluginContextBuilder.WithStage(Stages.PreOperation);

    }




    [TestCleanup]
    public void TestCleanup()
    {
        
    }
}
