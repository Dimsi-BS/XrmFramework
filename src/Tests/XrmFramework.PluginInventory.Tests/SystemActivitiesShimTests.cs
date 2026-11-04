// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using NUnit.Framework;

namespace XrmFramework.PluginInventory.Tests;

/// <summary>
///     Keeps the hand-written <c>System.Activities</c> stand-in in step with what the assemblies it
///     has to load actually ask of it.
/// </summary>
/// <remarks>
///     A stand-in for a framework assembly is only as good as its coverage, and the failure mode
///     when it falls short is nasty: a <c>TypeLoadException</c> naming the USER's workflow activity,
///     with nothing pointing back here. So instead of trusting a reading of the sources, this reads
///     the metadata - every type and member the real assemblies reference out of
///     <c>System.Activities</c> - and asserts the stand-in declares it. A framework change that
///     reaches for a new member then turns this test red at build time, rather than someone's
///     deployment red at six in the evening.
///     <para>
///         Metadata only: nothing here loads an assembly, so it runs on any runtime and says nothing
///         about whether the types can execute. That is what
///         <see cref="CrossTargetFrameworkInventoryTests" /> is for, with the real loader.
///     </para>
/// </remarks>
[TestFixture]
public class SystemActivitiesShimTests
{
    private const string ShimAssemblyName = "System.Activities";

    /// <summary>The assemblies whose System.Activities usage the stand-in has to satisfy.</summary>
    /// <remarks>
    ///     <c>XrmFramework.Workflows.dll</c> is in the list because it ships as SOURCE: in a real
    ///     plugin project its types are compiled into the plugin assembly, so whatever it references
    ///     is referenced by every workflow-enabled plugin.
    /// </remarks>
    private static IEnumerable<string> AssembliesUnderTest()
    {
        foreach (var targetFramework in InventoryHost.FixtureTargetFrameworks)
        {
            var directory = Path.GetDirectoryName(InventoryHost.Fixture(targetFramework))!;
            yield return Path.Combine(directory, "XrmFramework.PluginInventory.Fixtures.dll");
            yield return Path.Combine(directory, "XrmFramework.Workflows.dll");
        }
    }

    // ── Identity ────────────────────────────────────────────────────────────────────

