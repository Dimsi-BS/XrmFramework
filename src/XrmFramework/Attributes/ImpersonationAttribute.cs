// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ImpersonationAttribute : Attribute
    {
        public ImpersonationAttribute(string impersonationUserName)
        {
            ImpersonationUsername = impersonationUserName;
        }

        public string ImpersonationUsername { get; private set; }
    }
}
