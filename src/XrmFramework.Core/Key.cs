using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.Core;

[JsonObject(MemberSerialization.OptOut)]
public class Key
{
    public string LogicalName { get; set; }

    public string Name { get; set; }

    public List<string> FieldNames { get; } = new();

    /// <summary>
    /// The name the CRM knows this key under, whichever property the file carries it in.
    /// </summary>
    /// <remarks>
    /// <see cref="LogicalName" /> only appeared once the pull tool started deriving <see cref="Name" />
    /// from the key's label. The DefinitionManager that wrote the earlier files put the logical name
    /// in <see cref="Name" /> and left <see cref="LogicalName" /> empty, and those files are the ones
    /// a project compiles against until its next pull.
    /// </remarks>
    [JsonIgnore]
    public string EffectiveLogicalName => string.IsNullOrEmpty(LogicalName) ? Name : LogicalName;

    /// <summary>
    /// Name of the constant standing for this key in the generated <c>AlternateKeyNames</c> class.
    /// </summary>
    [JsonIgnore]
    public string MemberName => string.IsNullOrEmpty(Name) ? LogicalName : Name;
}
