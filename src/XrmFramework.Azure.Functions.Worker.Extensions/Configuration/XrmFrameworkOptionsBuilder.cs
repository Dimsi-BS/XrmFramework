using System.Reflection;

namespace XrmFramework.Azure.Functions.Worker.Extensions.Configuration;

public class XrmFrameworkOptionsBuilder
{
    private readonly XrmFrameworkOptions _xrmFrameworkOptions;

    internal XrmFrameworkOptionsBuilder(XrmFrameworkOptions xrmFrameworkOptions)
    {
        _xrmFrameworkOptions = xrmFrameworkOptions;
    }

    public XrmFrameworkOptionsBuilder RegisterMediatRServicesFromAssemblies(params Assembly[] assemblies)
    {
        _xrmFrameworkOptions.MediatRAssemblies = assemblies;
        return this;
    }
    
    public XrmFrameworkOptionsBuilder AddFluentValidation(params Assembly[] assemblies)
    {
        _xrmFrameworkOptions.FluentValidationAssemblies = assemblies;
        return this;
    }
}
