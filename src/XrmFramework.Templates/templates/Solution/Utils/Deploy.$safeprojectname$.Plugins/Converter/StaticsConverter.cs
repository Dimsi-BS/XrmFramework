// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using XrmFramework.DeployUtils.Model;

namespace Deploy
{
    public static class StaticsConverter
    {
        public static string Convert(global::XrmFramework.Messages message)
        {
            return message.ToString();
        }
        public static Modes Convert(global::XrmFramework.Modes mode)
        {
            return (Modes)Enum.ToObject(typeof(Modes), (int)mode);
        }
        public static Stages Convert(global::XrmFramework.Stages stage)
        {
            return (Stages)Enum.ToObject(typeof(Stages), (int)stage);
        }
    }
}
