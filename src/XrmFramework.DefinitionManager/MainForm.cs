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
using DefinitionManager;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using XrmFramework.Core;

using RelationshipAttributeDefinition = DefinitionManager.Definitions.RelationshipAttributeDefinition;

namespace XrmFramework.DefinitionManager
{
    public partial class MainForm : Form, ICustomListProvider
    {
        private readonly DefinitionCollection<EntityDefinition> _entityCollection;

        private readonly TableCollection _localTables;
        private readonly TableCollection _tables;
        private readonly TableCollection _selectedTables;
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
            _selectedTables = new TableCollection();
            _localTables = new TableCollection();

            this.attributeListView.SelectionChanged += attributeListView_SelectionChanged;




            //_selectedTables = LoadLocalTables();
            _selectedTables = new TableCollection();
            _localTables = LoadLocalTables();
            
           // _localTables.AddRange(GetTablesFromFormerDefinitionCode());
            //_selectedTables.AddRange(GetTablesFromFormerDefinitionCode());

            foreach(var table in _localTables)
            {
                var localEntity = TableToBaseEntityDefinition(table);
                _entityCollection.Add(localEntity);
                _tables.Add(table);
            }
            //MessageBox.Show($"There are currently {_selectedTables.Count} tables selected in this project.");


            this.generateDefinitionsToolStripMenuItem.Enabled = true;
            this.entityListView.Enabled = true;
            this.attributeListView.Enabled = true;

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
            MessageBox.Show($"There are {_localTables.Count} local tables");
            //MergeLocalTablesWithCrmData(_localTables);
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
            List<EntityDefinition> localCodedDefinitions = new List<EntityDefinition>();
            _entityCollection.AttachListView(this.entityListView);

            //var entities = GetCodedEntities().ToList();
            //_tables.AddRange(entities);

            //
            //localCodedDefinitions = GetCodedEntityDefinitions();

           //var localTables = LoadLocalTables();
           //foreach(var table in localTables)
           //{
           //    localCodedDefinitions.Add(TableToBaseEntityDefinition(table));
           //}
            //_entityCollection.AddRange(localCodedDefinitions);

