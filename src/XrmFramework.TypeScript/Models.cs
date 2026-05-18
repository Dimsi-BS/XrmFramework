// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

internal class Table
{
    public string? LogName { get; set; }

    public List<OptionSet> Enums { get; } = new();
}

internal class OptionSet
{
    public string Name { get; set; }

    public List<OptionSetValue> Values { get; } = new();
}

internal class OptionSetValue
{
    public string Name { get; set; }
    public int Value { get; set; }
}
