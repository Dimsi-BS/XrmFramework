using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlTransformAttribute : Attribute
    {
        public XmlTransformAttribute(Type actionType)
        {
            ActionType = actionType;
        }

        public Type ActionType { get; private set; }
    }
}
