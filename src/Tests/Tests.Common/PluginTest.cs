// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Model;
using Plugins;
using Plugins.Tests;
using Tests.Common.Utils;
using Xunit.Abstractions;
using FakeXrmEasy.Extensions;
using Microsoft.Xrm.Sdk.Metadata;
using Tests.Common.Metadata;
using AttributeMetadata = Microsoft.Xrm.Sdk.Metadata.AttributeMetadata;
using AttributeTypeCode = Model.Sdk.AttributeTypeCode;
using DateTimeBehavior = Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior;
using EntityMetadata = Microsoft.Xrm.Sdk.Metadata.EntityMetadata;

namespace UnitTest.Plugins.Utils
{
    public abstract class PluginTest<TPlugin> where TPlugin : Plugin
    {
        protected XrmFakedContext FakedContext { get; } = new XrmFakedContext();

        protected IOrganizationService FakedService { get; }

        protected  XrmFakedPluginExecutionContext Context { get; }

        protected GenericParameterCollection<InputParameters, object> InputParameters { get; } = new GenericParameterCollection<InputParameters, object>();

        protected PluginTest() { }

        protected PluginTest(ITestOutputHelper output)
        {
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            if (testMember == null)
            {
                throw new Exception("The passed ITestOutputHelper is not correctly initialized");
            }
            var test = (ITest)testMember.GetValue(output);

            var method = test.TestCase.TestMethod.Method;

            var contextAttribute = method.GetCustomAttributes(typeof(PluginContextAttribute).AssemblyQualifiedName).FirstOrDefault();

            if (contextAttribute == null)
            {
                throw new Exception("Plugin Test should provide the corresponding plugin context, define a [PluginContext(...)] attribute on your test");
            }

            var stage = contextAttribute.GetNamedArgument<Stages>(nameof(PluginContextAttribute.Stage));
            var message = contextAttribute.GetNamedArgument<Messages>(nameof(PluginContextAttribute.Message));
            var mode = contextAttribute.GetNamedArgument<Modes>(nameof(PluginContextAttribute.Mode));
            var entityName = contextAttribute.GetNamedArgument<string>(nameof(PluginContextAttribute.EntityName));

            FakedService = FakedContext.GetOrganizationService();

            Context = FakedContext.GetDefaultPluginContext();
            Context.Stage = stage.ToInt();
            Context.MessageName = message.ToString();
            Context.Mode = mode.ToInt();
            Context.PrimaryEntityName = entityName;

            Context.InputParameters = InputParameters.Collection;


            //InitializeMetadata();
        }

        private void InitializeMetadata()
        {
            var metadataList = new List<EntityMetadata>();
            foreach (var type in typeof(AccountDefinition).Assembly.GetExportedTypes())
            {
                if (type.GetCustomAttribute<EntityDefinitionAttribute>() == null)
                {
                    continue;
                }

                var definition = DefinitionCache.GetEntityDefinition(type);

                var metadata = definition.ToEntityMetadata();

                metadataList.Add(metadata);
            }
            //FakedContext.InitializeMetadata(metadataList);
        }

        protected void ExecutePlugin()
        {
            FakedContext.ExecutePluginWithConfigurations<TPlugin>(Context, null, null);
        }
    }
}
