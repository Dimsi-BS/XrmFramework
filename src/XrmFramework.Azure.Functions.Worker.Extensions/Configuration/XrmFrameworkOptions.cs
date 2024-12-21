using System.Reflection;

namespace XrmFramework.Azure.Functions.Worker.Extensions.Configuration;

internal class XrmFrameworkOptions
{
    public Assembly[]? MediatRAssemblies { get; set; }
    public Assembly[]? FluentValidationAssemblies { get; set; }
}
