using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.BindingModel;
using XrmFramework.Definitions;

namespace XrmFramework.Tests;

[TestClass]
public class BindingModelExtensionsTests
{
    private TestBindingModel _sourceModel = null!;
    private TestBindingModel _targetModel = null!;

    [TestInitialize]
    public void InitTests()
    {
        _sourceModel = new TestBindingModel
        {
            Id = Guid.NewGuid(),
            Name = "Source Name",
            Age = 30,
            Email = "source@test.com",
            CreatedDate = DateTime.UtcNow
        };

        _targetModel = new TestBindingModel
        {
            Id = _sourceModel.Id,
            Name = "Target Name",
            Age = 30,
            Email = "target@test.com",
            CreatedDate = DateTime.UtcNow.AddDays(-1)
        };
        
        DefinitionCache.RegisterDefinitionsAssembly(GetType().Assembly);
    }

    #region GetDiffGeneric Tests

    [TestMethod]
    public void GetDiffGeneric_TargetIsNull_ReturnsSource()
    {
        var result = _sourceModel.GetDiffGeneric<TestBindingModel>(null);

        Assert.IsNotNull(result);
        Assert.AreEqual(_sourceModel.Name, result.Name);
        Assert.AreEqual(_sourceModel.Age, result.Age);
    }

    [TestMethod]
    public void GetDiffGeneric_NoChanges_ReturnsEmptyModel()
    {
        var source = new TestBindingModel
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Age = 25
        };
        var target = new TestBindingModel
        {
            Id = source.Id,
            Name = "Test",
            Age = 25
        };

        var result = source.GetDiffGeneric(target);

        Assert.IsNotNull(result);
        Assert.AreEqual(source.Id, result.Id);
        // Only properties with differences should be set
    }

    [TestMethod]
    public void GetDiffGeneric_WithChanges_ReturnsDifferences()
    {
        var result = _sourceModel.GetDiffGeneric(_targetModel);

        Assert.IsNotNull(result);
        Assert.AreEqual(_sourceModel.Id, result.Id);
    }

    [TestMethod]
    public void GetDiffGeneric_TargetIdEmpty_UsesSourceId()
    {
        _targetModel.Id = Guid.Empty;

        var result = _sourceModel.GetDiffGeneric(_targetModel);

        Assert.AreEqual(_sourceModel.Id, result.Id);
    }

    [TestMethod]
    public void GetDiffGeneric_SourceIdEmpty_UsesTargetId()
    {
        var sourceId = _sourceModel.Id;
        _sourceModel.Id = Guid.Empty;

        var result = _sourceModel.GetDiffGeneric(_targetModel);

        Assert.AreEqual(_targetModel.Id, result.Id);
    }

    #endregion

    #region GetDiffGeneric List Tests

    [TestMethod]
    public void GetDiffGenericList_EmptyLists_ReturnsEmpty()
    {
        var sourceList = new List<TestBindingModel>();
        var targetList = new List<TestBindingModel>();

        var result = sourceList.GetDiffGeneric(targetList).ToList();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetDiffGenericList_NewItems_ReturnsAllNew()
    {
        var sourceList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = Guid.NewGuid(), Name = "Item1", Age = 20 },
            new TestBindingModel { Id = Guid.NewGuid(), Name = "Item2", Age = 30 }
        };
        var targetList = new List<TestBindingModel>();

        var result = sourceList.GetDiffGeneric(targetList).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetDiffGenericList_UpdatedItems_ReturnsUpdated()
    {
        var id = Guid.NewGuid();
        var sourceList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = id, Name = "Updated Name", Age = 25 }
        };
        var targetList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = id, Name = "Original Name", Age = 25 }
        };

        var result = sourceList.GetDiffGeneric(targetList).ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(id, result[0].Id);
    }

    [TestMethod]
    public void GetDiffGenericList_MixedChanges_ReturnsCorrectDiff()
    {
        var existingId = Guid.NewGuid();
        var newId = Guid.NewGuid();

        var sourceList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = existingId, Name = "Updated", Age = 30 },
            new TestBindingModel { Id = newId, Name = "New Item", Age = 25 }
        };
        var targetList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = existingId, Name = "Original", Age = 30 }
        };

        var result = sourceList.GetDiffGeneric(targetList).ToList();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetDiffGenericList_WithCustomComparer_UsesComparer()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModel));
        var sourceList = new List<TestBindingModel>
        {
            new TestBindingModel { Id = Guid.NewGuid(), Name = "Test" }
        };
        var targetList = new List<TestBindingModel>();

        var result = sourceList.GetDiffGeneric(targetList, comparer).ToList();

        Assert.AreEqual(1, result.Count);
    }

    #endregion

    #region CopyField Tests

    [TestMethod]
    public void CopyField_ByPropertyName_CopiesValue()
    {
        var input = new TestBindingModel { Name = "Test Name" };
        var output = new TestBindingModel();

        input.CopyField(output, nameof(TestBindingModel.Name), nameof(TestBindingModel.Name));

        Assert.AreEqual("Test Name", output.Name);
    }

    [TestMethod]
    public void CopyField_PropertyNotInitialized_DoesNotCopy()
    {
        var input = new TestBindingModel();
        var output = new TestBindingModel { Name = "Original" };

        // Name was not explicitly set in input, so it's not in InitializedProperties
        input.CopyField(output, nameof(TestBindingModel.Name), nameof(TestBindingModel.Name));

        Assert.AreEqual("Original", output.Name);
    }

    [TestMethod]
    public void CopyField_ByExpression_CopiesValue()
    {
        var input = new TestBindingModel { Age = 42 };
        var output = new TestBindingModel();

        input.CopyField(output, i => i.Age, o => o.Age);

        Assert.AreEqual(42, output.Age);
    }

    [TestMethod]
    public void CopyField_NullSourceProperty_DoesNotThrow()
    {
        var input = new TestBindingModel();
        var output = new TestBindingModel();

        input.CopyField(output, "NonExistentProperty", nameof(TestBindingModel.Name));

        // Should not throw
        Assert.IsNotNull(output);
    }

    [TestMethod]
    public void CopyField_NullTargetProperty_DoesNotThrow()
    {
        var input = new TestBindingModel { Name = "Test" };
        var output = new TestBindingModel();

        input.CopyField(output, nameof(TestBindingModel.Name), "NonExistentProperty");

        // Should not throw
        Assert.IsNotNull(output);
    }

    #endregion
}

