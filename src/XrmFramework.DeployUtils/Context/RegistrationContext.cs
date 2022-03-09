using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Context
{
    public class RegistrationContext : IRegistrationContext
    {
        public string SolutionName { get; private set; }
        public Solution Solution { get; set; }
        public Publisher Publisher { get; set; }

        public List<SolutionComponent> Components { get; set; }
        public List<SdkMessageFilter> Filters { get; set; }
        public Dictionary<string, EntityReference> Messages { get; private set; }
        public List<KeyValuePair<string, Guid>> Users { get; private set; }

        public RegistrationContext(string solutionName)
        {
            SolutionName = solutionName;

            Solution = null;
            Publisher = null;
            Components = new List<SolutionComponent>();
            Filters = new List<SdkMessageFilter>();
            Messages = new Dictionary<string, EntityReference>();
            Users = new List<KeyValuePair<string, Guid>>();
        }

        public void InitMetadata(IRegistrationService service, string solutionName)
        {
            SolutionName = solutionName;

            InitMetadata(service);
        }

        public void InitMetadata(IRegistrationService service)
        {
            Console.WriteLine("Metadata initialization");

            InitFilters(service);

            InitMessages(service);

            InitSolution(service);

            InitUsers(service);
        }

        private void InitUsers(IRegistrationService service)
        {
            var query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.DomainName);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.AccessMode, ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.IsDisabled, ConditionOperator.Equal, false);

            foreach (var user in service.RetrieveAll(query))
            {
                Users.Add(new KeyValuePair<string, Guid>(user.GetAttributeValue<string>(SystemUserDefinition.Columns.DomainName), user.Id));
            }
        }

        private void InitSolution(IRegistrationService service)
        {
            var query = new QueryExpression(SolutionDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SolutionDefinition.Columns.UniqueName, ConditionOperator.Equal, SolutionName);

            Solution = service.RetrieveAll(query).Select(s => s.ToEntity<Solution>()).FirstOrDefault();

            if (Solution == null)
            {
                Console.WriteLine("The solution {0} does not exist in the CRM, modify App.config to point to an existing solution.", SolutionName);
                Console.WriteLine("\r\nAppuyez sur une touche pour arrêter.");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            else if (Solution.GetAttributeValue<bool>(SolutionDefinition.Columns.IsManaged))
            {
                Console.WriteLine("The solution {0} is managed in the CRM, modify App.config to point to a development environment.", SolutionName);
                System.Environment.Exit(1);
            }
            else
            {
                Publisher = service.Retrieve(PublisherDefinition.EntityName, Solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();

                query = new QueryExpression(SolutionComponentDefinition.EntityName);
                query.ColumnSet.AllColumns = true;
                query.Criteria.AddCondition(SolutionComponentDefinition.Columns.SolutionId, ConditionOperator.Equal, Solution.Id);

                var components = service.RetrieveAll(query).Select(s => s.ToEntity<SolutionComponent>());

                Components.AddRange(components);
            }
        }

        private void InitMessages(IRegistrationService service)
        {
            var query = new QueryExpression(SdkMessageDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageDefinition.Columns.Id, SdkMessageDefinition.Columns.Name);

            var messages = service.RetrieveAll(query).Select(e => e.ToEntity<SdkMessage>());

            Messages.Clear();
            foreach (SdkMessage e in messages)
            {
                Messages.Add(e.Name, e.ToEntityReference());
            }
        }


        private void InitFilters(IRegistrationService service)
        {
            var query = new QueryExpression(SdkMessageFilterDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageFilterDefinition.Columns.Id,
                                       SdkMessageFilterDefinition.Columns.SdkMessageId,
                                       SdkMessageFilterDefinition.Columns.PrimaryObjectTypeCode);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsCustomProcessingStepAllowed, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsVisible, ConditionOperator.Equal, true);

            var filters = service.RetrieveAll(query);

            Filters.Clear();
            Filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));
        }
    }
}
