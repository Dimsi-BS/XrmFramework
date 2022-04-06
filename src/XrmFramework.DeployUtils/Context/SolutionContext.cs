using Deploy;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Context
{
    public class SolutionContext : ISolutionContext
    {
        public SolutionContext(IRegistrationService service, IOptions<SolutionSettings> settings)
        {
            _service = service;

            SolutionName = settings.Value.PluginSolutionUniqueName;

            InitSolution();
        }
        private readonly IRegistrationService _service;

        private Solution _solution = null;
        private Publisher _publisher = null;

        private readonly List<SolutionComponent> _components = new();
        private readonly List<SdkMessageFilter> _filters = new();
        private readonly Dictionary<Messages, EntityReference> _messages = new();
        private readonly List<KeyValuePair<string, Guid>> _users = new();



        public string SolutionName { get; }
        public Solution Solution => _solution ?? InitSolution();
        public Publisher Publisher => _publisher ?? InitPublisher();

        public List<SolutionComponent> Components => _components.Count == 0 ? InitComponents() : _components;
        public List<SdkMessageFilter> Filters => _filters.Count == 0 ? InitFilters() : _filters;
        public Dictionary<Messages, EntityReference> Messages => _messages.Count == 0 ? InitMessages() : _messages;
        public List<KeyValuePair<string, Guid>> Users => _users.Count == 0 ? InitUsers() : _users;



        public void InitMetadata()
        {
            Console.WriteLine("Metadata initialization");

            InitFilters();

            InitMessages();

            InitPublisher();

            InitComponents();

            InitUsers();
        }

        private List<KeyValuePair<string, Guid>> InitUsers()
        {
            var query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.DomainName);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.AccessMode, ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.IsDisabled, ConditionOperator.Equal, false);

            foreach (var user in _service.RetrieveAll(query))
            {
                _users.Add(new KeyValuePair<string, Guid>(user.GetAttributeValue<string>(SystemUserDefinition.Columns.DomainName), user.Id));
            }
            return _users;
        }


        private Solution InitSolution()
        {
            var query = new QueryExpression(SolutionDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SolutionDefinition.Columns.UniqueName, ConditionOperator.Equal, SolutionName);

            _solution = _service.RetrieveAll(query).Select(s => s.ToEntity<Solution>()).FirstOrDefault();

            if (_solution == null)
            {
                Console.WriteLine("The solution {0} does not exist in the CRM, modify App.config to point to an existing solution.", SolutionName);
                Console.WriteLine("\r\nAppuyez sur une touche pour arrêter.");
                Console.ReadKey();
                System.Environment.Exit(1);
            }
            else if (_solution.GetAttributeValue<bool>(SolutionDefinition.Columns.IsManaged))
            {
                Console.WriteLine("The solution {0} is managed in the CRM, modify App.config to point to a development environment.", SolutionName);
                System.Environment.Exit(1);
            }
            return _solution;
        }
        private Publisher InitPublisher()
        {
            _publisher = _service.Retrieve(PublisherDefinition.EntityName, Solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();
            return _publisher;
        }

        private List<SolutionComponent> InitComponents()
        {
            var query = new QueryExpression(SolutionComponentDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SolutionComponentDefinition.Columns.SolutionId, ConditionOperator.Equal, Solution.Id);

            var components = _service.RetrieveAll(query).Select(s => s.ToEntity<SolutionComponent>());

            _components.AddRange(components);

            return _components;
        }

        private Dictionary<Messages, EntityReference> InitMessages()
        {
            var query = new QueryExpression(SdkMessageDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageDefinition.Columns.Id, SdkMessageDefinition.Columns.Name);

            var messages = _service.RetrieveAll(query).Select(e => e.ToEntity<SdkMessage>());

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(XrmFramework.Messages.GetMessage(e.Name), e.ToEntityReference());
            }
            return _messages;
        }

        private List<SdkMessageFilter> InitFilters()
        {
            var query = new QueryExpression(SdkMessageFilterDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageFilterDefinition.Columns.Id,
                                       SdkMessageFilterDefinition.Columns.SdkMessageId,
                                       SdkMessageFilterDefinition.Columns.PrimaryObjectTypeCode);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsCustomProcessingStepAllowed, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsVisible, ConditionOperator.Equal, true);

            var filters = _service.RetrieveAll(query);

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));

            return _filters;
        }
    }
}
