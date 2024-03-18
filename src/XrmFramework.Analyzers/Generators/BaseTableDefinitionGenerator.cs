using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators;

[Generator]
public abstract class BaseTableDefinitionGenerator : IIncrementalGenerator
{
	protected abstract bool GenerateTableFiles { get; }
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		if(!GenerateTableFiles)
		{
			return;
		}
		
		var namesAndContents =
			context.AdditionalTextsProvider
			   .Where(a => a.Path.EndsWith(".table"))
			   .Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path),
				           content: text.GetText(cancellationToken)!.ToString()))
			   .Collect();

		var compilationAndTables = context.CompilationProvider.Combine(namesAndContents);

		context.RegisterSourceOutput(compilationAndTables, (productionContext, compilationTables) =>
		{
			var (_, tablesValues) = compilationTables;

			var tables = new TableCollection();

			try
			{
				foreach (var tuple in tablesValues)
				{
					var table = JsonConvert.DeserializeObject<Table>(tuple.content);

					tables.Add(table);
				}

				WriteTables(productionContext, tables);
			}
			catch (Exception e)
			{
				productionContext.AddSource("Exception.txt", $"/*\r\n{e}\r\n*/");
			}
		});
	}

	private void WriteTables(SourceProductionContext productionContext, TableCollection tables)
	{
		var alreadyCreatedEnums = new HashSet<string>();
		
		foreach (var table in tables)
		{
			WriteTable(productionContext, tables, table, alreadyCreatedEnums);
		}

	}

	protected abstract void WriteTable(SourceProductionContext productionContext, TableCollection tables, Table table, HashSet<string> alreadyCreatedEnums);
}
