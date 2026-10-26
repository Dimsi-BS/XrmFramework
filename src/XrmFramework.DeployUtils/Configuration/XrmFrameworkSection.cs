// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Configuration;

namespace XrmFramework.DeployUtils.Configuration
{
    [Serializable]
    public class XrmFrameworkSection : ConfigurationSection
    {
        private static readonly ConfigurationPropertyCollection _properties;
        private static readonly ConfigurationProperty _propProjects;
        private static readonly ConfigurationProperty _entityProperty;
        private static readonly ConfigurationProperty _selectedConnectionProperty;

        [ConfigurationProperty("entitySolution", IsRequired = true)]
        public EntitySolutionElement EntitySolution
        {
            get => (EntitySolutionElement)base[_entityProperty];
            set => base[_entityProperty] = value;
        }

        [ConfigurationProperty("selectedConnection", IsRequired = true)]
        public string SelectedConnection
        {
            get => base["selectedConnection"] as string;
            set => base["selectedConnection"] = value;
        }

        [ConfigurationProperty("projects", IsRequired = true)]
        [ConfigurationCollection(typeof(ProjectCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ProjectCollection Projects => (ProjectCollection)base[_propProjects];

        static XrmFrameworkSection()
        {
            _propProjects = new ConfigurationProperty("projects", typeof(ProjectCollection), null, ConfigurationPropertyOptions.IsDefaultCollection | ConfigurationPropertyOptions.IsRequired);
            _entityProperty = new ConfigurationProperty("entitySolution", typeof(EntitySolutionElement), null, ConfigurationPropertyOptions.IsRequired);
            _selectedConnectionProperty = new ConfigurationProperty("selectedConnection", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            _properties = new ConfigurationPropertyCollection
            {
                _propProjects, _entityProperty, _selectedConnectionProperty
            };
        }

        protected override ConfigurationPropertyCollection Properties => _properties;
    }

    [Serializable]
    public class EntitySolutionElement : ConfigurationElement
    {
        private static readonly ConfigurationPropertyCollection _properties;
        private static readonly ConfigurationProperty _nameProperty;

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get => (string)this[_nameProperty];
            set => this[_nameProperty] = value;
        }

        static EntitySolutionElement()
        {
            _nameProperty = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
            _properties = new ConfigurationPropertyCollection { _nameProperty };
        }

        protected override ConfigurationPropertyCollection Properties => _properties;
    }

    [Serializable]
    public class ProjectCollection : ConfigurationElementCollection
    {
        private static readonly ConfigurationProperty ProjectProperty;

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProjectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProjectElement)element).Name;
        }

        public new ProjectElement this[string index] => (ProjectElement)BaseGet(index);

        public void Add(ConfigurationElement project)
        {
            BaseAdd(project);
        }

        static ProjectCollection()
        {
            ProjectProperty = new ConfigurationProperty("project", typeof(ProjectElement), null, ConfigurationPropertyOptions.IsRequired);
        }

        //protected override ConfigurationPropertyCollection Properties => _properties;
    }

