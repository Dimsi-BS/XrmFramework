// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// NUnit assembly-level setup fixture. Registers this assembly with <see cref="DefinitionCache"/> so that
    /// the test-defined entity definitions (ContactDefinition, AccountDefinition, …) can be resolved
    /// by the mappers under test.
    /// </summary>
    [SetUpFixture]
    public class TestAssemblySetup
    {
        [OneTimeSetUp]
        public void AssemblyInitialize()
        {
            DefinitionCache.RegisterAssembly(typeof(TestAssemblySetup).Assembly);
        }
    }
}
