// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workflows;

namespace Deploy.Utils
{
    public static class PluginConverter
    {

        public static Plugin Convert(global::Plugins.Plugin p)
        {
            var plugin = new Plugin(p.GetType().FullName);

            var stepComparer = new StepEqualityComparer();

            var stepSet = new HashSet<global::Plugins.Step>(stepComparer);

            foreach (var s in p.Steps)
            {
                if (stepSet.Contains(s))
                {
                    var existingStep = stepSet.Single(step => stepComparer.Equals(step, s));
                    existingStep.Merge(s);
                    continue;
                }
                stepSet.Add(s);
            }

            foreach (var s in stepSet)
            {
                plugin.Steps.Add(StepConverter.ConvertToDeployStep(s));
            }

            return plugin;
        }

        public static Plugin Convert(CustomWorkflowActivity wf)
        {
            var plugin = new Plugin(wf.GetType().FullName, wf.DisplayName);

            return plugin;
        }

        private class StepEqualityComparer : IEqualityComparer<global::Plugins.Step>
        {

            public bool Equals(global::Plugins.Step x, global::Plugins.Step y)
            {
                return x.EntityName == y.EntityName && x.Mode == y.Mode && x.Stage == y.Stage && x.Message == y.Message;
            }

            public int GetHashCode(global::Plugins.Step obj)
            {
                return obj.EntityName.GetHashCode() ^ obj.Mode.GetHashCode() ^ obj.Stage.GetHashCode() ^ obj.Message.GetHashCode();
            }
        }
    }
}
