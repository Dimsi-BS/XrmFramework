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
