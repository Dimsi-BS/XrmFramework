using System.Linq;
using XrmFramework.Generator.Interfaces;

namespace XrmFramework.Generator
{
    public interface ITestUpToDateChecker
    {
        bool? IsUpToDatePreliminary(TableFileInput tableFileInput, string generatedTestFullPath, UpToDateCheckingMethod upToDateCheckingMethod);
        bool IsUpToDate(TableFileInput tableFileInput, string generatedTestFullPath, string generatedTestContent, UpToDateCheckingMethod upToDateCheckingMethod);
    }
}
