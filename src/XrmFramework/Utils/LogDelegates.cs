// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework
{
    public delegate void LogServiceMethod(string methodName, string message, params object[] args);

    public delegate void LogMethod(string message, params object[] args);
}