            //DataAccessManager.Instance.Connect(ConnectionSucceeded);
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
            var definitionList = new List<EntityDefinition>();
            return definitionList;
            //Console.WriteLine(_tables.Count);
            var entityDefinitionAttributeType = GetExternalType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = _iServiceType.Assembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            var relationshipAttributeType = GetExternalType("XrmFramework.RelationshipAttribute");
            var definitionManagerIgnoreAttributeType = GetExternalType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute");

            

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
            //GenerateDefinitionCodeFromJson();
            // Add new selected tables
            foreach(var item in _entityCollection.SelectedDefinitions)
            {
                if(!_selectedTables.Any(t=>t.LogicalName == item.LogicalName))
                {
                    var table = _tables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                    table.Selected = true;
                    foreach (var a in item.AttributesCollection.SelectedDefinitions)
                    {
                        table.Columns.FirstOrDefault(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;
                    }

                    _selectedTables.Add(table);

                }
                else
                {
                    var tableToRemove = _selectedTables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                    _selectedTables.Remove(tableToRemove);

                    var table = _tables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                    if(table == null)
                    {
                        throw new Exception($"Table named {item.LogicalName} does not exist in the list _tables, _tables.Count is {_tables.Count}");
                    }
                    table.Selected = true;
                    foreach (var a in item.AttributesCollection.SelectedDefinitions)
                    {
                        table.Columns.FirstOrDefault(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;
                    }

                    _selectedTables.Add(table);

                }
            }

            var nameOfTablesToDelete = new List<String>();
            // Delete tables that aren't selected
            foreach(var table in _selectedTables)
            {
                if (!_entityCollection.SelectedDefinitions.Any(e => e.LogicalName == table.LogicalName))
                {
                    nameOfTablesToDelete.Add(table.LogicalName);
                }
            }

            foreach(var name in nameOfTablesToDelete)
            {
                _selectedTables.Remove(_selectedTables.FirstOrDefault(t => t.LogicalName == name));
            }

                foreach (var item in this._entityCollection.SelectedDefinitions)
            {
                var sb = new IndentedStringBuilder();

                //var entity = _tables.FirstOrDefault(e => e.LogicalName == item.LogicalName);
                //var selectedAttributes = item.AttributesCollection.SelectedDefinitions;

                //entity.Columns.RemoveAll(a => selectedAttributes.All(s => s.LogicalName != a.LogicalName));

                //var enumsToKeep = entity.Columns.Where(a => !string.IsNullOrEmpty(a.EnumName))
                //    .Select(en => en.EnumName).Distinct().ToList();

                //entity.Enums.RemoveAll(en => !enumsToKeep.Contains(en.LogicalName));

                //var entityTxt = JsonConvert.SerializeObject(entity, Formatting.Indented, new JsonSerializerSettings
                //{
                //    DefaultValueHandling = DefaultValueHandling.Ignore
                //});

                /*
                
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
                                                    sb.Append($"\"{rAttr.TargetEntityName}\"");
                                                }

                                                sb.Append($", EntityRole.{rAttr.Role}, \"{rAttr.NavigationPropertyName}\", ");

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
                                                        sb.Append($"\"{rAttr.LookupFieldName}\"");
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


                */


               //var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
               //if (definitionFolder.Exists == false)
               //{
               //    definitionFolder.Create();
               //}
               //
               ////File.WriteAllText(fileInfo.FullName, sb.ToString());
               //
               //var fileInfo2 = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{item.Name.Replace("Definition", string.Empty)}.table");

                //File.WriteAllText(fileInfo2.FullName, entityTxt);
            }




            var globalEnums = EnumDefinitionCollection.Instance.SelectedDefinitions.Where(en => en.IsSelected)
                .Select(en => en.LogicalName).ToList();

            var globalOptionSets = new Table
            {
                LogicalName = "globalEnums",
                Name = "OptionSet"
            };
            globalOptionSets.Enums.AddRange(_enums.Where(en => globalEnums.Contains(en.LogicalName) && en.IsGlobal));

            var optionSetsTxt = JsonConvert.SerializeObject(globalOptionSets, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var fileInfoOptionSets = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{globalOptionSets.Name}.table");

            File.WriteAllText(fileInfoOptionSets.FullName, optionSetsTxt);

            /*
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
            */
            foreach(var table in _selectedTables)
            {
                //MessageBox.Show(table.Name);

                
                //table.Columns.RemoveNonSelectedColumns();


                // To be deleted
                //table.isLocked = true;
                //foreach (var column in table.Columns)
                //{
                //    column.IsLocked = true;
                //}
                //foreach(var en in table.Enums)
                //{
                //    en.IsLocked = true;
                //}



                var serializedTable = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                JObject test = JObject.Parse(serializedTable);

                //MessageBox.Show(test["LogName"].ToString());
                //MessageBox.Show(test.ToString());


                

                var fileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{table.Name}.table");
                
                var definitionFolderForEnums = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
                if (definitionFolderForEnums.Exists == false)
                {
                    definitionFolderForEnums.Create();
                }
                
                
                File.WriteAllText(fileInfo.FullName, serializedTable);
                
            }

            var globalSelectedEnumDefinitions = EnumDefinitionCollection.Instance.SelectedDefinitions.Where(en => en.IsSelected && en.IsGlobal)
             .Select(en => en.LogicalName).ToList();
            List<OptionSetEnum> globalSelectedEnums = new List<OptionSetEnum>();
            globalSelectedEnums.AddRange(_enums.Where(en => globalSelectedEnumDefinitions.Contains(en.LogicalName) && en.IsGlobal));

            var optionSetTable = new Table
            {
                LogicalName = "globalEnums",
                Name = "OptionSets"
            };
            globalOptionSets.Enums.AddRange(_enums.Where(en => globalEnums.Contains(en.LogicalName) && en.IsGlobal));


            //if (globalSelectedEnums.Any())
            //{
            //
            //    string serializedEnums;
            //    serializedEnums = JsonConvert.SerializeObject(globalSelectedEnums, Formatting.Indented, new JsonSerializerSettings
            //    {
            //        DefaultValueHandling = DefaultValueHandling.Ignore
            //    });
            //
            //    var enumFileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/OptionSet.table");
            //
            //    var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
            //    if (definitionFolder.Exists == false)
            //    {
            //        definitionFolder.Create();
            //    }
            //
            //
            //    File.WriteAllText(enumFileInfo.FullName, serializedEnums);
            //}

            var serializedGlobalEnums = JsonConvert.SerializeObject(globalOptionSets, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var enumFileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/OptionSet.table");

            var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
            if (definitionFolder.Exists == false)
            {
                definitionFolder.Create();
            }


            File.WriteAllText(enumFileInfo.FullName, serializedGlobalEnums);

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

        private void AddColumnSummary(IndentedStringBuilder sb, Column col)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// ");
            sb.AppendLine($"/// Type : {(AttributeTypeCode)col.Type}{(col.EnumName == null ? "" : " (" + col.EnumName + ")")}");
            sb.Append("/// Validity :  ");

            var isFirst = true;
            if((col.Capabilities & AttributeCapabilities.Read) != AttributeCapabilities.None)
            {
                isFirst = false;
                sb.Append("Read ");
            }

            if ((col.Capabilities & AttributeCapabilities.Create) != AttributeCapabilities.None)
            {
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("Create ");
            }

            if ((col.Capabilities & AttributeCapabilities.Update) != AttributeCapabilities.None)
            {
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("Update ");
            }

            if ((col.Capabilities & AttributeCapabilities.AdvancedFind) != AttributeCapabilities.None)
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


        // Is now deprecated
        private void GenerateDefinitionCodeFromJson()
        {
            TableCollection tables = new TableCollection();
            List<OptionSetEnum> globalEnums = new List<OptionSetEnum>();
            List<OptionSetEnum> referencedSelectedOptionSet = new List<OptionSetEnum>();
            //
            //MessageBox.Show($"../../../../../{CoreProjectName}/Definitions");
            FileInfo fileInfo;
            String text;
            Table currentTable;
            foreach (string fileName in Directory.GetFiles($"../../../../../{CoreProjectName}/Definitions", "*.table"))
            {
                if(!fileName.Contains("OptionSet.table"))
                {
                    //MessageBox.Show(fileName);
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    JObject jTable = JObject.Parse(text);


                    currentTable = jTable.ToObject<Table>();
                    /*currentTable = JsonConvert.DeserializeObject<Table>(text,new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });*/

                    if (jTable["Cols"].Any())
                    {
                        Column currentColumn;
                        foreach (var jColumn in jTable["Cols"])
                        {
                            currentColumn = jColumn.ToObject<Column>();
                            currentTable.Columns.Add(currentColumn);
                        }
                    }
                    tables.Add(currentTable);
                }
                else
                {
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    globalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(text, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                    //MessageBox.Show($"There are {globalEnums.Count} enums");
                }
                
            }




            foreach (var table in tables)
            {
                var sb = new IndentedStringBuilder();

                sb.AppendLine("");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.CodeDom.Compiler;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine("using System.ComponentModel;");
                sb.AppendLine("using XrmFramework;");
                sb.AppendLine();
                sb.AppendLine($"namespace {CoreProjectName}");
                sb.AppendLine("{");

                using (sb.Indent())
                {

                    sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
                    sb.AppendLine("[EntityDefinition]");
                    sb.AppendLine("[ExcludeFromCodeCoverage]");


                    sb.AppendLine($"public static partial class {table.Name}Definition");
                    sb.AppendLine("{");

                    using (sb.Indent())
                    {

                        sb.AppendLine($"public const string EntityName = \"{table.LogicalName}\";");
                        sb.AppendLine($"public const string EntityCollectionName = \"{table.CollectionName}\";");

                        // foreach (var t in item.AdditionalInfoCollection.Definitions)
                        // {
                        //     sb.AppendLine();
                        //     sb.AppendLine(
                        //         $"public const {t.Type} {t.Name} = {(t.Type == "String" ? "\"" + (string)t.Value + "\"" : t.Value)};");
                        // }

                        sb.AppendLine();
                        sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                        sb.AppendLine("public static class Columns");
                        sb.AppendLine("{");

                        using (sb.Indent())
                        {
                            
                            foreach (var col in table.Columns)
                            {

                                AddColumnSummary(sb, col);
                                if (col.Type != null)
                                {

                                    sb.AppendLine($"[AttributeMetadata(AttributeTypeCode.{col.Type.ToString()})]");
                                    if (col.Type == AttributeTypeCode.Lookup)
                                    {
                                        var relation = table.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == col.LogicalName);
                                        if (relation != null)
                                        {
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);
                                            //var eC = this._entityCollection[relationship.ReferencedEntity];
                                            var rcol = tb?.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id);
                                            
                                            if (tb != null)
                                            {
                                                sb.Append($"[CrmLookup({tb.Name}Definition.EntityName,");
                                                if (rcol != null)
                                                {
                                                    sb.Append($"{tb.Name}Definition.Columns.{rcol.Name},");
                                                }
                                                else
                                                {
                                                    throw new Exception("No primaryType was found for the referenced table");
                                                    sb.Append($"{relation.LookupFieldName},");
                                                }
                                                
                                            }
                                            else
                                            {
                                                sb.Append($"[CrmLookup(\"{relation.EntityName}\",");
                                                sb.Append($"\"{relation.LookupFieldName}\",");
                                                
                                            }
                                            sb.AppendLine($"RelationshipName = ManyToOneRelationships.{relation.Name})]");
                                            
                                            
                                        }
                                        else
                                        {
                                            MessageBox.Show("The corresponding relationShip is null ??");
                                        }




                                    }



                                }

                                

                                if (col.PrimaryType == PrimaryType.Id)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Id)]");
                                }
                                if (col.EnumName != null && col.EnumName != "")
                                {
                                    foreach(var e in table.Enums)
                                    {
                                        if(e.LogicalName == col.EnumName)
                                        {

                                            sb.AppendLine($"[OptionSet(typeof({e.Name}))]");
                                            break;
                                        }
                                    }

                                    
                                }

                                if (col.PrimaryType == PrimaryType.Name)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Name)]");
                                }

                                if (col.PrimaryType == PrimaryType.Image)
                                {
                                    sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Image)]");
                                }
                                if (col.StringLength.HasValue)
                                {
                                    sb.AppendLine($"[StringLength({col.StringLength.Value})]");
                                }

