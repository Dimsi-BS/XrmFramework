// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Workflows
{
    public interface IEnableRuleProvider
    {
        string EntityName { get; }

        IDictionary<string, bool> GetEnableStatus(Guid id);
    }
}