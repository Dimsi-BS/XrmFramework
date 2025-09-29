namespace XrmFramework.UnitTests.Plugin;

public interface IInputParametersBuilder
{
    IInputParametersBuilder WithEntity(InputParameters inputParameter, Action<IEntityBuilder> builder);
    IInputParametersBuilder WithObject(InputParameters inputParameter, object value);
}