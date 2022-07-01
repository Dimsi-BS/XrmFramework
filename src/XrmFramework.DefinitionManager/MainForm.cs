// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using DefinitionManager;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using XrmFramework.Core;
using XrmFramework.DefinitionManager.Extensions;
using RelationshipAttributeDefinition = DefinitionManager.Definitions.RelationshipAttributeDefinition;

namespace XrmFramework.DefinitionManager
{
    public partial class MainForm : Form, ICustomListProvider
    {
        private readonly DefinitionCollection<EntityDefinition> _entityCollection;

        private readonly TableCollection _tables;
        private readonly List<OptionSetEnum> _enums = new();

        private readonly Type _iServiceType;

        public string CoreProjectName { get; }

        public MainForm(Type iServiceType, string coreProjectName)
        {
            _iServiceType = iServiceType;
            CoreProjectName = coreProjectName;
            CustomProvider.Instance = this;
            InitializeComponent();

            DataAccessManager.Instance.StepChanged += StepChangedHandler;

            _entityCollection = new DefinitionCollection<EntityDefinition>();
            _tables = new TableCollection();

            this.attributeListView.SelectionChanged += attributeListView_SelectionChanged;
        }

        void attributeListView_SelectionChanged(object sender, CustomListViewControl<AttributeDefinition>.SelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                if (this.splitContainer2.SplitterDistance == 100)
                {
                    this.splitContainer2.SplitterDistance = 350;
                }

                this.splitContainer2.Panel2Collapsed = e.Definition.Enum == null;
            }
        }

        void StepChangedHandler(object sender, StepChangedEventArgs e)
        {
            this.toolStripStatusLabel1.Text = e.StepName;
        }

        void RetrieveEntitiesSucceeded(object result)
        {
            var entities = (Tuple<List<EntityDefinition>, List<Table>, List<OptionSetEnum>>)result;

            _entityCollection.AddRange(entities.Item1);
            _tables.AddRange(entities.Item2);
            _enums.AddRange(entities.Item3);
            this.generateDefinitionsToolStripMenuItem.Enabled = true;
            this.entityListView.Enabled = true;
            this.attributeListView.Enabled = true;
        }

        void ConnectionSucceeded(object service)
        {
            DataAccessManager.Instance.RetrieveEntities(RetrieveEntitiesSucceeded, _entityCollection.Definitions.Select(d => d.LogicalName).ToArray());
        }

        private void DefinitionManager_Load(object sender, EventArgs e)
        {
            InitEnumDefinitions();

            _entityCollection.AttachListView(this.entityListView);

            //var entities = GetCodedEntities().ToList();
            //_tables.AddRange(entities);

            _entityCollection.AddRange(GetCodedEntityDefinitions());

            DataAccessManager.Instance.Connect(ConnectionSucceeded);
        }

        private void InitEnumDefinitions()
        {
            var optionSetDefinitionAttributeType = GetExternalType("XrmFramework.OptionSetDefinitionAttribute");
            var definitionManagerIgnoreAttributeType = GetExternalType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute");

            var definitionTypes = _iServiceType.Assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(optionSetDefinitionAttributeType, false).Any());

