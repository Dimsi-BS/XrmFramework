using System;

namespace XrmFramework.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    public interface ITestGenerator : IDisposable
    {
        TestGeneratorResult GenerateTestFile(TableFileInput tableFileInput, GenerationSettings settings);
        Version DetectGeneratedTestVersion(TableFileInput tableFileInput);
        string GetTestFullPath(TableFileInput tableFileInput);
    }
}
