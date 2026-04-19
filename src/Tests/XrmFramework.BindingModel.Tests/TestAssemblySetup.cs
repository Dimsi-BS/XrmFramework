// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XrmFramework.BindingModel.Tests
{
    /// <summary>
    /// MSTest assembly-level fixture. Registers this assembly with <see cref="DefinitionCache"/> so that
    /// the test-defined entity definitions (ContactDefinition, AccountDefinition, …) can be resolved
    /// by the mappers under test.
    /// </summary>
    [TestClass]
    public class TestAssemblySetup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            DefinitionCache.RegisterAssembly(typeof(TestAssemblySetup).Assembly);
        }
    }
}
