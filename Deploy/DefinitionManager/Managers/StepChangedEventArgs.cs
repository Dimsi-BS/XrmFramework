
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
