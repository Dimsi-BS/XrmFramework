using Deploy;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Context
{
    /// <summary>
    /// Base implementation of <see cref="ISolutionContext"/>
    /// </summary>
    public class SolutionContext : ISolutionContext
    {
        public SolutionContext(IRegistrationService service, IOptions<DeploySettings> settings)
        {
            _service = service;

            SolutionName = settings.Value.PluginSolutionUniqueName;
        }
        private readonly IRegistrationService _service;

        #region Private fields implementing the interface
        private Solution _solution;
        private Publisher _publisher;
        private readonly List<SolutionComponent> _components = new();
        private readonly List<SdkMessageFilter> _filters = new();
        private readonly Dictionary<Messages, EntityReference> _messages = new();
        private readonly Dictionary<string, Guid> _users = new();
        #endregion

        public string SolutionName { get; private set; }
        public Solution Solution => _solution;
        public Publisher Publisher => _publisher;

        public void InitSolutionContext(string solutionName = null)
        {
            if (solutionName != null)
            {
                SolutionName = solutionName;
            }

            _components.Clear();
            _filters.Clear();
            _messages.Clear();
            _users.Clear();

            InitSolution();
            InitPublisher();
            InitComponents();
        }

        public Guid GetUserId(string userName)
        {
            var userId = _users[userName];

            if (userId == default)
                throw new Exception($"User with full name {userName} was requested but not found on the CRM");

            return userId;
        }

        public void InitMetadata()
        {
            Console.WriteLine("Metadata initialization");

            InitSolution();

            InitPublisher();

            InitComponents();

            InitFilters();

            InitMessages();

            InitUsers();
        }

        public void InitExportMetadata(IEnumerable<Step> steps)
        {
            if (!steps.Any()) return;
            ImportUsersForSteps(steps);
            ImportMessagesForSteps(steps);
            ImportFiltersForSteps(steps);
        }

        /// <summary>Retrieves and store the <see cref="Solution"/> field</summary>
        private void InitSolution()
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
        }

        /// <summary>Retrieves and store the <see cref="Publisher"/> field</summary>
        private void InitPublisher()
        {
            _publisher = _service.Retrieve(PublisherDefinition.EntityName, Solution.PublisherId.Id, new ColumnSet(true)).ToEntity<Publisher>();
        }

        /// <summary>Retrieves and store the <see cref="Components"/> field</summary>
        private void InitComponents()
        {
            var query = new QueryExpression(SolutionComponentDefinition.EntityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition(SolutionComponentDefinition.Columns.SolutionId, ConditionOperator.Equal, Solution.Id);

            var components = _service.RetrieveAll(query).Select(s => s.ToEntity<SolutionComponent>());

            _components.AddRange(components);
        }

        /// <summary>Retrieves and store the <see cref="Users"/> field related to at least one of the given <paramref name="steps"/></summary>
        private void ImportUsersForSteps(IEnumerable<Step> steps)
        {
            var interestingUsers = steps
                .GroupBy(s => s.ImpersonationUsername)
                .Select(g => g.Key);

            var query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.DomainName);

            query.Criteria.AddCondition(SystemUserDefinition.Columns.AccessMode, ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.IsDisabled, ConditionOperator.Equal, false);

            query.Criteria.AddCondition(SystemUserDefinition.Columns.DomainName, ConditionOperator.NotNull);

            query.Criteria.AddCondition(SystemUserDefinition.Columns.DomainName, ConditionOperator.In, interestingUsers.ToArray<object>());

            foreach (var user in _service.RetrieveAll(query))
            {
                _users.Add(user.GetAttributeValue<string>(SystemUserDefinition.Columns.DomainName), user.Id);
            }
        }

        /// <summary>Retrieves and store the <see cref="Filters"/> field related to at least one of the given <paramref name="steps"/></summary>
        private void ImportFiltersForSteps(IEnumerable<Step> steps)
        {
            var interestingMessages = steps
                .GroupBy(m => m.Message)
                .Select(g => g.Key.ToString());

            var iterestingEntityNames = steps
                .GroupBy(s => s.EntityName)
                .Select(g => g.Key);

            var query = new QueryExpression(SdkMessageFilterDefinition.EntityName);

            query.ColumnSet.AddColumns(SdkMessageFilterDefinition.Columns.Id,
                SdkMessageFilterDefinition.Columns.SdkMessageId,
                SdkMessageFilterDefinition.PrimaryObjectTypeCode);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsCustomProcessingStepAllowed, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsVisible, ConditionOperator.Equal, true);

            query.Criteria.AddCondition(SdkMessageFilterDefinition.PrimaryObjectTypeCode, ConditionOperator.In, iterestingEntityNames.ToArray<object>());

            var messageLink = query.AddLink(SdkMessageDefinition.EntityName,
                SdkMessageFilterDefinition.Columns.SdkMessageId, SdkMessageDefinition.Columns.Id);
            messageLink.LinkCriteria.AddCondition(SdkMessageDefinition.Columns.Name, ConditionOperator.In, interestingMessages.ToArray<object>());

            var filters = _service.RetrieveAll(query);

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));
        }

        /// <summary>Retrieves and store the <see cref="Messages"/> field related to at least one of the given <paramref name="steps"/></summary>
        private void ImportMessagesForSteps(IEnumerable<Step> steps)
        {
            var interestingMessages = steps
                .GroupBy(m => m.Message)
                .Select(g => g.Key.ToString());

            var query = new QueryExpression(SdkMessageDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageDefinition.Columns.Id, SdkMessageDefinition.Columns.Name);

            query.Criteria.AddCondition(SdkMessageDefinition.Columns.Name, ConditionOperator.In, interestingMessages.ToArray<object>());

            var messages = _service.RetrieveAll(query).Select(e => e.ToEntity<SdkMessage>());

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(Messages.GetMessage(e.Name), e.ToEntityReference());
            }
        }

        /// <summary>Retrieves and store the <see cref="Users"/> field</summary>
        private void InitUsers()
        {
            var query = new QueryExpression(SystemUserDefinition.EntityName);
            query.ColumnSet.AddColumn(SystemUserDefinition.Columns.DomainName);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.AccessMode, ConditionOperator.NotEqual, 3);
            query.Criteria.AddCondition(SystemUserDefinition.Columns.IsDisabled, ConditionOperator.Equal, false);

            foreach (var user in _service.RetrieveAll(query))
            {
                _users.Add(user.GetAttributeValue<string>(SystemUserDefinition.Columns.DomainName), user.Id);
            }
        }

        /// <summary>Retrieves and store the <see cref="Messages"/> field</summary>
        private void InitMessages()
        {
            var query = new QueryExpression(SdkMessageDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageDefinition.Columns.Id, SdkMessageDefinition.Columns.Name);

            var messages = _service.RetrieveAll(query).Select(e => e.ToEntity<SdkMessage>());

            _messages.Clear();
            foreach (SdkMessage e in messages)
            {
                _messages.Add(XrmFramework.Messages.GetMessage(e.Name), e.ToEntityReference());
            }
        }

        /// <summary>Retrieves and store the <see cref="Filters"/> field</summary>
        private void InitFilters()
        {
            var query = new QueryExpression(SdkMessageFilterDefinition.EntityName);
            query.ColumnSet.AddColumns(SdkMessageFilterDefinition.Columns.Id,
                                       SdkMessageFilterDefinition.Columns.SdkMessageId,
                                       SdkMessageFilterDefinition.PrimaryObjectTypeCode);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsCustomProcessingStepAllowed, ConditionOperator.Equal, true);
            query.Criteria.AddCondition(SdkMessageFilterDefinition.Columns.IsVisible, ConditionOperator.Equal, true);

            var filters = _service.RetrieveAll(query);

            _filters.Clear();
            _filters.AddRange(filters.Select(f => f.ToEntity<SdkMessageFilter>()));
        }

        public SolutionComponent GetComponentByObjectRef(EntityReference objectRef)
        {
            return _components.FirstOrDefault(c => c.ObjectId.Equals(objectRef.Id));
        }

        public EntityReference GetMessage(Messages message)
        {
            return _messages[message];
        }

        public EntityReference GetMessageFilter(Messages message, string entityName)
        {
            return _filters.FirstOrDefault(f =>
                f.SdkMessageId.Name == message.ToString()
                && f.PrimaryObjectTypeCode == entityName)
                ?.ToEntityReference();
        }
    }
}
