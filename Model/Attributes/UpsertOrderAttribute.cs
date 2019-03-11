using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UpsertOrderAttribute : Attribute
    {
        public UpsertOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; private set; }
    }
}