[TestClass]
public class KeyEqualityComparerTests
{
    [TestInitialize]
    public void InitTests()
    {
        DefinitionCache.RegisterDefinitionsAssembly(GetType().Assembly);
    }

    [TestMethod]
    public void Equals_SameKeyValues_ReturnsTrue()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModel));
        var id = Guid.NewGuid();
        var model1 = new TestBindingModel { Id = id, Name = "Name1" };
        var model2 = new TestBindingModel { Id = id, Name = "Name2" };

        var result = comparer.Equals(model1, model2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_DifferentKeyValues_ReturnsFalse()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModel));
        var model1 = new TestBindingModel { Id = Guid.NewGuid(), Name = "Name1" };
        var model2 = new TestBindingModel { Id = Guid.NewGuid(), Name = "Name1" };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_StringProperties_CaseInsensitive()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModelWithStringKey));
        var model1 = new TestBindingModelWithStringKey { KeyField = "TEST" };
        var model2 = new TestBindingModelWithStringKey { KeyField = "test" };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_ReturnsConsistentValue()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModel));
        var model = new TestBindingModel { Id = Guid.NewGuid(), Name = "Test" };

        var hash1 = comparer.GetHashCode(model);
        var hash2 = comparer.GetHashCode(model);

        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_NullValues_ReturnsZero()
    {
        var comparer = new KeyEqualityComparer(typeof(TestBindingModel));
        var model = new TestBindingModel();

        var hash = comparer.GetHashCode(model);

        Assert.AreEqual(0, hash);
    }
}

