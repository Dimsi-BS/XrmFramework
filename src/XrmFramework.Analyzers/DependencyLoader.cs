// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace XrmFramework.Analyzers;

/// <summary>
/// Roslyn source generators run in an isolated AssemblyLoadContext that does not
/// automatically probe the generator's directory for dependencies.
/// This loader embeds Newtonsoft.Json.dll as a manifest resource inside the analyzer
/// assembly and resolves it at runtime by hooking the ALC's <see cref="AssemblyLoadContext.Resolving"/>
/// event — which fires inside the same isolation context that Roslyn creates for analyzers.
/// Call <see cref="EnsureLoaded"/> in the static constructor of every generator class.
/// </summary>
internal static class DependencyLoader
{
    private static bool _registered;
    private static readonly object Lock = new();

    internal static void EnsureLoaded()
    {
        if (_registered) return;
        lock (Lock)
        {
            if (_registered) return;

            // Hook the AssemblyLoadContext that loaded THIS assembly.
            // In Roslyn's isolated generator host (both VS and dotnet build), each analyzer
            // runs in its own ALC. Hooking alc.Resolving is the correct interception point
            // in .NET Core — AppDomain.AssemblyResolve only fires for the *default* ALC and
            // is not called when resolution fails inside a custom ALC.
            try
            {
                var alc = AssemblyLoadContext.GetLoadContext(typeof(DependencyLoader).Assembly);
                if (alc != null)
                    alc.Resolving += OnAlcResolving;
            }
            catch
            {
                // Running on .NET Framework (no ALC support) — fall back to AppDomain.
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            }

            _registered = true;
        }
    }

    // .NET Core path: called by the ALC that owns the analyzer when it cannot find Newtonsoft.Json.
    // The Resolving event is only raised after the ALC has already failed to locate the assembly,
    // so there is no need to check for an already-loaded copy here.
    private static Assembly? OnAlcResolving(AssemblyLoadContext context, AssemblyName name)
    {
        if (name.Name != "Newtonsoft.Json") return null;

        var stream = GetNewtonsoftStream();
        if (stream is null) return null;

        using (stream)
            // Load into the requesting context so type-identity is consistent within the ALC.
            return context.LoadFromStream(stream);
    }

    // .NET Framework fallback path.
    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        if (new AssemblyName(args.Name).Name != "Newtonsoft.Json") return null;

        foreach (var loaded in AppDomain.CurrentDomain.GetAssemblies())
            if (loaded.GetName().Name == "Newtonsoft.Json") return loaded;

        var stream = GetNewtonsoftStream();
        if (stream is null) return null;

        byte[] data;
        using (stream)
        {
            data = new byte[stream.Length];
            _ = stream.Read(data, 0, data.Length);
        }

#pragma warning disable RS1035
        return Assembly.Load(data);
#pragma warning restore RS1035
    }

    private static Stream? GetNewtonsoftStream()
        => typeof(DependencyLoader).Assembly.GetManifestResourceStream("Newtonsoft.Json.dll");
}
