using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IBehaviour<in T> where T:IXmlModel
    {
        void ApplyBehaviour(IOrganizationService service, T model);
    }
}