                                if (col.MinRange.HasValue && col.MaxRange.HasValue)
                                {
                                    sb.AppendLine($"[Range({col.MinRange.Value}, {col.MaxRange.Value})]");
                                }
                                
                                if(table.Keys != null)
                                {
                                    foreach (var key in table.Keys)
                                    {
                                        if (key.FieldNames.FirstOrDefault(n => n == col.LogicalName) != null)
                                        {
                                            // Write a corresonding line
                                            sb.AppendLine($"[AlternateKey(AlternateKeyNames.{key.Name})]");
                                        }
                                    }
                                }
                                

                                if (col.DateTimeBehavior != null)
                                {
                                    var behavior = string.Empty;
                                    if (col.DateTimeBehavior == DateTimeBehavior.DateOnly)
                                    {
                                        behavior = "DateOnly";
                                    }
                                    else if (col.DateTimeBehavior ==
                                             DateTimeBehavior.TimeZoneIndependent)
                                    {
                                        behavior = "TimeZoneIndependent";
                                    }
                                    else if (col.DateTimeBehavior ==
                                             DateTimeBehavior.UserLocal)
                                    {
                                        behavior = "UserLocal";
                                    }

                                    sb.AppendLine($"[DateTimeBehavior(DateTimeBehavior.{behavior})]");
                                }

