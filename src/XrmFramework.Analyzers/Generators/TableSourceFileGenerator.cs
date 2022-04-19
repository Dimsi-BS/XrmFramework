using Microsoft.CodeAnalysis;

namespace XrmFramework.Analyzers.Generators
{
	[Generator]
	public class TableSourceFileGenerator : IIncrementalGenerator
	{
		/// <inheritdoc />
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var tableFiles =
				context.AdditionalTextsProvider
					.Where(a => a.Path.EndsWith(".table"));

			// read their contents and save their name
			var namesAndContents =
				tableFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

			context.RegisterSourceOutput(namesAndContents, (productionContext, tuple) =>
			{
				//var tableDefinition = JsonConvert.Dese


				productionContext.AddSource($"{tuple.name}.table.cs", @$"
namespace Titi
{{ 
	public static class {tuple.name}Definition
	{{ 
		public const string EntityName = ""{tuple.name}""; 

		public static class Columns
		{{ 
			/// <summary>
			/// 
			/// Type : Uniqueidentifier
			/// Validity :  Read | Create | AdvancedFind 
			/// </summary> Commentaire à afficher
			public const string Name = ""name""; 
		}}  
	}} 
}}");
			});
		}
	}
}