[TestClass]
public class MultipleEqualityComparerTests
{
    [TestMethod]
    public void Constructor_WithPropertyNames_InitializesCorrectly()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name), nameof(TestBindingModel.Age));

        Assert.IsNotNull(comparer);
    }

    [TestMethod]
    public void Equals_SamePropertyValues_ReturnsTrue()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name), nameof(TestBindingModel.Age));
        var model1 = new TestBindingModel { Name = "Test", Age = 25 };
        var model2 = new TestBindingModel { Name = "Test", Age = 25 };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentPropertyValues_ReturnsFalse()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name), nameof(TestBindingModel.Age));
        var model1 = new TestBindingModel { Name = "Test1", Age = 25 };
        var model2 = new TestBindingModel { Name = "Test2", Age = 25 };

        var result = comparer.Equals(model1, model2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_StringProperty_CaseInsensitive()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name));
        var model1 = new TestBindingModel { Name = "TEST" };
        var model2 = new TestBindingModel { Name = "test" };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_BothNull_ReturnsTrue()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Email));
        var model1 = new TestBindingModel { Email = null };
        var model2 = new TestBindingModel { Email = null };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_ReturnsConsistentValue()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name));
        var model = new TestBindingModel { Name = "Test" };

        var hash1 = comparer.GetHashCode(model);
        var hash2 = comparer.GetHashCode(model);

        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void GetHashCode_StringProperty_CaseInsensitive()
    {
        var comparer = new MultipleEqualityComparer<TestBindingModel>(nameof(TestBindingModel.Name));
        var model1 = new TestBindingModel { Name = "TEST" };
        var model2 = new TestBindingModel { Name = "test" };

        var hash1 = comparer.GetHashCode(model1);
        var hash2 = comparer.GetHashCode(model2);

        Assert.AreEqual(hash1, hash2);
    }
}

