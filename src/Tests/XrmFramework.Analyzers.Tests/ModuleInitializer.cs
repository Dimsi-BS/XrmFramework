using System.Runtime.CompilerServices;
using DiffEngine;
using VerifyTests;

namespace XrmFramework.Analyzers.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();

            // Verify.Xunit 16.5.4 ships an EmptyFiles whose Category enum
            // cannot parse "binary" under .NET 10 — the type initializer
            // throws as soon as a baseline is missing and Verify tries to
            // launch the diff tool. We don't want a GUI diff in this repo
            // anyway, so disable the runner outright. Verify still produces
            // .received.* files next to the .verified.* ones.
            DiffRunner.Disabled = true;
        }
    }
}
