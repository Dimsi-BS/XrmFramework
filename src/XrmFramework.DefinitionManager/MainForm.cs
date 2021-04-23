// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Deploy;
using XrmFramework;
using XrmFramework.DeployUtils;
using RelationshipAttributeDefinition = DefinitionManager.Definitions.RelationshipAttributeDefinition;

namespace DefinitionManager
{
    public partial class MainForm : Form, ICustomListProvider
    {
        private readonly DefinitionCollection<EntityDefinition> entityCollection;

        private readonly Type _iServiceType;

        public MainForm(Type iServiceType)
        {
            _iServiceType = iServiceType;

            CustomProvider.Instance = this;
            InitializeComponent();

            DataAccessManager.Instance.SetConnectionHelperType(GetExternalType("XrmFramework.ConnectionHelper"));

            DataAccessManager.Instance.StepChanged += StepChangedHandler;

            entityCollection = new DefinitionCollection<EntityDefinition>();

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
            var entities = (List<EntityDefinition>)result;

            entityCollection.AddRange(entities);
            this.generateDefinitionsToolStripMenuItem.Enabled = true;
            this.entityListView.Enabled = true;
            this.attributeListView.Enabled = true;
        }

        void ConnectionSucceeded(object service)
        {
            DataAccessManager.Instance.RetrieveEntities(RetrieveEntitiesSucceeded);
        }

        private void DefinitionManager_Load(object sender, EventArgs e)
        {
            InitEnumDefinitions();

            entityCollection.AttachListView(this.entityListView);
            entityCollection.AddRange(GetCodedEntityDefinitions());

            DataAccessManager.Instance.Connect(ConnectionSucceeded);
        }

