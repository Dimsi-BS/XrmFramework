// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.BindingModel
{
    [TestFixture]
    public class BindingModelBaseTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Concrete helpers
        // ─────────────────────────────────────────────────────────────

        private class SimpleModel : BindingModelBase
        {
            private string _name;
            public string Name
            {
                get => _name;
                set { _name = value; OnPropertyChanged(); }
            }

            private int _age;
            public int Age
            {
                get => _age;
                set { _age = value; OnPropertyChanged(); }
            }
        }

        private class ModelWithDependent : BindingModelBase
        {
            private string _firstName;

            [Dependent(nameof(FullName))]
            public string FirstName
            {
                get => _firstName;
                set { _firstName = value; OnPropertyChanged(); }
            }

            public string FullName => FirstName;
        }

        // ─────────────────────────────────────────────────────────────
        //  InitializedProperties
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void InitializedProperties_FreshInstance_IsEmpty()
        {
            var model = new SimpleModel();

            Assert.IsEmpty(model.InitializedProperties);
        }

        [Test]
        public void InitializedProperties_AfterSettingProperty_ContainsPropertyName()
        {
            var model = new SimpleModel { Name = "Alice" };

            CollectionAssert.Contains(model.InitializedProperties, nameof(SimpleModel.Name));
        }

        [Test]
        public void InitializedProperties_AfterSettingTwoProperties_ContainsBoth()
        {
            var model = new SimpleModel { Name = "Alice", Age = 30 };

            CollectionAssert.Contains(model.InitializedProperties, nameof(SimpleModel.Name));
            CollectionAssert.Contains(model.InitializedProperties, nameof(SimpleModel.Age));
            Assert.AreEqual(2, model.InitializedProperties.Count);
        }

        [Test]
        public void InitializedProperties_SettingSamePropertyTwice_CountsOnce()
        {
            var model = new SimpleModel();
            model.Name = "Alice";
            model.Name = "Bob";

            Assert.AreEqual(1, model.InitializedProperties.Count);
            CollectionAssert.Contains(model.InitializedProperties, nameof(SimpleModel.Name));
        }

        // ─────────────────────────────────────────────────────────────
        //  ClearInitializedProperties
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void ClearInitializedProperties_AfterSetting_EmptiesCollection()
        {
            var model = new SimpleModel { Name = "Alice", Age = 30 };

            model.ClearInitializedProperties();

            Assert.IsEmpty(model.InitializedProperties);
        }

        [Test]
        public void ClearInitializedProperties_CanSetPropertiesAgainAfterClear()
        {
            var model = new SimpleModel { Name = "Alice" };
            model.ClearInitializedProperties();
            model.Age = 25;

            CollectionAssert.DoesNotContain(model.InitializedProperties, nameof(SimpleModel.Name));
            CollectionAssert.Contains(model.InitializedProperties, nameof(SimpleModel.Age));
        }

        // ─────────────────────────────────────────────────────────────
        //  DependentAttribute cascade
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void DependentAttribute_SettingSourceProperty_AlsoTracksDependent()
        {
            var model = new ModelWithDependent { FirstName = "Alice" };

            CollectionAssert.Contains(model.InitializedProperties, nameof(ModelWithDependent.FirstName));
            CollectionAssert.Contains(model.InitializedProperties, nameof(ModelWithDependent.FullName));
        }

        [Test]
        public void DependentAttribute_FullName_CascadesFromFirstName()
        {
            var model = new ModelWithDependent();
            var events = new System.Collections.Generic.List<string>();
            model.PropertyChanged += (_, e) => events.Add(e.PropertyName);

            model.FirstName = "Bob";

            CollectionAssert.Contains(events, nameof(ModelWithDependent.FirstName));
            CollectionAssert.Contains(events, nameof(ModelWithDependent.FullName));
        }

        // ─────────────────────────────────────────────────────────────
        //  Id
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Id_DefaultIsEmptyGuid()
        {
            var model = new SimpleModel();

            Assert.AreEqual(System.Guid.Empty, model.Id);
        }

        [Test]
        public void Id_CanBeSet()
        {
            var id = System.Guid.NewGuid();
            var model = new SimpleModel { Id = id };

            Assert.AreEqual(id, model.Id);
        }
    }
}