    [Serializable]
    public class TestProjectCollection : ConfigurationElementCollection
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new TestProjectElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TestProjectElement)element).Name;
        }

        public new TestProjectElement this[string index] => (TestProjectElement)BaseGet(index);

        public void Add(ConfigurationElement project)
        {
            BaseAdd(project);
        }
    }

    [Serializable]
    public class TestProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => base["name"] as string;
            set => base["name"] = value;
        }


        [ConfigurationProperty("targetProject", IsRequired = true)]
        public string TargetProject
        {
            get => base["targetProject"] as string;
            set => base["targetProject"] = value;
        }


        [ConfigurationProperty("type", IsRequired = true)]
        public TestProjectType Type
        {
            get => (TestProjectType)base["type"];
            set => base["type"] = value;
        }
    }

    [Serializable]
    public class ProjectElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => base["name"] as string;
            set => base["name"] = value;
        }


        [ConfigurationProperty("targetSolution", IsRequired = true)]
        public string TargetSolution
        {
            get => base["targetSolution"] as string;
            set => base["targetSolution"] = value;
        }


        [ConfigurationProperty("type", IsRequired = true)]
        public ProjectType Type
        {
            get => (ProjectType)base["type"];
            set => base["type"] = value;
        }


        /// <summary>
        /// Raw value of the <c>deployment</c> attribute. Typed as a string rather than as
        /// <see cref="DeploymentMode" /> so that the casing of a hand-written config does not
        /// matter: System.Configuration parses enums case-SENSITIVELY, so a perfectly reasonable
        /// <c>deployment="package"</c> would otherwise fail with "The enumeration value must be one
        /// of the following: Assembly, Package" before the deployment even starts.
        /// Read it through <see cref="Deployment" />.
        /// </summary>
        [ConfigurationProperty("deployment", IsRequired = false, DefaultValue = "Auto")]
        internal string DeploymentValue
        {
            get => base["deployment"] as string;
            set => base["deployment"] = value;
        }

        /// <summary>
        /// How the project's assembly reaches Dataverse: as a lone signed <c>pluginassembly</c>,
        /// or wrapped in a <c>pluginpackage</c> that also carries its runtime dependencies.
        /// Defaults to <see cref="DeploymentMode.Auto" />, which reads the answer off what the
        /// build produced rather than asking for it a second time here - see
        /// <see cref="DeploymentModeResolver" />.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        /// If the attribute holds something that is neither mode.
        /// </exception>
        public DeploymentMode Deployment
        {
            get
            {
                var raw = DeploymentValue;

                // Absent attribute: every config written before 3.2, whatever the project type.
                // Auto then reads back what the build produced, which for such a project is a
                // signed assembly and no package - so the historical behaviour.
                if (string.IsNullOrWhiteSpace(raw)) return DeploymentMode.Auto;

                if (Enum.TryParse(raw.Trim(), ignoreCase: true, out DeploymentMode mode))
                    return mode;

                throw new ConfigurationErrorsException(
                    $"Project \"{Name}\": deployment=\"{raw}\" is not a valid deployment mode. " +
                    "Use \"Auto\" (the default: follow what the build produced), \"Assembly\" (the " +
                    "signed assembly is pushed on its own) or \"Package\" (a NuGet package carrying " +
                    "the assembly and its dependencies).");
            }
            set => DeploymentValue = value.ToString();
        }
    }

    public enum ProjectType
    {
        PluginsWorkflows,
        WebResources
    }

    /// <summary>
    /// Deployment shape of a plugin project, chosen by the <c>deployment</c> attribute of its
    /// <c>&lt;add&gt;</c> element in <c>xrmFramework.config</c>.
    /// </summary>
    public enum DeploymentMode
    {
        /// <summary>
        /// Follow what the build produced: a plugin package when the project was built with
        /// <c>XrmFrameworkPluginPackage</c>, a bare assembly otherwise. Keeps the choice in one
        /// place - the project - instead of asking for it again here, where the two can drift.
        /// This is the default, including for a configuration written before the attribute existed.
        /// </summary>
        Auto,

        /// <summary>
        /// The assembly is pushed on its own into the <c>pluginassembly</c> table, and the
        /// framework registers plugin types, steps, images and custom APIs itself. The assembly
        /// must be signed and cannot carry a third-party dependency.
        /// </summary>
        Assembly,

        /// <summary>
        /// A NuGet package holding the assembly and its dependencies is pushed into the
        /// <c>pluginpackage</c> table. Dataverse unpacks it and creates the
        /// <c>pluginassembly</c> and <c>plugintype</c> rows itself; the framework only
        /// registers what remains - steps, images and custom APIs. Not available on-premise,
        /// and not available for custom workflow activities.
        /// </summary>
        Package
    }

    public enum TestProjectType
    {
        Checkin,
        Quality
    }
}
