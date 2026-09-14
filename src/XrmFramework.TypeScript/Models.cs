// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

using Newtonsoft.Json;

internal class Table
{
    public string? LogName { get; set; }

    public List<Column> Cols { get; } = new();

    public List<OptionSet> Enums { get; } = new();
}

internal class Column
{
    public string? EnumName { get; set; }

    [JsonProperty("Select")]
    public bool Selected { get; set; }
}

internal class OptionSet
{
    [JsonProperty("LogName")]
    public string? LogicalName { get; set; }

    public string Name { get; set; }

    public List<OptionSetValue> Values { get; } = new();
}

internal class OptionSetValue
{
    public string Name { get; set; }
    public int Value { get; set; }
}
