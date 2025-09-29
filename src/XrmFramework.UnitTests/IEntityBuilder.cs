using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IEntityBuilder
{
    Entity Build();
}