        private void InitEnumDefinitions()
        {
            var optionSetDefinitionAttributeType = GetExternalType("XrmFramework.OptionSetDefinitionAttribute");

            var definitionTypes = _iServiceType.Assembly.GetTypes().Where(t => t.GetCustomAttributes(optionSetDefinitionAttributeType, false).Any());
            var definitionManagerIgnoreType = GetExternalType("XrmFramework.DefinitionManagerIgnoreAttribute");


            foreach (var type in definitionTypes)
            {
                dynamic attribute = type.GetCustomAttribute(optionSetDefinitionAttributeType);

                var ignoreAttribute = type.GetCustomAttribute(definitionManagerIgnoreType);
                if (ignoreAttribute != null)
                {
                    continue;
                }

                var enumDefinition = new EnumDefinition
                {
                    LogicalName = (string.IsNullOrEmpty(attribute.EntityName) ? string.Empty : attribute.EntityName + "_") + attribute.LogicalName,
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

                        if ((int)value == 0)
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

        private IEnumerable<EntityDefinition> GetCodedEntityDefinitions()
        {
            var entityDefinitionAttributeType = GetExternalType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = _iServiceType.Assembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            var definitionManagerIgnoreType = GetExternalType("XrmFramework.DefinitionManagerIgnoreAttribute");
            var relationshipAttributeType = GetExternalType("XrmFramework.RelationshipAttribute");

            var definitionList = new List<EntityDefinition>();

            foreach (var t in definitionTypes)
            {
                var ignoreAttribute = t.GetCustomAttribute(definitionManagerIgnoreType);
                if (ignoreAttribute != null)
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
            var fileList = new List<TfsHelper.FileInfo>();
            foreach (var item in this.entityCollection.SelectedDefinitions)
            {
                var sb = new StringBuilder();

                sb.AppendLine("");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.CodeDom.Compiler;");
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                sb.AppendLine("using XrmFramework;");
                sb.AppendLine();
                sb.AppendLine("namespace Model");
                sb.AppendLine("{");
                sb.AppendLine("\t[GeneratedCode(\"XrmFramework\", \"1.0\")]");
                sb.AppendLine("\t[EntityDefinition]\r\n");
                sb.AppendLine("\t[ExcludeFromCodeCoverage]\r\n");
                sb.AppendFormat("\tpublic static class {0}\r\n", item.Name);
                sb.AppendLine("\t{");
                sb.AppendFormat("\t\tpublic const string EntityName = \"{0}\";\r\n", item.LogicalName);
                sb.AppendFormat("\t\tpublic const string EntityCollectionName = \"{0}\";\r\n", item.LogicalCollectionName);

                foreach (var t in item.AdditionalInfoCollection.Definitions)
                {
                    sb.AppendLine();
                    sb.AppendFormat("\t\tpublic const {0} {1} = {2};\r\n", t.Type, t.Name, t.Type == "String" ? "\"" + (string)t.Value + "\"" : t.Value);
                }

                sb.AppendLine();
                sb.AppendLine("\t\t[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                sb.AppendLine("\t\tpublic static class Columns");
                sb.AppendLine("\t\t{");

                foreach (var attr in item.AttributesCollection.SelectedDefinitions)
                {
                    sb.Append(attr.Summary);
                    if (!string.IsNullOrEmpty(attr.Type))
                    {
                        sb.AppendFormat("\t\t\t[AttributeMetadata(AttributeTypeCode.{0})]\r\n", attr.Type);
                    }
                    if (attr.Enum != null)
                    {
                        sb.AppendFormat("\t\t\t[OptionSet(typeof({0}))]\r\n", attr.Enum.Name);
                    }
                    if (attr.IsPrimaryIdAttribute)
                    {
                        sb.AppendLine("\t\t\t[PrimaryAttribute(PrimaryAttributeType.Id)]");
                    }
                    if (attr.IsPrimaryNameAttribute)
                    {
                        sb.AppendLine("\t\t\t[PrimaryAttribute(PrimaryAttributeType.Name)]");
                    }
                    if (attr.IsPrimaryImageAttribute)
                    {
                        sb.AppendLine("\t\t\t[PrimaryAttribute(PrimaryAttributeType.Image)]");
                    }

                    if (attr.StringMaxLength.HasValue)
                    {
                        sb.AppendLine($"\t\t\t[StringLength({attr.StringMaxLength.Value})]");
                    }

                    if (attr.MinRange.HasValue && attr.MaxRange.HasValue)
                    {
                        sb.AppendLine($"\t\t\t[Range({attr.MinRange.Value}, {attr.MaxRange.Value})]");
                    }

                    foreach (var keyName in attr.KeyNames)
                    {
                        sb.AppendFormat("\t\t\t[AlternateKey(AlternateKeyNames.{0})]\r\n", keyName);
                    }
                    if (attr.DateTimeBehavior != null)
                    {
                        var behavior = string.Empty;
                        if (attr.DateTimeBehavior == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.DateOnly)
                        {
                            behavior = "DateOnly";
                        }
                        else if (attr.DateTimeBehavior == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.TimeZoneIndependent)
                        {
                            behavior = "TimeZoneIndependent";
                        }
                        else if (attr.DateTimeBehavior == Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior.UserLocal)
                        {
                            behavior = "UserLocal";
                        }
                        sb.AppendFormat("\t\t\t[DateTimeBehavior(DateTimeBehavior.{0})]\r\n", behavior);
                    }
                    foreach (var relationship in attr.Relationships)
                    {
                        if (this.entityCollection.SelectedDefinitions.Any(d => d.LogicalName == relationship.ReferencedEntity))
                        {
                            var eC = this.entityCollection[relationship.ReferencedEntity];
                            sb.AppendFormat("\t\t\t[CrmLookup({0}.EntityName, {0}.Columns.{1}, RelationshipName = ManyToOneRelationships.{2})]\r\n", eC.Name, eC.AttributesCollection[relationship.ReferencedAttribute].Name, relationship.SchemaName);
                        }
                        else if (relationship.ReferencedEntity != "owner")
                        {
                            sb.AppendFormat("\t\t\t[CrmLookup(\"{0}\", \"{1}\", RelationshipName = \"{2}\")]\r\n", relationship.ReferencedEntity, relationship.ReferencedAttribute, relationship.SchemaName);
                        }
                        else if (relationship.ReferencedEntity == "owner")
                        {
                            sb.AppendFormat("\t\t\t[CrmLookup(\"{0}\", \"{1}\", RelationshipName = \"{2}\")]\r\n", relationship.ReferencedEntity, relationship.ReferencedAttribute, relationship.SchemaName);
                        }
                    }

                    sb.AppendFormat("\t\t\tpublic const string {0} = \"{1}\";\r\n\r\n", attr.Name, attr.LogicalName);
                }

                sb.AppendLine("\t\t}");


                foreach (var def in item.AdditionalClassesCollection.Definitions)
                {
                    sb.AppendLine();
                    if (def.IsEnum)
                    {
                        sb.AppendFormat("\t\tpublic enum {0}\r\n", def.Name);
                    }
                    else
                    {
                        sb.AppendLine("\t\t[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                        sb.AppendFormat("\t\tpublic static class {0}\r\n", def.Name);
                    }
                    sb.AppendLine("\t\t{");

                    if (def.IsEnum)
                    {
                        foreach (var attr in def.Attributes.Definitions.OrderBy(a => a.Value))
                        {
                            sb.AppendFormat("\t\t\t{0} = {1},\r\n", attr.Name, attr.Value);
                        }
                    }
                    else
                    {
                        foreach (var attr in def.Attributes.Definitions.OrderBy(a => a.Name))
                        {
                            if (attr.Type == "String")
                            {
                                if (attr is RelationshipAttributeDefinition)
                                {
                                    sb.Append($"\t\t\t[Relationship(");
                                    var rAttr = attr as RelationshipAttributeDefinition;
                                    if (this.entityCollection.SelectedDefinitions.Any(d => d.LogicalName == rAttr.TargetEntityName))
                                    {
                                        sb.Append($"{entityCollection[rAttr.TargetEntityName].Name}.EntityName");
                                    }
                                    else
                                    {
                                        sb.Append($"\"{rAttr.TargetEntityName}\"");
                                    }

                                    sb.Append($", EntityRole.{rAttr.Role}, \"{rAttr.NavigationPropertyName}\", ");

                                    if (rAttr.Role == "Referencing")
                                    {
                                        var ec = entityCollection.Definitions.FirstOrDefault(d => d.LogicalName == item.LogicalName);
                                        var att = ec?.AttributesCollection.SelectedDefinitions.FirstOrDefault(d => d.LogicalName == rAttr.LookupFieldName);

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
                                        var ec = entityCollection.SelectedDefinitions.FirstOrDefault(d => d.LogicalName == rAttr.TargetEntityName);
                                        var att = ec?.AttributesCollection.SelectedDefinitions.FirstOrDefault(d => d.LogicalName == rAttr.LookupFieldName);

                                        if (att != null)
                                        {
                                            sb.Append($"{ec.Name}.Columns.{att.Name}");
                                        }
                                        else
                                        {
                                            sb.Append($"\"{rAttr.LookupFieldName}\"");
                                        }
                                    }

                                    sb.Append(")]\r\n");
                                }

                                sb.AppendFormat("\t\t\tpublic const string {0} = \"{1}\";\r\n", attr.Name, attr.Value);
                            }
                            else
                            {
                                sb.AppendFormat("\t\t\tpublic const int {0} = {1};\r\n", attr.Name, attr.Value);
                            }
                        }
                    }

                    sb.AppendLine("\t\t}");
                }


                sb.AppendLine("\t}");
                sb.AppendLine("}");

                var fileInfo = new FileInfo(string.Format("../../../../../Model/Definitions/{0}.cs", item.Name));

                File.WriteAllText(fileInfo.FullName, sb.ToString());
                string oldFileName = null;

                if (!string.IsNullOrEmpty(item.PreviousName) && item.PreviousName != item.Name)
                {
                    oldFileName = string.Format("{0}.cs", item.PreviousName);
                }

                fileList.Add(new TfsHelper.FileInfo(fileInfo.Name, oldFileName));
            }


            var fc = new StringBuilder();
            fc.AppendLine("using System.ComponentModel;");
            fc.AppendLine("using XrmFramework;");
            fc.AppendLine();
            fc.AppendLine("namespace Model");
            fc.AppendLine("{");
            foreach (var def in EnumDefinitionCollection.Instance.SelectedDefinitions)
            {
                fc.AppendLine();
                if (def.IsGlobal)
                {
                    fc.AppendFormat("\t[OptionSetDefinition(\"{0}\")]\r\n", def.LogicalName);
                }
                else
                {
                    var attribute = def.ReferencedBy.First();

                    fc.AppendFormat("\t[OptionSetDefinition({0}.EntityName, {0}.Columns.{1})]\r\n", attribute.ParentEntity.Name, attribute.Name);
                }
                fc.AppendFormat("\tpublic enum {0}\r\n", def.Name);
                fc.AppendLine("\t{");
                if (def.HasNullValue)
                {
                    fc.AppendLine("\t\tNull = 0,");
                }
                foreach (var val in def.Values.Definitions)
                {
                    fc.AppendFormat(CultureInfo.InvariantCulture, "\t\t[Description(\"{0}\")]\r\n", val.DisplayName);
                    if (!string.IsNullOrEmpty(val.ExternalValue))
                    {
                        fc.AppendFormat(CultureInfo.InvariantCulture, "\t\t[ExternalValue(\"{0}\")]\r\n", val.ExternalValue);
                    }

                    fc.AppendFormat("\t\t{0} = {1},\r\n", val.Name, val.LogicalName);
                }

                fc.AppendLine("\t}");
            }
            fc.AppendLine("}");
            File.WriteAllText("../../../../../Model/Definitions/OptionSetDefinitions.cs", fc.ToString());
            fileList.Add(new TfsHelper.FileInfo("OptionSetDefinitions.cs"));

            TfsHelper.EnsureReferencesInProjectFile("../../../../../Model/Model.projitems", fileList, @"$(MSBuildThisFileDirectory)Definitions");

            MessageBox.Show("Definition files generation succeedeed");
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