[TestClass]
public class ModelEqualityComparerTests
{
    [TestMethod]
    public void Equals_SameId_ReturnsTrue()
    {
        var comparer = new ModelEqualityComparer<TestBindingModel>();
        var id = Guid.NewGuid();
        var model1 = new TestBindingModel { Id = id };
        var model2 = new TestBindingModel { Id = id };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var comparer = new ModelEqualityComparer<TestBindingModel>();
        var model1 = new TestBindingModel { Id = Guid.NewGuid() };
        var model2 = new TestBindingModel { Id = Guid.NewGuid() };

        var result = comparer.Equals(model1, model2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_BothNull_ReturnsTrue()
    {
        var comparer = new ModelEqualityComparer<TestBindingModel>();

        var result = comparer.Equals(null, null);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_OneNull_ReturnsFalse()
    {
        var comparer = new ModelEqualityComparer<TestBindingModel>();
        var model = new TestBindingModel { Id = Guid.NewGuid() };

        var result = comparer.Equals(model, null);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_ReturnsIdHashCode()
    {
        var comparer = new ModelEqualityComparer<TestBindingModel>();
        var id = Guid.NewGuid();
        var model = new TestBindingModel { Id = id };

        var hash = comparer.GetHashCode(model);

        Assert.AreEqual(id.GetHashCode(), hash);
    }
}

[TestClass]
public class DeepModelEqualityComparerTests
{
    [TestMethod]
    public void Constructor_ValidType_InitializesCorrectly()
    {
        var comparer = new DeepModelEqualityComparer(typeof(TestBindingModel));

        Assert.IsNotNull(comparer);
    }

    [TestMethod]
    public void Equals_AllPropertiesSame_ReturnsTrue()
    {
        var comparer = new DeepModelEqualityComparer(typeof(TestBindingModel));
        var model1 = new TestBindingModel { Id = Guid.NewGuid(), Name = "Test", Age = 25 };
        var model2 = new TestBindingModel { Id = model1.Id, Name = "Test", Age = 25 };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentProperties_ReturnsFalse()
    {
        var comparer = new DeepModelEqualityComparer(typeof(TestBindingModel));
        var model1 = new TestBindingModel { Id = Guid.NewGuid(), Name = "Test1", Age = 25 };
        var model2 = new TestBindingModel { Id = model1.Id, Name = "Test2", Age = 25 };

        var result = comparer.Equals(model1, model2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_StringProperties_CaseInsensitive()
    {
        var comparer = new DeepModelEqualityComparer(typeof(TestBindingModel));
        var model1 = new TestBindingModel { Name = "TEST" };
        var model2 = new TestBindingModel { Name = "test" };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetHashCode_ReturnsConsistentValue()
    {
        var comparer = new DeepModelEqualityComparer(typeof(TestBindingModel));
        var model = new TestBindingModel { Id = Guid.NewGuid(), Name = "Test" };

        var hash1 = comparer.GetHashCode(model);
        var hash2 = comparer.GetHashCode(model);

        Assert.AreEqual(hash1, hash2);
    }
}

[TestClass]
public class DeepModelEqualityComparerGenericTests
{
    [TestMethod]
    public void Constructor_InitializesProperties()
    {
        var comparer = new DeepModelEqualityComparer<TestBindingModel>();

        Assert.IsNotNull(comparer);
    }

    [TestMethod]
    public void Equals_AllPropertiesSame_ReturnsTrue()
    {
        var comparer = new DeepModelEqualityComparer<TestBindingModel>();
        var id = Guid.NewGuid();
        var model1 = new TestBindingModel { Id = id, Name = "Test", Age = 25 };
        var model2 = new TestBindingModel { Id = id, Name = "Test", Age = 25 };

        var result = comparer.Equals(model1, model2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_DifferentProperties_ReturnsFalse()
    {
        var comparer = new DeepModelEqualityComparer<TestBindingModel>();
        var model1 = new TestBindingModel { Name = "Test1" };
        var model2 = new TestBindingModel { Name = "Test2" };

        var result = comparer.Equals(model1, model2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_ReturnsConsistentValue()
    {
        var comparer = new DeepModelEqualityComparer<TestBindingModel>();
        var model = new TestBindingModel { Id = Guid.NewGuid(), Name = "Test" };

        var hash1 = comparer.GetHashCode(model);
        var hash2 = comparer.GetHashCode(model);

        Assert.AreEqual(hash1, hash2);
    }
}

// Helper test classes
[CrmEntity(DummyDefinition.EntityName)]
public class TestBindingModel : BindingModelBase
{
    private int _age;
    private string _name;
    private string _email;
    private DateTime? _createdDate;


    [CrmMapping(DummyDefinition.Columns.FullName, IsValidForUpdate = true)]
    [AlternateKey("name")]
    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    [CrmMapping(DummyDefinition.Columns.Age, IsValidForUpdate = true)]
    public int Age
    {
        get => _age;
        set
        {
            if (value == _age) return;
            _age = value;
            OnPropertyChanged();
            OnPropertyChanged();
        }
    }

    [CrmMapping(DummyDefinition.Columns.InternalEMailAddress, IsValidForUpdate = true)]
    public string Email
    {
        get => _email;
        set
        {
            if (value == _email) return;
            _email = value;
            OnPropertyChanged();
        }
    }

    [CrmMapping(DummyDefinition.Columns.CreatedOn, IsValidForUpdate = true)]
    public DateTime? CreatedDate
    {
        get => _createdDate;
        set
        {
            if (Nullable.Equals(value, _createdDate)) return;
            _createdDate = value;
            OnPropertyChanged();
        }
    }
}

[CrmEntity(FooDefinition.EntityName)]
public class TestBindingModelWithStringKey : BindingModelBase
{
    private string _keyField;

    [CrmMapping(FooDefinition.Columns.Name)]
    [AlternateKey("key")]
    public string KeyField
    {
        get => _keyField;
        set
        {
            _keyField = value;
            OnPropertyChanged();
        }
    }
}


[EntityDefinition]
public static class DummyDefinition
{
    public const string EntityName = "dummy";
    public const string EntityCollectionName = "dummies";

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    public static class Columns
    {
        [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
        [PrimaryAttribute(PrimaryAttributeType.Id)]
        public const String Id = "id";
        
        [AttributeMetadata(AttributeTypeCode.Lookup)]
        [CrmLookup(FooDefinition.EntityName, FooDefinition.Columns.Id,
            RelationshipName = ManyToOneRelationships.business_unit_system_users)]
        public const string BusinessUnitId = "businessunitid";

        [AttributeMetadata(AttributeTypeCode.String)]
        [PrimaryAttribute(PrimaryAttributeType.Name)]
        [StringLength(200)]
        public const string FullName = "fullname";

        [AttributeMetadata(AttributeTypeCode.String)] 
        [StringLength(100)]
        public const string InternalEMailAddress = "internalemailaddress";

        [AttributeMetadata(AttributeTypeCode.Integer)]
        public const string Age = "age";
        
        [AttributeMetadata(AttributeTypeCode.DateTime)]
        [DateTimeBehavior(DateTimeBehavior.UserLocal)]
        public const string CreatedOn = "createdon";
        
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    public static class ManyToOneRelationships
    {
        [Relationship("businessunit", EntityRole.Referencing, "businessunitid", "businessunitid")]
        public const string business_unit_system_users = "business_unit_system_users";
    }
}

[EntityDefinition]
public static class FooDefinition
{
    public const string EntityName = "foo";
    public const string EntityCollectionName = "foos";

    public static class Columns
    {
        [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
        [PrimaryAttribute(PrimaryAttributeType.Id)]
        public const string Id = "id";
        
        [AttributeMetadata(AttributeTypeCode.String)]
        public const string Name = "name";
    }
}
