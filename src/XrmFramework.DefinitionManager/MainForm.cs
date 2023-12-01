// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DefinitionManager;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XrmFramework.Core;

namespace XrmFramework.DefinitionManager;

public partial class MainForm : Form, ICustomListProvider
{
    private const string OptionSetFileName = "CustomOptionSets";
    private readonly DefinitionCollection<EntityDefinition> _entityCollection;
    private readonly List<OptionSetEnum> _enums = new();

    private readonly Type _iServiceType;

    private readonly TableCollection _localTables;
    private readonly TableCollection _selectedTables;
    private readonly TableCollection _tables;

    public MainForm(string coreProjectName)
    {
        _iServiceType = typeof(IService);
        CoreProjectName = coreProjectName;
        CustomProvider.Instance = this;
        InitializeComponent();

        DataAccessManager.Instance.StepChanged += StepChangedHandler;

        _entityCollection = new DefinitionCollection<EntityDefinition>();
        _tables = new TableCollection();
        _selectedTables = new TableCollection();
        _localTables = new TableCollection();

        attributeListView.SelectionChanged += attributeListView_SelectionChanged;


        _selectedTables = new TableCollection();
        _localTables = LoadLocalTables();


        foreach (var table in _localTables)
        {
            var localEntity = TableToBaseEntityDefinition(table);
            _entityCollection.Add(localEntity);
            _tables.Add(table);
        }


        generateDefinitionsToolStripMenuItem.Enabled = true;
        entityListView.Enabled = true;
        attributeListView.Enabled = true;
    }

    public string CoreProjectName { get; }

    public T GetCustomList<T>()
    {
        var result = GetCustomList(typeof(T));

        return (T)result;
    }


    public object GetCustomList(Type type)
    {
        foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            if (type.IsAssignableFrom(field.FieldType))
                return field.GetValue(this);
        return null;
    }

    private void attributeListView_SelectionChanged(object sender,
        CustomListViewControl<AttributeDefinition>.SelectionChangedEventArgs e)
    {
        if (e.IsSelected)
        {
            if (splitContainer2.SplitterDistance == 100) splitContainer2.SplitterDistance = 350;

            splitContainer2.Panel2Collapsed = e.Definition.Enum == null;
        }
    }

    private void StepChangedHandler(object sender, StepChangedEventArgs e)
    {
        toolStripStatusLabel1.Text = e.StepName;
    }

    private void RetrieveEntitiesSucceeded(object result)
    {
        var entities = (Tuple<List<EntityDefinition>, List<Table>, List<OptionSetEnum>>)result;

        _entityCollection.AddRange(entities.Item1);
        _tables.AddRange(entities.Item2);
        MessageBox.Show($"There are {_localTables.Count} local tables");
        _enums.AddRange(entities.Item3);
        generateDefinitionsToolStripMenuItem.Enabled = true;

        entityListView.Enabled = true;
        attributeListView.Enabled = true;
    }

    private void ConnectionSucceeded(object service)
    {
        DataAccessManager.Instance.RetrieveEntities(RetrieveEntitiesSucceeded,
            _entityCollection.Definitions.Select(d => d.LogicalName).ToArray());
    }

    private void DefinitionManager_Load(object sender, EventArgs e)
    {
        InitEnumDefinitions();
        var localCodedDefinitions = new List<EntityDefinition>();
        _entityCollection.AttachListView(entityListView);
    }

    private void InitEnumDefinitions()
    {
        var optionSetDefinitionAttributeType = GetExternalType("XrmFramework.OptionSetDefinitionAttribute");
        var definitionManagerIgnoreAttributeType =
            GetExternalType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute");

        var definitionTypes = _iServiceType.Assembly.GetTypes()
            .Where(t => t.GetCustomAttributes(optionSetDefinitionAttributeType, false).Any());

        foreach (var type in definitionTypes)
        {
            if (type.GetCustomAttributes(definitionManagerIgnoreAttributeType).Any()) continue;

            dynamic attribute = type.GetCustomAttribute(optionSetDefinitionAttributeType);

            var enumDefinition = new EnumDefinition
            {
                LogicalName = (string.IsNullOrEmpty(attribute.EntityName) ? string.Empty : attribute.EntityName + "|") +
                              attribute.LogicalName,
                Name = type.Name,
                IsGlobal = string.IsNullOrEmpty(attribute.EntityName)
            };

            if (type.IsEnum)
                foreach (var name in Enum.GetNames(type))
                {
                    if (name == "Null") continue;

                    var value = (int)Enum.Parse(type, name);

                    enumDefinition.Values.Add(new EnumValueDefinition
                    {
                        Name = name,
                        LogicalName = value.ToString(),
                        Value = value.ToString()
                    });
                }
            else
                foreach (var field in type.GetFields())
                {
                    var value = (int)field.GetValue(null);

                    if (value == 0) continue;

                    enumDefinition.Values.Add(new EnumValueDefinition
                    {
                        Name = field.Name,
                        LogicalName = value.ToString(),
                        Value = value.ToString()
                    });
                }

            EnumDefinitionCollection.Instance.Add(enumDefinition);
        }
    }

