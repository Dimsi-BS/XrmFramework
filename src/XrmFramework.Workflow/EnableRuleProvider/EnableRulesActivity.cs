using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json.Xrm;
#if INTERNAL_NEWTONSOFT

#else
using Newtonsoft.Json;
#endif

namespace XrmFramework.Workflow.EnableRuleProvider
{
    public abstract class EnableRulesActivityBase : CustomWorkflowActivity
    {
        [Input("EntityName")]
        public InArgument<string> EntityName { get; set; }

        [Input("ObjectId")]
        public InArgument<string> ObjectId { get; set; }

        [Output("Result")]
        public OutArgument<string> Result { get; set; }

        protected EnableRulesActivityBase()
        {
            SetDisplayName("Enable rules activity");

            SetAction(nameof(GetEnableRules));
        }

        public void GetEnableRules(ICustomWorkflowContext context)
        {
            #region Parameters check

            if (context == null) throw new ArgumentNullException(nameof(context));

            #endregion

            var entityName = context.GetArgumentValue(EntityName);
            var objectIdArg = context.GetArgumentValue(ObjectId);

            context.SetArgumentValue(Result, "[]");

            if (objectIdArg == null || !Guid.TryParse(objectIdArg, out var objectId))
            {
                objectId = Guid.Empty;
            }

            var enableRuleProviderTypes = GetType().Assembly.GetExportedTypes().Where(t => typeof(IEnableRuleProvider).IsAssignableFrom(t) && !t.IsAbstract).ToList();

            IEnableRuleProvider ruleProvider = null;

            foreach (var ruleProviderType in enableRuleProviderTypes)
            {
                var constructor = ruleProviderType.GetConstructor(new[] { typeof(ICustomWorkflowContext) });

                if (constructor == null)
                {
                    throw new InvalidPluginExecutionException($"The rule provider {ruleProviderType} must have one, and only one, public constructor.");
                }

                ruleProvider = (IEnableRuleProvider)constructor.Invoke(new object[] { context });

                if (ruleProvider.EntityName == entityName)
                {
                    break;
                }

                ruleProvider = null;
            }

            if (ruleProvider == null)
            {
                return;
            }

            var enableRules = ruleProvider.GetEnableStatus(objectId);

            var resultString = JsonConvert.SerializeObject(enableRules);

            context.SetArgumentValue(Result, resultString);
        }
    }
}
