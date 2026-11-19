// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using XrmFramework.RemoteDebugger;
using XrmFramework.RemoteDebugger.Converters;

namespace XrmFramework.DeployUtils.Tests.RemoteDebugger;

/// <summary>
///     The wire format the remote debugger uses has to be read by the OTHER runtime.
/// </summary>
/// <remarks>
///     <para>
///         The runner can be a .NET process; the plugin it debugs is always .NET Framework. Each end
///         serializes a value together with its type name, and the two runtimes do not name types
///         the same way: the core library is <c>System.Private.CoreLib</c> on .NET and
///         <c>mscorlib</c> on .NET Framework. .NET resolves either; .NET Framework resolves only its
///         own. So a runner that wrote <c>System.Private.CoreLib</c> produced a message its plugin
///         could not read - and failed deep in a positional parse with a
///         <c>NullReferenceException</c> naming nothing.
///     </para>
///     <para>
///         These tests run on net8.0, which is the side that regressed. The .NET Framework side takes
///         the same branch by construction - <c>DateTime</c> is in <c>typeof(object).Assembly</c>
///         there too - so what is asserted below is the rule, not one runtime's spelling of it.
///     </para>
/// </remarks>
[TestFixture]
public class ObjectSerializationConverterTests
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new RemoteDebuggerContractResolver(),
    };

    private static string TypeTokenOf(object value)
        => JObject.Parse(JsonConvert.SerializeObject(new ObjectSerialization(value), Settings))
            .Value<string>("Type");

    // ── What goes on the wire ───────────────────────────────────────────────────────

    /// <remarks>
    ///     The regression guard. An assembly-qualified core library type is exactly what the plugin
    ///     cannot resolve, so the name must carry no assembly at all.
    /// </remarks>
    [TestCase("a string")]
    [TestCase(42)]
    [TestCase(true)]
    public void A_Core_Library_Value_Travels_Without_Its_Assembly(object value)
    {
        var typeToken = TypeTokenOf(value);

        Assert.Multiple(() =>
        {
            Assert.That(typeToken, Is.EqualTo(value.GetType().FullName));
            Assert.That(typeToken, Does.Not.Contain(","),
                "An assembly-qualified core library type is unreadable by the other runtime.");
        });
    }

    /// <remarks>The type that broke it in the field: a DateTime in a query condition.</remarks>
    [Test]
    public void A_DateTime_Travels_As_System_DateTime()
        => Assert.That(TypeTokenOf(new DateTime(2026, 11, 9)), Is.EqualTo("System.DateTime"));

    [Test]
    public void A_Guid_Travels_Without_Its_Assembly()
        => Assert.That(TypeTokenOf(Guid.NewGuid()), Is.EqualTo("System.Guid"));

    /// <remarks>
    ///     The other half of the rule. Microsoft.Xrm.Sdk has one identity on both runtimes, so its
    ///     types keep their assembly - dropping it would make the name ambiguous for no benefit.
    /// </remarks>
    [Test]
    public void A_Dataverse_Type_Keeps_Its_Assembly()
    {
        var typeToken = TypeTokenOf(new EntityReference("account", Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(typeToken, Does.StartWith("Microsoft.Xrm.Sdk.EntityReference,"));
            Assert.That(typeToken, Does.Contain("Microsoft.Xrm.Sdk"));
        });
    }

    [Test]
    public void A_Null_Value_Carries_No_Type()
        => Assert.That(TypeTokenOf(null), Is.Null);

    // ── Reading what the other end wrote ────────────────────────────────────────────

    private static ObjectSerialization ReadBack(string typeToken, string valueJson)
        => JsonConvert.DeserializeObject<ObjectSerialization>(
            $"{{\"Type\":{JsonConvert.ToString(typeToken)},\"Value\":{valueJson}}}", Settings);

    /// <remarks>
    ///     A plugin deployed from an older build still sends fully qualified core library types, and
    ///     a .NET runner has to keep reading them. This is the mscorlib spelling, which a .NET
    ///     runtime cannot resolve as written - the fallback drops the assembly and retries.
    /// </remarks>
    [Test]
    public void A_Legacy_Fully_Qualified_Core_Type_Is_Still_Read()
    {
        var read = ReadBack(
            "System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            "\"2026-11-09T00:00:00\"");

        Assert.That(read.Value, Is.EqualTo(new DateTime(2026, 11, 9)));
    }

    /// <remarks>
    ///     The failure mode itself. An unresolvable type used to leave the value token unread, and
    ///     since everything here is parsed positionally the rest of the message was then read against
    ///     the wrong tokens - surfacing as a NullReferenceException far from the cause. The value is
    ///     lost either way; what matters is that the reader stays aligned and says so quietly.
    /// </remarks>
    [Test]
    public void An_Unresolvable_Type_Does_Not_Derail_The_Reader()
    {
        var read = ReadBack("Some.Type.That.Does.Not.Exist, NoSuchAssembly", "{\"a\":1,\"b\":[2,3]}");

        Assert.That(read.Value, Is.Null);
    }

    // ── The shape that actually failed in the field ─────────────────────────────────

    /// <remarks>
    ///     A RetrieveMultiple whose conditions carry a DateTime range, which is what a .NET runner
    ///     sent to a .NET Framework plugin when this broke. Round-tripped through the same settings
    ///     both ends use.
    /// </remarks>
    [Test]
    public void A_Query_With_Date_Conditions_Round_Trips()
    {
        var query = new QueryExpression("dimsi_cra");
        query.Criteria.AddCondition("dimsi_debut", ConditionOperator.GreaterEqual, new DateTime(2026, 11, 9));
        query.Criteria.AddCondition("dimsi_fin", ConditionOperator.LessEqual, new DateTime(2026, 11, 15, 22, 0, 0));
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

        var json = JsonConvert.SerializeObject(query, Settings);

        Assert.That(json, Does.Not.Contain("System.Private.CoreLib"),
            "A .NET Framework plugin cannot resolve System.Private.CoreLib.");
        Assert.That(json, Does.Not.Contain("mscorlib"),
            "A core library type should travel unqualified, whichever runtime wrote it.");

        var readBack = JsonConvert.DeserializeObject<QueryExpression>(json, Settings);

        var values = readBack.Criteria.Conditions.SelectMany(c => c.Values).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(readBack.Criteria.Conditions, Has.Count.EqualTo(3));
            Assert.That(values, Does.Contain(new DateTime(2026, 11, 9)));
            Assert.That(values, Does.Contain(new DateTime(2026, 11, 15, 22, 0, 0)));
            Assert.That(values, Does.Contain(0));
        });
    }
}