    private Type GetExternalType(string name)
    {
        return _iServiceType.Assembly.GetType(name);
    }

    private void generateDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        foreach (var item in _entityCollection.SelectedDefinitions)
            if (!_selectedTables.Any(t => t.LogicalName == item.LogicalName))
            {
                var table = _tables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                table.Selected = true;
                foreach (var a in item.AttributesCollection.SelectedDefinitions)
                    table.Columns.FirstOrDefault(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;

                _selectedTables.Add(table);
            }
            else
            {
                var tableToRemove = _selectedTables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                _selectedTables.Remove(tableToRemove);

                var table = _tables.FirstOrDefault(t => t.LogicalName == item.LogicalName);
                if (table == null)
                    throw new Exception(
                        $"Table named {item.LogicalName} does not exist in the list _tables, _tables.Count is {_tables.Count}");
                table.Selected = true;
                foreach (var a in item.AttributesCollection.SelectedDefinitions)
                    table.Columns.FirstOrDefault(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;


                _selectedTables.Add(table);
            }

        var nameOfTablesToDelete = new List<string>();
        // Delete tables that aren't selected
        foreach (var table in _selectedTables)
            if (!_entityCollection.SelectedDefinitions.Any(e => e.LogicalName == table.LogicalName))
                nameOfTablesToDelete.Add(table.LogicalName);

        foreach (var name in nameOfTablesToDelete)
            _selectedTables.Remove(_selectedTables.FirstOrDefault(t => t.LogicalName == name));


        var globalEnums = EnumDefinitionCollection.Instance.SelectedDefinitions
            .Where(en => en.IsSelected && en.IsGlobal)
            .Select(en => en.LogicalName).ToList();

        var globalOptionSets = new Table
        {
            LogicalName = "globalEnums",
            Name = OptionSetFileName
        };
        globalOptionSets.Enums.AddRange(_enums.Where(en => globalEnums.Contains(en.LogicalName) && en.IsGlobal));

        var optionSetsTxt = JsonConvert.SerializeObject(globalOptionSets, Formatting.Indented,
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

        var fileInfoOptionSets =
            new FileInfo($"../../../../../{CoreProjectName}/Definitions/{globalOptionSets.Name}.table");

        File.WriteAllText(fileInfoOptionSets.FullName, optionSetsTxt);


        foreach (var table in _selectedTables)
        {
            var serializedTable = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var test = JObject.Parse(serializedTable);

            var fileInfo = new FileInfo($"../../../../../{CoreProjectName}/Definitions/{table.Name}.table");

            var definitionFolderForEnums = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
            if (definitionFolderForEnums.Exists == false) definitionFolderForEnums.Create();


            File.WriteAllText(fileInfo.FullName, serializedTable);
        }

        var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
        if (definitionFolder.Exists == false) definitionFolder.Create();


        MessageBox.Show(@"Definition files generation succeeded");
    }

    private void AddColumnSummary(IndentedStringBuilder sb, Column col)
    {
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// ");
        sb.AppendLine(
            $"/// Type : {col.Type}{(col.EnumName == null ? "" : " (" + col.EnumName + ")")}");
        sb.Append("/// Validity :  ");

        var isFirst = true;
        if ((col.Capabilities & AttributeCapabilities.Read) != AttributeCapabilities.None)
        {
            isFirst = false;
            sb.Append("Read ");
        }

        if ((col.Capabilities & AttributeCapabilities.Create) != AttributeCapabilities.None)
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("| ");
            sb.Append("Create ");
        }

        if ((col.Capabilities & AttributeCapabilities.Update) != AttributeCapabilities.None)
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("| ");
            sb.Append("Update ");
        }

        if ((col.Capabilities & AttributeCapabilities.AdvancedFind) != AttributeCapabilities.None)
        {
            if (isFirst)
                isFirst = false;
            else
                sb.Append("| ");
            sb.Append("AdvancedFind ");
        }

        sb.AppendLine();

        sb.AppendLine("/// </summary>");
    }


