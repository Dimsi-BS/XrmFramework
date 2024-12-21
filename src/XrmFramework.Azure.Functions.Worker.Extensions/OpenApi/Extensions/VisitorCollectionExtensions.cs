using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Visitors;

namespace XrmFramework.Azure.Functions.Worker.Extensions.OpenApi.Extensions;

public static class VisitorCollectionExtensions
{
    public static VisitorCollection SwitchTypeVisitor<TOldTypeValidator, TNewTypeValidator>(
        this VisitorCollection visitors
        )
        where TNewTypeValidator : class, TOldTypeValidator
        where TOldTypeValidator : IVisitor
    {

        visitors.Visitors.RemoveAll(v => v is TOldTypeValidator and not TNewTypeValidator);

        if (visitors.Visitors.All(visitor => visitor is not TNewTypeValidator))
        {
            var newVisitor = Activator.CreateInstance(typeof(TNewTypeValidator), visitors) as TNewTypeValidator;

            visitors.Visitors.Add(newVisitor);
        }

        return visitors;
    }
}