                                sb.AppendLine($"public const string {col.Name} = \"{col.LogicalName}\";\r\n");
                            }

                            
                            
                            

                            

                        }

                        sb.AppendLine("}");

                        //if(table.Enums.Any(e=>table.Columns.Any())

                        if (table.Keys != null && table.Keys.Any())
                        {
                            sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                            sb.AppendLine("public static class AlternateKeyNames");
                            sb.AppendLine("{");
                            using (sb.Indent())
                            {
                                foreach (var key in table.Keys)
                                {
                                    sb.AppendLine($"public const string {key.Name} = \"{key.LogicalName}\";\r\n");
                                }
                            }
                            sb.AppendLine("}");
                        }

                        if (table.ManyToOneRelationships.Any())
                        {
                            sb.AppendLine("public static class ManyToOneRelationships");
                            sb.AppendLine("{");
                            using (sb.Indent())
                            {
                                foreach (var relationship in table.ManyToOneRelationships)
                                {
                                    sb.Append("[Relationship(");
                                    var targetTable = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                    if (targetTable != null)
                                    {
                                        sb.Append($"{targetTable.Name}Definition.EntityName");
                                    }
                                    else
                                    {
                                        sb.Append($"\"{relationship.EntityName}\"");
                                    }

                                    sb.Append($", EntityRole.{relationship.Role}, \"{relationship.NavigationPropertyName}\", ");

                                    if (relationship.Role == EntityRole.Referencing)
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);


                                        //var re = tb?.OneToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);
                                        var rc = table.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);

                                        if (rc != null)
                                        {
                                            sb.Append($"{table.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }
                                    else
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                        var rc = table.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);

                                        //var r = tb?.OneToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);


                                        if (rc != null)
                                        {
                                            sb.Append($"{table.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }

                                    sb.AppendLine(")]");
                                    sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
                                }


                            }
                            sb.AppendLine("}");
                        }


                        if (table.ManyToManyRelationships.Any())
                        {
                            sb.AppendLine("public static class ManyToManyRelationships");
                            sb.AppendLine("{");
                            using (sb.Indent())
                            {
                                foreach (var relationship in table.ManyToManyRelationships)
                                {
                                    sb.Append("[Relationship(");
                                    var targetTable = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                    if (targetTable != null)
                                    {
                                        sb.Append($"{targetTable.Name}Definition.EntityName");
                                    }
                                    else
                                    {
                                        sb.Append($"\"{relationship.EntityName}\"");
                                    }

                                    sb.Append($", EntityRole.{relationship.Role}, \"{relationship.NavigationPropertyName}\", ");

                                    if (relationship.Role == EntityRole.Referencing)
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                        var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                        //var re = tb?.ManyToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);


                                        if (rc != null)
                                        {
                                            sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }
                                    else
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);

                                        var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                        if (rc != null)
                                        {
                                            sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }

                                    sb.AppendLine(")]");
                                    sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
                                }
                            }
                            sb.AppendLine("}");
                        }
                        if (table.OneToManyRelationships.Any())
                        {
                            sb.AppendLine("public static class OneToManyRelationships");
                            sb.AppendLine("{");
                            using (sb.Indent())
                            {
                                foreach (var relationship in table.OneToManyRelationships)
                                {
                                    sb.Append("[Relationship(");
                                    var targetTable = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                    if (targetTable != null)
                                    {
                                        sb.Append($"{targetTable.Name}Definition.EntityName");
                                    }
                                    else
                                    {
                                        sb.Append($"\"{relationship.EntityName}\"");
                                    }

                                    sb.Append($", EntityRole.{relationship.Role}, \"{relationship.NavigationPropertyName}\", ");

                                    if (relationship.Role == EntityRole.Referencing)
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                        var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                        //var re = tb?.ManyToOneRelationships.FirstOrDefault(r => r.Name == relationship.Name);
                                        

                                        if (rc != null)
                                        {
                                            sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }
                                    else
                                    {
                                        var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);

                                        //var re = tb?.ManyToOneRelationships.FirstOrDefault(r => r.Name == relationship.Name);
                                        var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                        if (rc != null)
                                        {
                                            sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{relationship.LookupFieldName}\"");
                                        }
                                    }

                                    sb.AppendLine(")]");
                                    sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
                                }
                            }
                            sb.AppendLine("}");
                        }

                        if(table.Enums.Any())
                        {
                            foreach (var ose in table.Enums)
                            {
                                sb.AppendLine();
                                if (ose.IsGlobal)
                                {
                                    continue;
                                }
                                else
                                {
                                    var referencedColumns = table.Columns.Where(c => c.EnumName == ose.LogicalName);
                                    //var attribute = def.ReferencedBy.First();

                                    if (!referencedColumns.Any())
                                    {
                                        continue;
                                    }
                                    foreach(var col in referencedColumns)
                                    {
                                        sb.AppendLine(string.Format("[OptionSetDefinition({0}Definition.EntityName, {0}Definition.Columns.{1})]",
                                        table.Name, col.Name));
                                    }
                                    
                                }

                                sb.AppendLine($"public enum {ose.Name}");
                                sb.AppendLine("{");

                                using (sb.Indent())
                                {

                                    if (ose.HasNullValue)
                                    {
                                        sb.AppendLine("Null = 0,");
                                    }

                                    foreach (var val in ose.Values)
                                    {
                                        sb.AppendLine($"[Description(\"{val.Name}\")]");

                                        if (!string.IsNullOrEmpty(val.ExternalValue))
                                        {
                                            sb.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
                                        }

                                        sb.AppendLine($"{val.Name} = {val.Value},");
                                    }
                                }

                                sb.AppendLine("}");
                            }

                        }


                    }

                    sb.AppendLine("}");
                }

                sb.AppendLine("}");



                var classFileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{table.Name}.table.cs");

                var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
                if (definitionFolder.Exists == false)
                {
                    definitionFolder.Create();
                }

                File.WriteAllText(classFileInfo.FullName, sb.ToString());



                


            }

            var fc = new IndentedStringBuilder();
            fc.AppendLine("using System.ComponentModel;");
            fc.AppendLine("using XrmFramework;");
            fc.AppendLine();
            fc.AppendLine($"namespace {CoreProjectName}");
            fc.AppendLine("{");

            using (fc.Indent())
            {
                foreach (var ose in globalEnums)
                {
                    fc.AppendLine();
                    if (ose.IsGlobal)
                    {
                        fc.AppendLine($"[OptionSetDefinition(\"{ose.LogicalName}\")]");
                    }
                    else
                    {
                        continue;
                    }

                    fc.AppendLine($"public enum {ose.Name}");
                    fc.AppendLine("{");

                    using (fc.Indent())
                    {

                        if (ose.HasNullValue)
                        {
                            fc.AppendLine("Null = 0,");
                        }

                        foreach (var val in ose.Values)
                        {
                            fc.AppendLine($"[Description(\"{val.Name}\")]");

                            if (!string.IsNullOrEmpty(val.ExternalValue))
                            {
                                fc.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
                            }

                            fc.AppendLine($"{val.Name} = {val.Value},");
                        }
                    }

                    fc.AppendLine("}");
                }
            }
            fc.AppendLine("}");


            var optionSetFileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/OptionSet.table.cs");
            
            File.WriteAllText(optionSetFileInfo.FullName, fc.ToString());
            MessageBox.Show("Finished writing option set");







            //MessageBox.Show(sb.ToString());
        }

        public List<Table> GetTablesFromFormerDefinitionCode()
        {
            MessageBox.Show("Going to try to get coded entity defs");

            List<Table> localTables = new List<Table>();
            //MessageBox.Show("Getting ");
            // Get entity definitions
            var definitionsToBeConverted = GetCodedEntityDefinitions();
            Table table;
            Column column;
            Relation relation;

            foreach(var entity in definitionsToBeConverted)
            {
                table = new Table()
                {
                    LogicalName = entity.LogicalName,
                    CollectionName = entity.LogicalCollectionName,
                    Name = entity.Name.Replace("Definition",""),
                    Selected = true,
                    
                    
                };


                foreach (var def in entity.AdditionalClassesCollection.Definitions)
                {
                    if (def.IsEnum)
                    {

                    }
                    
                }


                // Assign columns
                foreach (var attr in entity.AttributesCollection.Definitions)
                {
                    column = new Column()
                    {
                        LogicalName = attr.LogicalName,
                        Name = attr.Name,


                        Selected = true,
                        //Delete this line after generation of base tables
                        //IsLocked = true,
                        
                    };


                    table.Columns.Add(column);
                    /*
                    // Assign Primary type
                    if(attr.IsPrimaryIdAttribute)
                    {
                        column.PrimaryType = PrimaryType.Id;
                    }
                    else if(attr.IsPrimaryImageAttribute)
                    {
                        column.PrimaryType = PrimaryType.Image;
                    }
                    else if(attr.IsPrimaryNameAttribute)
                    {
                        column.PrimaryType = PrimaryType.Name;
                    }
                    else
                    {
                        column.PrimaryType = PrimaryType.None;
                    }
                    
                    // Assign AttributeTypeCode Type
                    if(!string.IsNullOrEmpty(attr.Type))
                    {
                        column.Type = (AttributeTypeCode)Enum.Parse(typeof(AttributeTypeCode), attr.Type);
                    }
                    //Assign Capabilities
                    if(attr.IsValidForAdvancedFind)
                    {
                        column.Capabilities |= AttributeCapabilities.AdvancedFind;
                    }
                    if (attr.IsValidForCreate)
                    {
                        column.Capabilities |= AttributeCapabilities.Create;
                    }
                    if (attr.IsValidForRead)
                    {
                        column.Capabilities |= AttributeCapabilities.Read;
                    }
                    if (attr.IsValidForUpdate)
                    {
                        column.Capabilities |= AttributeCapabilities.Update;
                    }
                    // Assign Labels ? 

                    // Assign StringLength
                    column.StringLength = attr.StringMaxLength;
                    // Assign MinRange
                    column.MinRange = attr.MinRange;
                    // Assign MaxRange
                    column.MaxRange = attr.MaxRange;
                    // DateTimeBehavior
                    column.DateTimeBehavior = attr.DateTimeBehavior;
                    //IsMultiSelect ?

                    //EnumName
                    column.EnumName = attr.EnumName;
                    //Selected
                    column.Selected = true;
                    if(attr.Relationships != null && attr.Relationships.Any())
                    {
                        column.Type = AttributeTypeCode.Lookup;
                    }

                    // Assign keys
                    if(attr.KeyNames != null && attr.KeyNames.Any())
                    {
                        foreach(var keyAttr in attr.KeyNames)
                        {
                            var correspondingkey = table.Keys.FirstOrDefault(k => k.Name == keyAttr);
                            if (correspondingkey != null)
                            {
                                correspondingkey.FieldNames.Add(attr.LogicalName);
                            }
                        }
                    }*/
                }





                // Assign Enums

                // Assign selected
                table.Selected = true;

                localTables.Add(table);
            }


            // Penser aussi à checker pour récupérer les enums globaux s'il y en a; ou juste a les mettre en tant qu'enum selectionné



            return localTables;
        }

        



        private EntityDefinition TableToBaseEntityDefinition(Table table)
        {
            var entityDefinition = new EntityDefinition()
            {
                Name = table.Name + "Definition",
                LogicalCollectionName = table.CollectionName,
                LogicalName = table.LogicalName,
                IsSelected = true,
            };

            foreach(var col in table.Columns)
            {
                var attributeDefinition = new AttributeDefinition()
                {
                    DisplayName = col.Name,
                    LogicalName = col.LogicalName,
                    Name = col.Name,
                    IsSelected=col.Selected,

                };

                entityDefinition.Add(attributeDefinition);
            }



            return entityDefinition;
        }

        private Table EntityDefinitionToBaseTable(EntityDefinition entity)
        {

            Column column;
            var table = new Table()
            {
                LogicalName = entity.LogicalName,
                CollectionName = entity.LogicalCollectionName,
                Name = entity.Name,
            };

            foreach (var attr in entity.AttributesCollection.Definitions)
            {

                column = new Column()
                {
                    LogicalName = attr.LogicalName,
                    Name = attr.Name,
                    Selected = attr.IsSelected,

                };

                // Assign Primary type
                if (attr.IsPrimaryIdAttribute)
                {
                    column.PrimaryType = PrimaryType.Id;
                }
                else if (attr.IsPrimaryImageAttribute)
                {
                    column.PrimaryType = PrimaryType.Image;
                }
                else if (attr.IsPrimaryNameAttribute)
                {
                    column.PrimaryType = PrimaryType.Name;
                }
                else
                {
                    column.PrimaryType = PrimaryType.None;
                }

                // Assign AttributeTypeCode Type
                if (!string.IsNullOrEmpty(attr.Type))
                {
                    column.Type = (AttributeTypeCode)Enum.Parse(typeof(AttributeTypeCode), attr.Type);
                }
                //Assign Capabilities
                if (attr.IsValidForAdvancedFind)
                {
                    column.Capabilities |= AttributeCapabilities.AdvancedFind;
                }
                if (attr.IsValidForCreate)
                {
                    column.Capabilities |= AttributeCapabilities.Create;
                }
                if (attr.IsValidForRead)
                {
                    column.Capabilities |= AttributeCapabilities.Read;
                }
                if (attr.IsValidForUpdate)
                {
                    column.Capabilities |= AttributeCapabilities.Update;
                }
                // Assign Labels ? 

                // Assign StringLength
                column.StringLength = attr.StringMaxLength;
                // Assign MinRange
                column.MinRange = attr.MinRange;
                // Assign MaxRange
                column.MaxRange = attr.MaxRange;
                // DateTimeBehavior
                column.DateTimeBehavior = attr.DateTimeBehavior.ToDateTimeBehavior();
                //IsMultiSelect ?

                //EnumName
                column.EnumName = attr.EnumName;
                //Selected
                column.Selected = true;
                if (attr.Relationships != null && attr.Relationships.Any())
                {
                    column.Type = AttributeTypeCode.Lookup;
                }

                // Assign keys
                if (attr.KeyNames != null && attr.KeyNames.Any())
                {
                    foreach (var keyAttr in attr.KeyNames)
                    {
                        var correspondingkey = table.Keys.FirstOrDefault(k => k.Name == keyAttr);
                        if (correspondingkey != null)
                        {
                            correspondingkey.FieldNames.Add(attr.LogicalName);
                        }
                    }
                }
            }






            return table;
        }


        public TableCollection LoadLocalTables()
        {
            TableCollection tables = new TableCollection();

            foreach (string fileName in Directory.GetFiles($"../../../../../{CoreProjectName}/Definitions", "*.table"))
            {
                if (!fileName.Contains("OptionSet.table"))
                {
                    //MessageBox.Show(fileName);
                    var fileInfo = new FileInfo(fileName);
                    var text = File.ReadAllText(fileInfo.FullName);
                    JObject jTable = JObject.Parse(text);


                    var currentTable = jTable.ToObject<Table>();
                    /*currentTable = JsonConvert.DeserializeObject<Table>(text,new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });*/

                    if (jTable["Cols"].Any())
                    {
                        Column currentColumn;
                        foreach (var jColumn in jTable["Cols"])
                        {
                            currentColumn = jColumn.ToObject<Column>();
                            currentTable.Columns.Add(currentColumn);
                        }
                    }
                    tables.Add(currentTable);
                }
                else
                {
                    /*
                    fileInfo = new FileInfo(fileName);
                    text = File.ReadAllText(fileInfo.FullName);
                    globalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(text, new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                    MessageBox.Show($"There are {globalEnums.Count} enums");
                    */
                }

            }




            return tables;
        }

        private void getEntitiesFromCRMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.generateDefinitionsToolStripMenuItem.Enabled = false;
            this.entityListView.Enabled = false;
            this.attributeListView.Enabled = false;
            this.getEntitiesFromCRMToolStripMenuItem.Enabled = false;
            DataAccessManager.Instance.Connect(ConnectionSucceeded);
            
        }


        //public void MergeLocalTablesWithCrmData(TableCollection localTables)
        //
        //   // Merge with TableCollection
        //   foreach (Table table in localTables)
        //   {
        //       var correspondingTable = _tables.FirstOrDefault(t => t.LogicalName == table.LogicalName);
        //       if (correspondingTable != null)
        //       {
        //           correspondingTable.Name = table.Name;
        //           correspondingTable.Selected = true;
        //           foreach(var col in table.Columns)
        //           {
        //               var correspondingCol = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == col.LogicalName);
        //               if(correspondingCol != null)
        //               {
        //                   correspondingCol.Name = col.Name;
        //                   correspondingCol.Selected = true;
        //               }
        //           }
        //       }
        //
        //       var correspondingEntity = _entityCollection[table.LogicalName];
        //       correspondingEntity.IsSelected = true;
        //       //MessageBox.Show("Has just tried to set IsSelected to true");
        //       
        //       if(correspondingEntity != null)
        //       {
        //           _entityCollection[table.LogicalName].IsSelected = true;
        //           //MessageBox.Show("Has just tried to set IsSelected to true");
        //
        //           correspondingEntity.Name = table.Name+"Definition ocajociaz";
        //           foreach( var col in table.Columns)
        //           {
        //               var correspondingAttribute = correspondingEntity.AttributesCollection[col.LogicalName];
        //               if(correspondingAttribute != null)
        //               {
        //                   correspondingAttribute.IsSelected = true;
        //                   correspondingAttribute.Name = col.Name + "ovnhzeo";
        //               }
        //           }
        //       }
        //
        //   }
        //
        //
        //
        //
        //
    }
    

}


