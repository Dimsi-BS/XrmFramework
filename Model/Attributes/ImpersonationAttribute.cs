using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ImpersonationAttribute : Attribute
    {
        public ImpersonationAttribute(string impersonationUserName)
        {
            ImpersonationUsername = impersonationUserName;
        }

        public string ImpersonationUsername { get; private set; }
    }
}
