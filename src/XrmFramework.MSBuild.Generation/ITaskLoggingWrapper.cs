namespace XrmFramework.MSBuild.Generation
{
    public interface ITaskLoggingWrapper
    {
        void LogMessage(string message);

        void LogMessageWithLowImportance(string message);

        void LogError(string message);

        bool HasLoggedErrors();
    }
}
