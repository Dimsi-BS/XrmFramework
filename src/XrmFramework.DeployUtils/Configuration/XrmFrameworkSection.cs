// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

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
    }

    public enum ProjectType
    {
        PluginsWorkflows,
        WebResources
    }

    public enum TestProjectType
    {
        Checkin,
        Quality
    }
}
