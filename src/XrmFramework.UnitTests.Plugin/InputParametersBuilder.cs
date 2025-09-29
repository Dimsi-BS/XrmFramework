namespace XrmFramework.UnitTests.Plugin;

internal class InputParametersBuilder : IInputParametersBuilder
{
    private readonly Dictionary<InputParameters, object> _inputParameters = new();
    
    public IInputParametersBuilder WithEntity(InputParameters inputParameter, Action<IEntityBuilder> builder)
    {
        var entityBuilder = new EntityBuilder();
        builder.Invoke(entityBuilder);
        
        _inputParameters[inputParameter] = entityBuilder.Build();
        
        return this;
    }

    public IInputParametersBuilder WithObject(InputParameters inputParameter, object value)
    {
        _inputParameters[inputParameter] =value;
        
        return this;
    }

    public IDictionary<InputParameters, object> Build()
    {
        return new Dictionary<InputParameters, object>(_inputParameters);
    }
}