/*var fc = new IndentedStringBuilder();
            fc.AppendLine("using System.ComponentModel;");
            fc.AppendLine("using XrmFramework;");
            fc.AppendLine();
            fc.AppendLine($"namespace {CoreProjectName}");
            fc.AppendLine("{");

            using (fc.Indent())
            {
                foreach (var ose in enums)
                {
                    fc.AppendLine();
                    if (ose.IsGlobal)
                    {
                        fc.AppendLine($"[OptionSetDefinition(\"{ose.LogicalName}\")]");
                    }
                    else
                    {
                        var attribute = ose.ReferencedBy.First();

                        if (!_entityCollection.SelectedDefinitions.Any(s => s.LogicalName == attribute.ParentEntity.LogicalName))
                        {
                            continue;
                        }

                        fc.AppendLine(string.Format("[OptionSetDefinition({0}.EntityName, {0}.Columns.{1})]",
                            attribute.ParentEntity.Name, attribute.Name));
                    }

                    fc.AppendLine($"public enum {ose.Name}");
                    fc.AppendLine("{");

                    using (fc.Indent())
                    {

                        if (ose.HasNullValue)
                        {
                            fc.AppendLine("Null = 0,");
                        }

                        foreach (var val in ose.Values)
                        {
                            fc.AppendLine($"[Description(\"{val.Name}\")]");

                            if (!string.IsNullOrEmpty(val.ExternalValue))
                            {
                                fc.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
                            }

                            fc.AppendLine($"{val.Name} = {val.Value},");
                        }
                    }

                    fc.AppendLine("}");
                }
            }

            fc.AppendLine("}");*/
