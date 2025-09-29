namespace XrmFramework.UnitTests;

public interface IMockOrganizationService
{
    IMockOrganizationService RetrieveAll(Action<IRetrieveAllChecker> builder);
    
    IMockOrganizationService RetrieveMultiple(Action<IRetrieveAllChecker> builder);

    IMockOrganizationService Retrieve(Action<IRetrieveBuilder> builder);
}
