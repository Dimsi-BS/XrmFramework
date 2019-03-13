// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DateTimeBehaviorAttribute : Attribute
    {
        public DateTimeBehaviorAttribute(DateTimeBehavior behavior)
        {
            Behavior = behavior;
        }

        public DateTimeBehavior Behavior
        {
            get;
            private set;
        }
    }
}
