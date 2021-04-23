// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    class StepChangedEventArgs
    {
        public string StepName { get; set; }

        public int Maximum { get; set; }

        public int Current { get; set; }
    }
}
