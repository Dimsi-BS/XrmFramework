// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DefinitionManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XrmFramework.Core;

namespace XrmFramework.DefinitionManager;

public partial class MainForm : Form, ICustomListProvider
{
    private const string OptionSetFileName = "CustomOptionSets";
    private readonly DefinitionCollection<EntityDefinition> _entityCollection;
    private readonly List<OptionSetEnum> _enums = new();

    private readonly TableCollection _localTables;
    private readonly TableCollection _selectedTables;
    private readonly TableCollection _tables;

    private readonly CoreProjectAttribute _coreProject;

    public MainForm()
    {
        CustomProvider.Instance = this;

        _coreProject = Assembly.GetCallingAssembly().GetCustomAttribute<CoreProjectAttribute>();

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

            if (table.Columns.Any())
            {
                _entityCollection.Add(localEntity);
                _tables.Add(table);
            }

            _enums.AddRange(table.Enums);
        }

        foreach (var @enum in _enums)
        {
            var enumDefinition = new EnumDefinition
            {
                LogicalName = @enum.LogicalName,
                Name = @enum.Name,
                IsGlobal = @enum.IsGlobal
            };

            foreach (var value in @enum.Values)
            {
                enumDefinition.Values.Add(new EnumValueDefinition
                {
                    Name = value.Name,
                    LogicalName = value.ToString(),
                    Value = value.ToString()
                });
            }

            EnumDefinitionCollection.Instance.Add(enumDefinition);
        }

        generateDefinitionsToolStripMenuItem.Enabled = true;
        entityListView.Enabled = true;
        attributeListView.Enabled = true;
    }

    public T GetCustomList<T>()
    {
        var result = GetCustomList(typeof(T));

        return (T)result;
    }


    public object GetCustomList(Type type)
    {
        foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (type.IsAssignableFrom(field.FieldType))
            {
                return field.GetValue(this);
            }
        }

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
        _enums.AddRange(entities.Item3);
        _tables.AddRange(entities.Item2);
        MessageBox.Show($@"There are {_localTables.Count} local tables");
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
        _entityCollection.AttachListView(entityListView);
    }

    private void generateDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        foreach (var item in _entityCollection.SelectedDefinitions)
            if (_selectedTables.All(t => t.LogicalName != item.LogicalName))
            {
                var table = _tables.Single(t => t.LogicalName == item.LogicalName);
                table.Selected = true;
                foreach (var a in item.AttributesCollection.SelectedDefinitions)
                    table.Columns.First(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;

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
                    table.Columns.First(c => c.LogicalName == a.LogicalName).Selected = a.IsSelected;


                _selectedTables.Add(table);
            }

        var nameOfTablesToDelete = new List<string>();
        // Delete tables that aren't selected
        foreach (var table in _selectedTables)
            if (_entityCollection.SelectedDefinitions.All(entityDefinition => entityDefinition.LogicalName != table.LogicalName))
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

        var definitionFolder = new DirectoryInfo($"{_coreProject.RootFolder}/{_coreProject.Name}/Definitions");
        if (!definitionFolder.Exists)
        {
            definitionFolder.Create();
        }

        var fileInfoOptionSets =
            new FileInfo($"{definitionFolder.FullName}/{globalOptionSets.Name}.table");

        File.WriteAllText(fileInfoOptionSets.FullName, optionSetsTxt);

        foreach (var table in _selectedTables)
        {
            var serializedTable = JsonConvert.SerializeObject(table, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            var fileInfo = new FileInfo($"{definitionFolder.FullName}/{table.Name}.table");

            File.WriteAllText(fileInfo.FullName, serializedTable);
        }

        MessageBox.Show(@"Definition files generation succeeded");
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

        var forceSelect = table.Columns.All(c => !c.Selected);

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
                IsSelected = col.Selected || forceSelect
            };

            entityDefinition.Add(attributeDefinition);
        }

        return entityDefinition;
    }

    private TableCollection LoadLocalTables()
    {
        var tables = new TableCollection();

        var definitionFolder = new DirectoryInfo($"{_coreProject.RootFolder}/{_coreProject.Name}/Definitions");
        if (!definitionFolder.Exists)
        {
            definitionFolder.Create();
        }

        foreach (var fileInfo in definitionFolder.GetFiles("*.table"))
        {
            var text = File.ReadAllText(fileInfo.FullName);
            var jTable = JObject.Parse(text);

            var currentTable = jTable.ToObject<Table>();

            var columns = jTable["Cols"] ?? Enumerable.Empty<JToken>();

            foreach (var jColumn in columns)
            {
                var currentColumn = jColumn.ToObject<Column>();
                currentTable.Columns.Add(currentColumn);
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