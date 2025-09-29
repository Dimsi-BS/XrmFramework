namespace XrmFramework.UnitTests.Plugin;

[AttributeUsage(AttributeTargets.Method)]
public class PluginStepAttribute : Attribute
{
    public Stages Stage { get; set; }
    
    public Modes Mode { get; set; }
    
    public Messages Message { get; set; }
    
    public string PrimaryEntity { get; set; }
    
    public string MethodName { get; set; }
    
    public PluginStepAttribute(Stages stage, Messages message, Modes mode, string primaryEntity, string methodName)
    {
        Stage = stage;
        Message = message;
        Mode = mode;
        PrimaryEntity = primaryEntity;
        MethodName = methodName;
    }
}