    /// <remarks>
    ///     Load-bearing, and quiet when it breaks. Every net4x plugin references
    ///     <c>System.Activities, Version=4.0.0.0</c>, and a resolver's answer is accepted on its
    ///     path - but keeping the version honest costs nothing and removes the question entirely.
    ///     Mostly this guards against Nerdbank.GitVersioning stamping the repository version over
    ///     it, which would be invisible until workflow activities started disappearing.
    /// </remarks>
    [Test]
    public void Shim_Declares_The_Version_Net4x_Plugins_Reference()
    {
        using var stream = File.OpenRead(InventoryHost.ShimDll);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();
        var definition = reader.GetAssemblyDefinition();

        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(definition.Name), Is.EqualTo(ShimAssemblyName));
            Assert.That(definition.Version, Is.EqualTo(new Version(4, 0, 0, 0)));
        });
    }

    /// <remarks>
    ///     It is the host that must serve System.Activities, so the file has to be beside the host.
    ///     If this ever fails, the packaging stopped carrying it and every workflow activity would
    ///     vanish from the manifests.
    /// </remarks>
    [Test]
    public void Shim_Ships_Beside_The_Inventory_Host()
        => Assert.That(File.Exists(InventoryHost.ShimDll), $"Expected '{InventoryHost.ShimDll}'.");

    // ── Coverage ────────────────────────────────────────────────────────────────────

    [Test]
    public void Shim_Declares_Every_Referenced_Type()
    {
        var declared = DeclaredTypes(InventoryHost.ShimDll);

        var missing = AssembliesUnderTest()
            .SelectMany(ReferencedTypes)
            .Distinct()
            .Where(type => !declared.Contains(type))
            .OrderBy(type => type, StringComparer.Ordinal)
            .ToArray();

        Assert.That(missing, Is.Empty,
            "These System.Activities types are referenced but the stand-in does not declare them. "
            + "Add them to src/XrmFramework.PluginInventory.SystemActivities/SystemActivities.cs, "
            + "matching the real reference assembly's shape:" + Environment.NewLine
            + string.Join(Environment.NewLine, missing));
    }

    /// <remarks>
    ///     Covers members no test ever EXECUTES - <c>Argument.Get</c> and everything else reachable
    ///     only from inside <c>Execute</c> - which is precisely where an inventory would never notice
    ///     a gap but a future code path would.
    ///     <para>
    ///         Matched on type, member name and parameter count rather than on the full signature:
    ///         the goal is to catch "a member appeared that the stand-in has never heard of". Exact
    ///         signature agreement is what the real loader checks, in
    ///         <see cref="CrossTargetFrameworkInventoryTests" />. References whose parent is a generic
    ///         instantiation are not enumerated here either, but the instantiated type itself always
    ///         shows up as a plain type reference, so the type-level test above still covers it.
    ///     </para>
    /// </remarks>
    [Test]
    public void Shim_Declares_Every_Referenced_Member()
    {
        var declared = DeclaredMembers(InventoryHost.ShimDll);

        var missing = AssembliesUnderTest()
            .SelectMany(ReferencedMembers)
            .Distinct()
            .Where(member => !declared.Contains(member))
            .OrderBy(member => member, StringComparer.Ordinal)
            .ToArray();

        Assert.That(missing, Is.Empty,
            "These System.Activities members are referenced but the stand-in does not declare them "
            + "(shown as type.member/parameter-count):" + Environment.NewLine
            + string.Join(Environment.NewLine, missing));
    }

    // ── Metadata helpers ────────────────────────────────────────────────────────────

    private static HashSet<string> DeclaredTypes(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        return reader.TypeDefinitions
            .Select(reader.GetTypeDefinition)
            .Select(type => Join(reader.GetString(type.Namespace), reader.GetString(type.Name)))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static HashSet<string> DeclaredMembers(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        var members = new HashSet<string>(StringComparer.Ordinal);
        foreach (var handle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(handle);
            var typeName = Join(reader.GetString(type.Namespace), reader.GetString(type.Name));

            foreach (var methodHandle in type.GetMethods())
            {
                var method = reader.GetMethodDefinition(methodHandle);
                var signature = method.DecodeSignature(ParameterCounter.Instance, genericContext: null);
                members.Add($"{typeName}.{reader.GetString(method.Name)}/{signature.ParameterTypes.Length}");
            }

            foreach (var fieldHandle in type.GetFields())
                members.Add($"{typeName}.{reader.GetString(reader.GetFieldDefinition(fieldHandle).Name)}/0");
        }

        return members;
    }

    private static IEnumerable<string> ReferencedTypes(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        foreach (var handle in reader.TypeReferences)
        {
            var typeReference = reader.GetTypeReference(handle);
            if (IsScopedToShim(reader, typeReference.ResolutionScope))
                yield return Join(
                    reader.GetString(typeReference.Namespace), reader.GetString(typeReference.Name));
        }
    }

    private static IEnumerable<string> ReferencedMembers(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(stream);
        var reader = peReader.GetMetadataReader();

        foreach (var handle in reader.MemberReferences)
        {
            var memberReference = reader.GetMemberReference(handle);
            if (memberReference.Parent.Kind != HandleKind.TypeReference)
                continue;

            var parent = reader.GetTypeReference((TypeReferenceHandle)memberReference.Parent);
            if (!IsScopedToShim(reader, parent.ResolutionScope))
                continue;

            var typeName = Join(reader.GetString(parent.Namespace), reader.GetString(parent.Name));
            var parameterCount = memberReference.GetKind() == MemberReferenceKind.Method
                ? memberReference.DecodeMethodSignature(ParameterCounter.Instance, genericContext: null)
                    .ParameterTypes.Length
                : 0;

            yield return $"{typeName}.{reader.GetString(memberReference.Name)}/{parameterCount}";
        }
    }

    /// <summary>
    ///     Whether a resolution scope is the <c>System.Activities</c> assembly reference.
    /// </summary>
    /// <remarks>
    ///     A nested type's scope is another type reference, so the chain is walked up to the
    ///     assembly reference it ultimately hangs from.
    /// </remarks>
    private static bool IsScopedToShim(MetadataReader reader, EntityHandle scope)
    {
        while (true)
        {
            switch (scope.Kind)
            {
                case HandleKind.AssemblyReference:
                    var assemblyReference = reader.GetAssemblyReference((AssemblyReferenceHandle)scope);
                    return reader.GetString(assemblyReference.Name) == ShimAssemblyName;

                case HandleKind.TypeReference:
                    scope = reader.GetTypeReference((TypeReferenceHandle)scope).ResolutionScope;
                    continue;

                default:
                    return false;
            }
        }
    }

    private static string Join(string @namespace, string name)
        => string.IsNullOrEmpty(@namespace) ? name : @namespace + "." + name;

    /// <summary>
    ///     Decodes signatures just far enough to count parameters; every type becomes the same token.
    /// </summary>
    private sealed class ParameterCounter : ISignatureTypeProvider<string, object>
    {
        internal static readonly ParameterCounter Instance = new();

        private const string Any = "?";

        public string GetArrayType(string elementType, ArrayShape shape) => Any;
        public string GetByReferenceType(string elementType) => Any;
        public string GetFunctionPointerType(MethodSignature<string> signature) => Any;
        public string GetGenericInstantiation(string genericType, System.Collections.Immutable.ImmutableArray<string> typeArguments) => Any;
        public string GetGenericMethodParameter(object genericContext, int index) => Any;
        public string GetGenericTypeParameter(object genericContext, int index) => Any;
        public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) => Any;
        public string GetPinnedType(string elementType) => Any;
        public string GetPointerType(string elementType) => Any;
        public string GetPrimitiveType(PrimitiveTypeCode typeCode) => Any;
        public string GetSZArrayType(string elementType) => Any;
        public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => Any;
        public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => Any;
        public string GetTypeFromSpecification(MetadataReader reader, object genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => Any;
    }
}
