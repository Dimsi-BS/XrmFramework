using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins
{
    /// <summary>
    /// All Services related to Account 
    /// </summary>
    public class AccountService : DefaultService, IAccountService
    {
        #region .ctor

        public AccountService(IServiceContext context) : base(context)
        {
        }

        #endregion

        public ICollection<EntityReference> GetSubContactRefs(EntityReference accountRef)
        {
            var query = new QueryExpression(ContactDefinition.EntityName);
            query.ColumnSet.AddColumn(ContactDefinition.Columns.Id);

            query.Criteria.AddCondition(ContactDefinition.Columns.ParentCustomerId, ConditionOperator.Equal, accountRef.Id);

            return AdminOrganizationService.RetrieveAll(query).Select(e => e.ToEntityReference()).ToList();
        }
    }
}
