// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;

namespace XrmFramework.Tests.Sdk.Queries
{
    [SetUpFixture]
    public class QueryTestsAssemblySetup
    {
        [OneTimeSetUp]
        public void AssemblyInitialize()
        {
            DefinitionCache.RegisterAssembly(typeof(QueryTestsAssemblySetup).Assembly);
        }

    }
}