            foreach (var type in definitionTypes)
            {
                if (type.GetCustomAttributes(definitionManagerIgnoreAttributeType).Any())
                {
                    continue;
                }

                dynamic attribute = type.GetCustomAttribute(optionSetDefinitionAttributeType);

                var enumDefinition = new EnumDefinition
                {
                    LogicalName = (string.IsNullOrEmpty(attribute.EntityName) ? string.Empty : attribute.EntityName + "|") + attribute.LogicalName,
                    Name = type.Name,
                    IsGlobal = string.IsNullOrEmpty(attribute.EntityName)
                };

                if (type.IsEnum)
                {
                    foreach (var name in Enum.GetNames(type))
                    {
                        if (name == "Null")
                        {
                            continue;
                        }

                        var value = (int)Enum.Parse(type, name);

                        enumDefinition.Values.Add(new EnumValueDefinition
                        {
                            Name = name,
                            LogicalName = value.ToString(),
                            Value = value.ToString()
                        });
                    }
                }
                else
                {
                    foreach (var field in type.GetFields())
                    {
                        var value = (int)field.GetValue(null);

                        if (value == 0)
                        {
                            continue;
                        }

                        enumDefinition.Values.Add(new EnumValueDefinition
                        {
                            Name = field.Name,
                            LogicalName = value.ToString(),
                            Value = value.ToString()
                        });
                    }
                }

                EnumDefinitionCollection.Instance.Add(enumDefinition);
            }
        }

        private Type GetExternalType(string name)
            => _iServiceType.Assembly.GetType(name);

        //private IEnumerable<Table> GetCodedEntities()
        //{
        //    var entityFiles = Directory.EnumerateFiles(".", "*.table", SearchOption.AllDirectories);

        //    foreach (var entityFile in entityFiles)
        //    {
        //        var fileContent = File.ReadAllText(entityFile);

        //        var entity = JsonConvert.DeserializeObject<Table>(fileContent);
        //        entity.Selected = true;
        //        entity.Columns.ForEach(a => a.Selected = true);

        //        yield return entity;
        //    }
        //}

        private IEnumerable<EntityDefinition> GetCodedEntityDefinitions()
        {
            var entityDefinitionAttributeType = GetExternalType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = _iServiceType.Assembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            var relationshipAttributeType = GetExternalType("XrmFramework.RelationshipAttribute");
            var definitionManagerIgnoreAttributeType = GetExternalType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute");

            var definitionList = new List<EntityDefinition>();

            foreach (var t in definitionTypes)
            {
                if (t.GetCustomAttributes(definitionManagerIgnoreAttributeType).Any())
                {
                    continue;
                }

                var definition = new EntityDefinition
                {
                    Name = t.Name
                    ,
                    LogicalName = t.GetField("EntityName").GetValue(null) as string
                    ,
                    LogicalCollectionName = t.GetField("EntityCollectionName")?.GetValue(null) as string
                    ,
                    IsSelected = true
                };

                foreach (var field in t.GetNestedType("Columns").GetFields())
                {
                    definition.Add(new AttributeDefinition
                    {
                        LogicalName = field.GetValue(null) as string
                        ,
                        Name = field.Name
                        ,
                        IsSelected = true
                        ,
                        ParentEntity = definition
                    });
                }

                foreach (var field in t.GetFields())
                {
                    if (field.Name == "EntityName" || field.Name == "EntityCollectionName")
                    {
                        continue;
                    }

                    var typeName = field.FieldType.Name;

                    definition.AdditionalInfoCollection.Add(new AttributeDefinition
                    {
                        Type = typeName
                        ,
                        Name = field.Name
                        ,
                        LogicalName = field.Name
                        ,
                        Value = field.GetValue(null).ToString()
                        ,
                        IsSelected = true
                    });
                }

                foreach (var nestedType in t.GetNestedTypes())
                {
                    if (nestedType.Name == "Columns")
                    {
                        continue;
                    }

                    var classDefinition = new ClassDefinition
                    {
                        LogicalName = nestedType.Name
                        ,
                        Name = nestedType.Name
                        ,
                        IsEnum = nestedType.IsEnum
                    };

                    if (nestedType.IsEnum)
                    {
                        var names = Enum.GetNames(nestedType);
                        var values = Enum.GetValues(nestedType);

                        for (var i = 0; i < names.Length; i++)
                        {
                            classDefinition.Attributes.Add(new AttributeDefinition
                            {
                                LogicalName = Name = names[i]
                                ,
                                Name = names[i]
                                ,
                                Value = (int)values.GetValue(i)
                                ,
                                IsSelected = true
                            });
                        }
                    }
                    else
                    {
                        foreach (var field in nestedType.GetFields())
                        {

                            if (nestedType.Name == "ManyToOneRelationships" || nestedType.Name == "OneToManyRelationships" || nestedType.Name == "ManyToManyRelationships")
                            {
                                dynamic relationshipAttribute = field.GetCustomAttribute(relationshipAttributeType);

                                classDefinition.Attributes.Add(new RelationshipAttributeDefinition
                                {
                                    LogicalName = field.GetValue(null).ToString()
                                    ,
                                    Name = field.Name
                                    ,
                                    Type = field.FieldType.Name
                                    ,
                                    Value = field.GetValue(null).ToString()
                                    ,
                                    IsSelected = true
                                    ,
                                    NavigationPropertyName = relationshipAttribute?.NavigationPropertyName
                                    ,
                                    Role = relationshipAttribute?.Role.ToString() ?? "Referenced"
                                    ,
                                    TargetEntityName = relationshipAttribute?.TargetEntityName
                                });
                            }
                            else
                            {
                                classDefinition.Attributes.Add(new AttributeDefinition
                                {
                                    LogicalName = field.GetValue(null).ToString()
                                    ,
                                    Name = field.Name
                                    ,
                                    Type = field.FieldType.Name
                                    ,
                                    Value = field.GetValue(null).ToString()
                                    ,
                                    IsSelected = true
                                });
                            }
                        }
                    }

                    definition.AdditionalClassesCollection.Add(classDefinition);
                }

                definitionList.Add(definition);
            }

            return definitionList;
        }

        private void generateDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in this._entityCollection.SelectedDefinitions)
            {
                var sb = new IndentedStringBuilder();

                var entity = _tables.FirstOrDefault(e => e.LogicalName == item.LogicalName);

                var selectedAttributes = item.AttributesCollection.SelectedDefinitions;

                entity.Columns.RemoveAll(a => selectedAttributes.All(s => s.LogicalName != a.LogicalName));

                var enumsToKeep = entity.Columns.Where(a => !string.IsNullOrEmpty(a.EnumName))
                    .Select(en => en.EnumName).Distinct().ToList();

                entity.Enums.RemoveAll(en => !enumsToKeep.Contains(en.LogicalName));

                var entityTxt = JsonConvert.SerializeObject(entity, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                sb.AppendLine("");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.CodeDom.Compiler;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine("using XrmFramework;");
                sb.AppendLine();
                sb.AppendLine($"namespace {CoreProjectName}");
                sb.AppendLine("{");

                using (sb.Indent())
                {

                    sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
                    sb.AppendLine("[EntityDefinition]");
                    sb.AppendLine("[ExcludeFromCodeCoverage]");
                    sb.AppendLine($"public static class {item.Name}");
                    sb.AppendLine("{");

                    using (sb.Indent())
                    {

                        sb.AppendLine($"public const string EntityName = \"{item.LogicalName}\";");
                        sb.AppendLine($"public const string EntityCollectionName = \"{item.LogicalCollectionName}\";");

                        foreach (var t in item.AdditionalInfoCollection.Definitions)
                        {
                            sb.AppendLine();
                            sb.AppendLine(
                                $"public const {t.Type} {t.Name} = {(t.Type == "String" ? "\"" + (string)t.Value + "\"" : t.Value)};");
                        }

                        sb.AppendLine();
                        sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                        sb.AppendLine("public static class Columns");
                        sb.AppendLine("{");

                        using (sb.Indent())
                        {
                            foreach (var attr in item.AttributesCollection.SelectedDefinitions)
                            {
                                AddAttributeSummary(sb, attr);
                                if (!string.IsNullOrEmpty(attr.Type))
                                {
                                    sb.AppendLine($"[AttributeMetadata(AttributeTypeCode.{attr.Type})]");
                                }

                                if (attr.Enum != null)
                                {
                                    sb.AppendLine($"[OptionSet(typeof({attr.Enum.Name}))]");
                                }

                                if (attr.IsPrimaryIdAttribute)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Id)]");
                                }

                                if (attr.IsPrimaryNameAttribute)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Name)]");
                                }

                                if (attr.IsPrimaryImageAttribute)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Image)]");
                                }

                                if (attr.StringMaxLength.HasValue)
                                {
                                    sb.AppendLine($"[StringLength({attr.StringMaxLength.Value})]");
                                }

                                if (attr.MinRange.HasValue && attr.MaxRange.HasValue)
                                {
                                    sb.AppendLine($"[Range({attr.MinRange.Value}, {attr.MaxRange.Value})]");
                                }

                                foreach (var keyName in attr.KeyNames)
                                {
                                    sb.AppendLine($"[AlternateKey(AlternateKeyNames.{keyName})]");
                                }

                                if (attr.DateTimeBehavior != null)
                                {
                                    var behavior = string.Empty;
                                    if (attr.DateTimeBehavior == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.DateOnly)
                                    {
                                        behavior = "DateOnly";
                                    }
                                    else if (attr.DateTimeBehavior ==
                                             Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.TimeZoneIndependent)
                                    {
                                        behavior = "TimeZoneIndependent";
                                    }
                                    else if (attr.DateTimeBehavior ==
                                             Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.UserLocal)
                                    {
                                        behavior = "UserLocal";
                                    }

                                    sb.AppendLine($"[DateTimeBehavior(DateTimeBehavior.{behavior})]");
                                }

                                foreach (var relationship in attr.Relationships)
                                {
                                    if (this._entityCollection.SelectedDefinitions.Any(d =>
                                        d.LogicalName == relationship.ReferencedEntity))
                                    {
                                        var eC = this._entityCollection[relationship.ReferencedEntity];
                                        sb.AppendLine(string.Format(
                                            "[CrmLookup({0}.EntityName, {0}.Columns.{1}, RelationshipName = ManyToOneRelationships.{2})]",
                                            eC.Name, eC.AttributesCollection[relationship.ReferencedAttribute].Name,
                                            relationship.SchemaName));
                                    }
                                    else if (relationship.ReferencedEntity != "owner")
                                    {
                                        sb.AppendLine(
                                            $"[CrmLookup(\"{relationship.ReferencedEntity}\", \"{relationship.ReferencedAttribute}\", RelationshipName = \"{relationship.SchemaName}\")]");
                                    }
                                    else if (relationship.ReferencedEntity == "owner")
                                    {
                                        sb.AppendLine(
                                            $"[CrmLookup(\"{relationship.ReferencedEntity}\", \"{relationship.ReferencedAttribute}\", RelationshipName = \"{relationship.SchemaName}\")]");
                                    }
                                }

                                sb.AppendLine($"public const string {attr.Name} = \"{attr.LogicalName}\";\r\n");
                            }
                        }

                        sb.AppendLine("}");


                        foreach (var def in item.AdditionalClassesCollection.Definitions)
                        {
                            sb.AppendLine();
                            if (def.IsEnum)
                            {
                                sb.AppendLine($"public enum {def.Name}");
                            }
                            else
                            {
                                sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                                sb.AppendLine($"public static class {def.Name}");
                            }

                            sb.AppendLine("{");

                            using (sb.Indent())
                            {
                                if (def.IsEnum)
                                {
                                    foreach (var attr in def.Attributes.Definitions.OrderBy(a => a.Value))
                                    {
                                        sb.AppendLine($"{attr.Name} = {attr.Value},");
                                    }
                                }
                                else
                                {
                                    foreach (var attr in def.Attributes.Definitions.OrderBy(a => a.Name))
                                    {
                                        if (attr.Type == "String")
                                        {
                                            if (attr is RelationshipAttributeDefinition rAttr)
                                            {
                                                sb.Append("[Relationship(");
                                                if (this._entityCollection.SelectedDefinitions.Any(d =>
                                                    d.LogicalName == rAttr.TargetEntityName))
                                                {
                                                    sb.Append($"{_entityCollection[rAttr.TargetEntityName].Name}.EntityName");
                                                }
                                                else
                                                {
                                                    sb.Append($"\"{rAttr.TargetEntityName.EscapeQuotes()}\"");
                                                }

                                                sb.Append($", EntityRole.{rAttr.Role}, \"{rAttr.NavigationPropertyName.EscapeQuotes()}\", ");

                                                if (rAttr.Role == "Referencing")
                                                {
                                                    var ec = _entityCollection.Definitions.FirstOrDefault(d =>
                                                        d.LogicalName == item.LogicalName);
                                                    var att = ec?.AttributesCollection.SelectedDefinitions
                                                        .FirstOrDefault(
                                                            d =>
                                                                d.LogicalName == rAttr.LookupFieldName);

                                                    if (att != null)
                                                    {
                                                        sb.Append($"{ec.Name}.Columns.{att.Name}");
                                                    }
                                                    else
                                                    {
                                                        sb.Append($"\"{rAttr.LookupFieldName.EscapeQuotes()}\"");
                                                    }
                                                }
                                                else
                                                {
                                                    var ec = _entityCollection.SelectedDefinitions.FirstOrDefault(d =>
                                                        d.LogicalName == rAttr.TargetEntityName);
                                                    var att = ec?.AttributesCollection.SelectedDefinitions
                                                        .FirstOrDefault(
                                                            d =>
                                                                d.LogicalName == rAttr.LookupFieldName);

                                                    if (att != null)
                                                    {
                                                        sb.Append($"{ec.Name}.Columns.{att.Name}");
                                                    }
                                                    else
                                                    {
                                                        sb.Append($"\"{rAttr.LookupFieldName}\"");
                                                    }
                                                }

                                                sb.AppendLine(")]");
                                            }

                                            sb.AppendLine($"public const string {attr.Name} = \"{attr.Value}\";");
                                        }
                                        else
                                        {
                                            sb.AppendLine($"public const int {attr.Name} = {attr.Value};");
                                        }
                                    }
                                }
                            }

                            sb.AppendLine("}");
                        }
                    }

                    sb.AppendLine("}");
                }

                sb.AppendLine("}");

                var fileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{item.Name}.cs");

                var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
                if (definitionFolder.Exists == false)
                {
                    definitionFolder.Create();
                }

                File.WriteAllText(fileInfo.FullName, sb.ToString());

                var fileInfo2 = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{item.Name.Replace("Definition", string.Empty)}.table");

                File.WriteAllText(fileInfo2.FullName, entityTxt);
            }



            var globalEnums = EnumDefinitionCollection.Instance.SelectedDefinitions.Where(en => en.IsSelected)
                .Select(en => en.LogicalName).ToList();

            var globalOptionSets = new Table
            {
                LogicalName = "globalEnums",
                Name = "OptionSets"
            };
            globalOptionSets.Enums.AddRange(_enums.Where(en => globalEnums.Contains(en.LogicalName) && en.IsGlobal));

            var optionSetsTxt = JsonConvert.SerializeObject(globalOptionSets, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var fileInfoOptionSets = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{globalOptionSets.Name}.table");

            File.WriteAllText(fileInfoOptionSets.FullName, optionSetsTxt);

            var fc = new IndentedStringBuilder();
            fc.AppendLine("using System.ComponentModel;");
            fc.AppendLine("using XrmFramework;");
            fc.AppendLine();
            fc.AppendLine($"namespace {CoreProjectName}");
            fc.AppendLine("{");

            using (fc.Indent())
            {
                foreach (var def in EnumDefinitionCollection.Instance.SelectedDefinitions)
                {
                    fc.AppendLine();
                    if (def.IsGlobal)
                    {
                        fc.AppendLine($"[OptionSetDefinition(\"{def.LogicalName}\")]");
                    }
                    else
                    {
                        var attribute = def.ReferencedBy.First();

                        if (!_entityCollection.SelectedDefinitions.Any(s => s.LogicalName == attribute.ParentEntity.LogicalName))
                        {
                            continue;
                        }

                        fc.AppendLine(string.Format("[OptionSetDefinition({0}.EntityName, {0}.Columns.{1})]",
                            attribute.ParentEntity.Name, attribute.Name));
                    }

                    fc.AppendLine($"public enum {def.Name}");
                    fc.AppendLine("{");

                    using (fc.Indent())
                    {

                        if (def.HasNullValue)
                        {
                            fc.AppendLine("Null = 0,");
                        }

                        foreach (var val in def.Values.Definitions)
                        {
                            fc.AppendLine($"[Description(\"{val.DisplayName}\")]");

                            if (!string.IsNullOrEmpty(val.ExternalValue))
                            {
                                fc.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
                            }

                            fc.AppendLine($"{val.Name} = {val.LogicalName},");
                        }
                    }

                    fc.AppendLine("}");
                }
            }

            fc.AppendLine("}");
            File.WriteAllText($"../../../../../{CoreProjectName}/Definitions/OptionSetDefinitions.cs", fc.ToString());

            MessageBox.Show(@"Definition files generation succeeded");
        }

        private void AddAttributeSummary(IndentedStringBuilder sb, AttributeDefinition attr)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// ");
            sb.AppendLine($"/// Type : {attr.Type}{(attr.Enum == null ? "" : " (" + attr.Enum.Name + ")")}");
            sb.Append("/// Validity :  ");

            var isFirst = true;
            if (attr.IsValidForRead)
            {
                isFirst = false;
                sb.Append("Read ");
            }

            if (attr.IsValidForCreate)
            {
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("Create ");
            }

            if (attr.IsValidForUpdate)
            {
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("Update ");
            }

            if (attr.IsValidForAdvancedFind)
            {
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("AdvancedFind ");
            }
            sb.AppendLine();

            sb.AppendLine("/// </summary>");
        }

        public T GetCustomList<T>()
        {
            var result = GetCustomList(typeof(T));

            return (T)result;
        }


        public object GetCustomList(Type type)
        {
            foreach (var field in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (type.IsAssignableFrom(field.FieldType))
                {
                    return field.GetValue(this);
                }
            }
            return null;
        }
    }
}
