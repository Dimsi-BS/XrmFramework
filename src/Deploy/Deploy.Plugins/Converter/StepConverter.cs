// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploy.Utils
{
    public static class StepConverter
    {
        public static Step ConvertToDeployStep(global::Plugins.Step s)
        {
            var step = new Step(s.Plugin.GetType().Name, StaticsConverter.Convert(s.Message), StaticsConverter.Convert(s.Stage), StaticsConverter.Convert(s.Mode), s.EntityName);

            step.FilteredAttributes = s.FilteredAttributes;
            step.ImpersonationUsername = s.ImpersonationUsername;
            step.Order = s.Order;
            step.PostImageAllAttributes = s.PostImageAllAttributes;
            step.PostImageAttributes = s.PostImageAttributes;
            step.PostImageUsed = s.PostImageUsed;
            step.PreImageAllAttributes = s.PreImageAllAttributes;
            step.PreImageAttributes = s.PreImageAttributes;
            step.PreImageUsed = s.PreImageUsed;
            step.UnsecureConfig = s.UnsecureConfig;

            step.MethodNames.AddRange(s.MethodNames);

            return step;
        }
    }
}
