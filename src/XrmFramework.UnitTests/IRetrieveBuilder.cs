using Microsoft.Xrm.Sdk;

namespace XrmFramework.UnitTests;

public interface IRetrieveBuilder
{
    IRetrieveBuilder EntityName(string entityName);
    
    IRetrieveBuilder Id(Guid id);

    IRetrieveBuilder Columns(Action<IColumnSetBuilder> builder);
    
    IRetrieveBuilder EntityReference(EntityReference entityReference);
    
    void Build();
}
