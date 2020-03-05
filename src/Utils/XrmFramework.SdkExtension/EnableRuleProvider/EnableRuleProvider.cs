// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Model;
using Plugins;

namespace Workflows
{
    public abstract class EnableRuleProvider : IEnableRuleProvider
    {
        protected ICustomWorkflowContext Context { get; }
        private readonly List<EnableRule> _enableRules = new List<EnableRule>();
        private bool _isRuleRegistered;

        public abstract string EntityName { get; }

        protected EnableRuleProvider(ICustomWorkflowContext context)
        {
            Context = context;
        }

        protected TService GetService<TService>() where TService : IService
        {
            return Context.GetService<TService>();
        }

        protected abstract void AddRules();

        private readonly List<Guid> RoleIds = new List<Guid>();

        protected bool HasRole(string roleIdTxt)
        {
            if (!Guid.TryParse(roleIdTxt, out var roleId))
            {
                throw new Exception($"The passed roleId ({roleIdTxt}) cannot be parsed has a Guid");
            }

            return HasRole(roleId);
        }

        protected bool HasRole(Guid roleId)
        {
            if (!RoleIds.Any())
            {
                var service = GetService<IService>();
                RoleIds.AddRange(service.GetUserRoleIds(new EntityReference(SystemUserDefinition.EntityName, Context.UserId)));
            }

            return RoleIds.Contains(roleId);
        }

        public IDictionary<string, bool> GetEnableStatus(Guid id)
        {
            if (!_isRuleRegistered)
            {
                AddRules();
                _isRuleRegistered = true;
            }

            var results = _enableRules.SelectMany(e =>
            {
                var result = (id != Guid.Empty || e.AllowEmptyId) && e.RuleCallback(id);

                return e.ButtonIds.Select(buttonId => new EnableRuleResult { ButtonId = buttonId, Result = result, OrRuleGroupName = e.OrRuleGroupName });
            });

            return results.GroupBy(e => e.ButtonId).ToDictionary(e => e.Key, enableRuleResults =>
            {
                var globalResult = true;

                foreach (var resultGroup in enableRuleResults.GroupBy(result => result.OrRuleGroupName))
                {
                    if (resultGroup.Key == null)
                    {
                        globalResult &= resultGroup.Any(r => r.Result);
                    }
                    else
                    {
                        globalResult &= resultGroup.All(r => r.Result);
                    }
                }

                return globalResult;
            });
        }

        protected void AddEnableRule(Func<Guid, bool> ruleCallback, string buttonId)
        {
            _enableRules.Add(new EnableRule(ruleCallback, false, buttonId));
        }

        protected void AddEnableRule(Func<Guid, bool> ruleCallback, bool allowEmptyId, string buttonId)
        {
            _enableRules.Add(new EnableRule(ruleCallback, allowEmptyId, buttonId));
        }

        protected void AddEnableRule(Func<Guid, bool> ruleCallback, params string[] buttonIds)
        {
            _enableRules.Add(new EnableRule(ruleCallback, false, buttonIds));
        }

        protected void AddEnableRule(Func<Guid, bool> ruleCallback, bool allowEmptyId, params string[] buttonIds)
        {
            _enableRules.Add(new EnableRule(ruleCallback, allowEmptyId, buttonIds));
        }

        protected void AddOrEnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, string buttonId)
        {
            _enableRules.Add(new EnableRule(orRuleGroupName, ruleCallback, false, buttonId));
        }

        protected void AddOrEnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, bool allowEmptyId, string buttonId)
        {
            _enableRules.Add(new EnableRule(orRuleGroupName, ruleCallback, allowEmptyId, buttonId));
        }

        protected void AddOrEnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, params string[] buttonIds)
        {
            _enableRules.Add(new EnableRule(orRuleGroupName, ruleCallback, false, buttonIds));
        }

        protected void AddOrEnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, bool allowEmptyId, params string[] buttonIds)
        {
            _enableRules.Add(new EnableRule(orRuleGroupName, ruleCallback, allowEmptyId, buttonIds));
        }

        private class EnableRuleResult
        {
            public string ButtonId { get; set; }
            public bool Result { get; set; }
            public string OrRuleGroupName { get; set; }
        }
        public class EnableRule
        {
            public List<string> ButtonIds { get; } = new List<string>();

            public string OrRuleGroupName { get; }

            public Func<Guid, bool> RuleCallback { get; }

            public bool AllowEmptyId { get; }

            public EnableRule(Func<Guid, bool> ruleCallback, bool allowEmptyId, string buttonId)
            {
                ButtonIds.Add(buttonId);
                RuleCallback = ruleCallback;
                AllowEmptyId = allowEmptyId;
            }
            public EnableRule(Func<Guid, bool> ruleCallback, bool allowEmptyId, params string[] buttonIds)
            {
                ButtonIds.AddRange(buttonIds);
                RuleCallback = ruleCallback;
                AllowEmptyId = allowEmptyId;
                AllowEmptyId = allowEmptyId;
            }
            public EnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, bool allowEmptyId, string buttonId)
            {
                ButtonIds.Add(buttonId);
                OrRuleGroupName = orRuleGroupName;
                RuleCallback = ruleCallback;
                AllowEmptyId = allowEmptyId;
            }
            public EnableRule(string orRuleGroupName, Func<Guid, bool> ruleCallback, bool allowEmptyId, params string[] buttonIds)
            {
                ButtonIds.AddRange(buttonIds);
                OrRuleGroupName = orRuleGroupName;
                RuleCallback = ruleCallback;
                AllowEmptyId = allowEmptyId;
            }
        }
    }
}