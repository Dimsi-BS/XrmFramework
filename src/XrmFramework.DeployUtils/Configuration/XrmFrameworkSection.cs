// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Configuration;

namespace XrmFramework.DeployUtils.Configuration
{
    [Serializable]
    public class XrmFrameworkSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty PropProjects = new("projects", typeof(ProjectCollection), null, ConfigurationPropertyOptions.IsDefaultCollection | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty EntityProperty = new("entitySolution", typeof(EntitySolutionElement), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty SelectedConnectionProperty = new("selectedConnection", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationPropertyCollection InnerProperties = new()
        {
            PropProjects, EntityProperty, SelectedConnectionProperty
        };
        
        [ConfigurationProperty("entitySolution", IsRequired = true)]
        public EntitySolutionElement EntitySolution
        {
            get => (EntitySolutionElement)base[EntityProperty];
            set => base[EntityProperty] = value;
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
        public ProjectCollection Projects => (ProjectCollection)base[PropProjects];

        protected override ConfigurationPropertyCollection Properties => InnerProperties;
    }

    [Serializable]
    public class EntitySolutionElement : ConfigurationElement
    {
        private static readonly ConfigurationProperty NameProperty = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationPropertyCollection InnerProperties = new () { NameProperty };
        
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get => (string)this[NameProperty];
            set => this[NameProperty] = value;
        }

        protected override ConfigurationPropertyCollection Properties => InnerProperties;
    }

    [Serializable]
    public class ProjectCollection : ConfigurationElementCollection
    {
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
