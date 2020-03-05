namespace Plugins
{
    partial interface ICustomWorkflowContext
    {
        T GetService<T>() where T : IService;
    }
}
