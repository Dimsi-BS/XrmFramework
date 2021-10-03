using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace XrmFramework.Analyzers.Utils
{
    public abstract class BindingModelSyntaxGenerator<T> : IBindingModelSyntaxGenerator where T : SubEntitySyntaxGenerator, new()
    {
        public abstract CompilationUnitSyntax AddUsings(CompilationUnitSyntax compilationSyntax);

        public SubEntitySyntaxGenerator GetSyntaxGenerator()
        {
            return new T();
        }
    }

    public abstract class SubEntitySyntaxGenerator
    {
        protected readonly Dictionary<AttributeDefinition, string> AlreadyAddedDefinitions = new Dictionary<AttributeDefinition, string>();
        
        public abstract bool GetSubSyntax(AttributeDefinition attributeDefinition, out StatementSyntax syntax, out string subRecordName);

        public ICollection<StatementSyntax> GetAllSubEntitySyntaxes(ModelDefinition modelDefinition)
        {
            var list = new List<StatementSyntax>();

            foreach (var attribute in modelDefinition.AllAttributes.Where(a => (a.AssociatedDefinition != null && !a.IsModelExtension) || a.ExplicitLinkedAttributeDefinition != null))
            {
                if (GetSubSyntax(attribute.AttributeDefinition, out var syntax, out _))
                {
                    list.Add(syntax.WithAdditionalAnnotations(SyntaxAnnotation.ElasticAnnotation));
                }
            }

            return list;
        }
    }
}
