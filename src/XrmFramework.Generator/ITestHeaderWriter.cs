using System;
using System.Linq;

namespace XrmFramework.Generator
{
    public interface ITestHeaderWriter
    {
        Version DetectGeneratedTestVersion(string generatedTestContent);
    }
}
