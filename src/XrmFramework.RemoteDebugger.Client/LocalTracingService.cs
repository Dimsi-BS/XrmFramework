// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using Spectre.Console;

namespace XrmFramework.RemoteDebugger.Common
{
    public class LocalTracingService : ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            AnsiConsole.WriteLine(format, args);
        }
    }
}