    private EntityDefinition TableToBaseEntityDefinition(Table table)
    {
        var entityDefinition = new EntityDefinition
        {
            Name = table.Name + "Definition",
            LogicalCollectionName = table.CollectionName,
            LogicalName = table.LogicalName,
            IsSelected = true
        };

        foreach (var col in table.Columns)
        {
            var attributeDefinition = new AttributeDefinition
            {
                DisplayName = col.Name,
                LogicalName = col.LogicalName,
                Name = col.Name,
                Type = col.Type.ToString(),
                IsValidForCreate = (col.Capabilities & AttributeCapabilities.Create) != 0,
                IsValidForUpdate = (col.Capabilities & AttributeCapabilities.Update) != 0,
                IsValidForAdvancedFind = (col.Capabilities & AttributeCapabilities.AdvancedFind) != 0,
                IsValidForRead = (col.Capabilities & AttributeCapabilities.Read) != 0,
                IsSelected = col.Selected
            };

            entityDefinition.Add(attributeDefinition);
        }


        return entityDefinition;
    }

    private Table EntityDefinitionToBaseTable(EntityDefinition entity)
    {
        Column column;
        var table = new Table
        {
            LogicalName = entity.LogicalName,
            CollectionName = entity.LogicalCollectionName,
            Name = entity.Name
        };

        foreach (var attr in entity.AttributesCollection.Definitions)
        {
            column = new Column
            {
                LogicalName = attr.LogicalName,
                Name = attr.Name,
                Selected = attr.IsSelected
            };

            // Assign Primary type
            if (attr.IsPrimaryIdAttribute)
                column.PrimaryType = PrimaryType.Id;
            else if (attr.IsPrimaryImageAttribute)
                column.PrimaryType = PrimaryType.Image;
            else if (attr.IsPrimaryNameAttribute)
                column.PrimaryType = PrimaryType.Name;
            else
                column.PrimaryType = PrimaryType.None;

            // Assign AttributeTypeCode Type
            if (!string.IsNullOrEmpty(attr.Type))
                column.Type = (AttributeTypeCode)Enum.Parse(typeof(AttributeTypeCode), attr.Type);
            //Assign Capabilities
            if (attr.IsValidForAdvancedFind) column.Capabilities |= AttributeCapabilities.AdvancedFind;
            if (attr.IsValidForCreate) column.Capabilities |= AttributeCapabilities.Create;
            if (attr.IsValidForRead) column.Capabilities |= AttributeCapabilities.Read;
            if (attr.IsValidForUpdate) column.Capabilities |= AttributeCapabilities.Update;
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
            if (attr.Relationships != null && attr.Relationships.Any()) column.Type = AttributeTypeCode.Lookup;

            // Assign keys
            if (attr.KeyNames != null && attr.KeyNames.Any())
                foreach (var keyAttr in attr.KeyNames)
                {
                    var correspondingkey = table.Keys.FirstOrDefault(k => k.Name == keyAttr);
                    if (correspondingkey != null) correspondingkey.FieldNames.Add(attr.LogicalName);
                }
        }


        return table;
    }


    public TableCollection LoadLocalTables()
    {
        var tables = new TableCollection();

        var definitionFolder = new DirectoryInfo($"../../../../../{CoreProjectName}/Definitions");
        if (!definitionFolder.Exists)
        {
            definitionFolder.Create();
        }

        foreach (var fileInfo in definitionFolder.GetFiles("*.table"))
            if (!fileInfo.Name.Contains($"{OptionSetFileName}.table"))
            {
                //MessageBox.Show(fileName);
                var text = File.ReadAllText(fileInfo.FullName);
                var jTable = JObject.Parse(text);


                var currentTable = jTable.ToObject<Table>();


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


        return tables;
    }

    private void getEntitiesFromCRMToolStripMenuItem_Click(object sender, EventArgs e)
    {
        generateDefinitionsToolStripMenuItem.Enabled = false;
        entityListView.Enabled = false;
        attributeListView.Enabled = false;
        getEntitiesFromCRMToolStripMenuItem.Enabled = false;
        DataAccessManager.Instance.Connect(ConnectionSucceeded);
    }
}