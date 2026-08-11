using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators;

[Generator]
public class TableSourceFileGenerator : IIncrementalGenerator
{
	static TableSourceFileGenerator() => DependencyLoader.EnsureLoaded();

	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var namesAndContents =
			context.AdditionalTextsProvider
			   .Where(a => a.Path.EndsWith(".table"))
			   .Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path),
				                  path: text.Path,
				                  content: text.GetText(cancellationToken)!.ToString()))
			   .Collect();

		var compilationAndTables = context.CompilationProvider.Combine(namesAndContents);

		context.RegisterSourceOutput(compilationAndTables, (productionContext, compilationTables) =>
		{
			var (_, tablesValues) = compilationTables;

			var tables = new TableCollection();
			var declarations = new List<(string Path, Table Table)>();

			try
			{
				foreach (var tuple in tablesValues)
				{
					try
					{
						var table = JsonConvert.DeserializeObject<Table>(tuple.content);

						declarations.Add((tuple.path, table));
						tables.Add(table);
					}
					catch (Exception e)
					{

					}
				}

				ReportNameConflicts(productionContext, declarations);

				WriteTables(productionContext, tables);
			}
			catch (Exception e)
			{
				productionContext.AddSource("Exception.txt", $"/*\r\n{e}\r\n*/");
			}
		});
	}

	/// <summary>
	/// Reports the tables several <c>.table</c> files declare under different names.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A table legitimately comes in two copies, one shipped by the framework package and one the
	/// project keeps to enrich it, and <see cref="Table.MergeTo" /> folds them on the CRM logical
	/// name. The C# name is the one thing that fold cannot reconcile: each distinct
	/// <c>Name</c> makes this generator emit its own definition class, so the project ends up with
	/// two classes for one table — <c>OptionSetDefinition</c> and <c>OptionSetsDefinition</c>, say —
	/// each holding the half of the columns and option sets its own copy declared.
	/// </para>
	/// <para>
	/// The two copies may perfectly well live in files named differently: the package names its own
	/// and no project can rename them. Only the <c>Name</c> they carry has to agree.
	/// </para>
	/// </remarks>
	private static void ReportNameConflicts(
		SourceProductionContext productionContext, List<(string Path, Table Table)> declarations)
	{
		var byLogicalName = declarations
			.Where(d => d.Table?.LogicalName != null && d.Table.Name != null)
			.GroupBy(d => d.Table.LogicalName, StringComparer.OrdinalIgnoreCase);

		foreach (var group in byLogicalName)
		{
			// Ordinal: two names differing by case alone are two different C# identifiers, hence two
			// classes, which is precisely the conflict being reported.
			var named = group.GroupBy(d => d.Table.Name, StringComparer.Ordinal)
			                 .OrderBy(g => g.Key, StringComparer.Ordinal)
			                 .ToList();

			if (named.Count < 2)
			{
				continue;
			}

			productionContext.ReportDiagnostic(Diagnostic.Create(
				ConflictingTableNames,
				location: null,
				group.Key,
				string.Join(", ", named.Select(g => $"\"{g.Key}\" ({string.Join(", ", g.Select(d => Path.GetFileName(d.Path)))})"))));
		}
	}

	private static readonly DiagnosticDescriptor ConflictingTableNames = new(
		id: "XRM1001",
		title: "Conflicting names for one table",
		messageFormat: "The table '{0}' is declared with several different \"Name\" values: {1}. "
		             + "Each one makes the generator emit its own definition class, splitting the "
		             + "table's columns between them. Give every .table declaring '{0}' the same "
		             + "\"Name\" — the one the project's code already refers to.",
		category: "XrmFramework.Generators",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: DiagnosticIds.HelpLink("XRM1001"));

	private void WriteTables(SourceProductionContext productionContext, TableCollection tables)
	{
		var alreadyCreatedEnums = new HashSet<string>();
		
		foreach (var table in tables)
		{
			var sb = new IndentedStringBuilder();

			var scopedEnums = table.Enums.Union(tables.SelectMany(t => t.Enums.Where(e => e.IsGlobal))).ToList();
			var keys = SelectKeys(table);

			sb.AppendLine("using System;");
			sb.AppendLine("using System.CodeDom.Compiler;");
			sb.AppendLine("using System.ComponentModel.DataAnnotations;");
			sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
			sb.AppendLine("using System.ComponentModel;");
			sb.AppendLine();
			sb.AppendLine("namespace XrmFramework");
			sb.AppendLine("{");

			using (sb.Indent())
			{
				sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
				sb.AppendLine("[EntityDefinition]");
				sb.AppendLine("[ExcludeFromCodeCoverage]");


				sb.AppendLine($"public static partial class {table.Name}Definition");
				sb.AppendLine("{");

				using (sb.Indent())
				{
					sb.AppendLine($"public const string EntityName = \"{table.LogicalName}\";");
					sb.AppendLine($"public const string EntityCollectionName = \"{table.CollectionName}\";");

					sb.AppendLine();
					sb.AppendLine(
						"[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
					sb.AppendLine("public static class Columns");
					sb.AppendLine("{");

					using (sb.Indent())
					{
						foreach (var col in table.Columns)
						{
							if (!col.Selected) continue;
							var enumDefinition = scopedEnums.FirstOrDefault(e => e.LogicalName == col.EnumName);

							AddColumnSummary(sb, col, enumDefinition);

							sb.AppendLine($"[AttributeMetadata(AttributeTypeCode.{col.Type.ToString()})]");
							if (col.Type == AttributeTypeCode.Lookup)
							{
								var relation =
									table.ManyToOneRelationships.FirstOrDefault(r =>
									                                            r.LookupFieldName == col.LogicalName);
								if (relation != null)
								{
									var tb = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);
									//var eC = this._entityCollection[relationship.ReferencedEntity];
									var rcol = tb?.Columns.FirstOrDefault(colTemp =>
									                                      colTemp.PrimaryType == PrimaryType.Id);

									if (tb != null)
									{
										sb.Append($"[CrmLookup({tb.Name}Definition.EntityName, ");
										if (rcol != null)
											sb.Append($"{tb.Name}Definition.Columns.{rcol.Name}, ");
										else
											throw new Exception(
												"No primaryType was found for the referenced table");
										//sb.Append($"{relation.LookupFieldName},");
									}
									else
									{
										sb.Append($"[CrmLookup(\"{relation.EntityName}\", ");
										sb.Append($"\"{relation.LookupFieldName}\", ");
									}

									sb.AppendLine($"RelationshipName = ManyToOneRelationships.{relation.Name})]");
								}
							}

							if (col.PrimaryType == PrimaryType.Id)
								sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Id)]");

							if (enumDefinition != null)
								sb.AppendLine($"[OptionSet(typeof({enumDefinition.Name}))]");

							if (col.PrimaryType == PrimaryType.Name)
								sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Name)]");

							if (col.PrimaryType == PrimaryType.Image)
								sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Image)]");
							if (col.StringLength.HasValue)
								sb.AppendLine($"[StringLength({col.StringLength.Value})]");

							if (col is { MinRange: not null, MaxRange: not null })
								sb.AppendLine($"[Range({col.MinRange.Value}, {col.MaxRange.Value})]");

							foreach (var key in keys)
								if (key.Covers(col.LogicalName))
									sb.AppendLine($"[AlternateKey(AlternateKeyNames.{key.MemberName})]");

							if (col.Type == AttributeTypeCode.DateTime)
								sb.AppendLine($"[DateTimeBehavior(DateTimeBehavior.{col.DateTimeBehavior.GetValueOrDefault()})]");

							sb.AppendLine($"public const string {col.Name} = \"{col.LogicalName}\";\r\n");
						}
					}

					sb.AppendLine("}");


					if (keys.Count > 0)
					{
						sb.AppendLine(
							"[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
						sb.AppendLine("public static class AlternateKeyNames");
						sb.AppendLine("{");
						using (sb.Indent())
						{
							foreach (var key in keys)
								sb.AppendLine($"public const string {key.MemberName} = \"{key.LogicalName}\";\r\n");
						}

						sb.AppendLine("}");
					}

					AddRelations(sb, tables, table, RelationSelector.ManyToOne(table),
					             "ManyToOneRelationships", lookupCarriedByTable: true);
					AddRelations(sb, tables, table, RelationSelector.ManyToMany(tables, table),
					             "ManyToManyRelationships", lookupCarriedByTable: false);
					AddRelations(sb, tables, table, RelationSelector.OneToMany(tables, table),
					             "OneToManyRelationships", lookupCarriedByTable: false);
				}

				sb.AppendLine("}");

				if (table.Enums.Any())
					foreach (var ose in table.Enums)
					{
						if (!alreadyCreatedEnums.Add(ose.Name))
						{
							continue;
						}

						if (ose.IsGlobal && tables.All(t => t.Columns.All(c =>
							    c.EnumName != ose.LogicalName || (c.EnumName == ose.LogicalName && !c.Selected))))
						{
							continue;
						}

						sb.AppendLine();
						if (ose.IsGlobal)
						{
							sb.AppendLine($"[OptionSetDefinition(\"{ose.LogicalName}\")]");
						}
						else
						{
							var referencedColumn =
								table.Columns.FirstOrDefault(col => col.EnumName == ose.LogicalName);

							if (referencedColumn == null || !referencedColumn.Selected) continue;

							sb.AppendLine(string.Format(
								              "[OptionSetDefinition({0}Definition.EntityName, {0}Definition.Columns.{1})]",
								              table.Name, referencedColumn.Name));
						}

						sb.AppendLine($"public enum {ose.Name}");
						sb.AppendLine("{");

						using (sb.Indent())
						{
							if (ose.HasNullValue) sb.AppendLine("Null = 0,");

							foreach (var val in ose.Values)
							{
								sb.AppendLine($"[Description(\"{val.Name}\")]");

								if (!string.IsNullOrEmpty(val.ExternalValue))
									sb.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");

								sb.AppendLine($"{val.Name} = {val.Value},");
							}
						}

						sb.AppendLine("}");
					}
			}

			sb.AppendLine("}");

			// Build the path for this table

			productionContext.AddSource($"{table.Name}.table.cs", sb.ToString());
		}
	}


	/// <summary>
	/// One alternate key as the generated code expresses it: the constant standing for it inside
	/// <c>AlternateKeyNames</c>, and the columns it rests on.
	/// </summary>
	private sealed class GeneratedKey
	{
		private readonly HashSet<string> _fieldNames;

		public GeneratedKey(string memberName, string logicalName, IEnumerable<string> fieldNames)
		{
			MemberName = memberName;
			LogicalName = logicalName;
			_fieldNames = new HashSet<string>(fieldNames.Where(f => !string.IsNullOrEmpty(f)),
			                                  StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>Name of the constant, which both the class and the <c>[AlternateKey]</c> attributes use.</summary>
		public string MemberName { get; }

		/// <summary>Value of the constant: the name the CRM knows the key under.</summary>
		public string LogicalName { get; }

		/// <summary>Whether the key rests on the given column, and therefore annotates it.</summary>
		public bool Covers(string columnLogicalName)
		=> !string.IsNullOrEmpty(columnLogicalName) && _fieldNames.Contains(columnLogicalName);
	}

	/// <summary>
	/// The alternate keys of <paramref name="table" /> the generated code can stand for.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A key naming itself nowhere designates nothing the CRM can be queried on, and a constant
	/// claimed twice does not compile: both are left out rather than emitted. Deciding this once,
	/// here, is what keeps the <c>[AlternateKey]</c> attributes naming constants the class really
	/// declares — the two used to walk <c>Table.Keys</c> on their own.
	/// </para>
	/// <para>
	/// A file older than <see cref="Key.LogicalName" /> carries that name in <see cref="Key.Name" />
	/// instead, so both fall back on each other: emitting the constant from one and its value from
	/// the other used to give such a key an empty logical name, which the CRM matches nothing on.
	/// </para>
	/// </remarks>
	private static IReadOnlyList<GeneratedKey> SelectKeys(Table table)
	{
		var keys = new List<GeneratedKey>();

		if (table.Keys == null)
		{
			return keys;
		}

		var claimedNames = new HashSet<string>(StringComparer.Ordinal);

		foreach (var key in table.Keys)
		{
			if (string.IsNullOrEmpty(key?.EffectiveLogicalName) || !claimedNames.Add(key.MemberName))
			{
				continue;
			}

			keys.Add(new GeneratedKey(key.MemberName, key.EffectiveLogicalName,
			                          key.FieldNames ?? Enumerable.Empty<string>()));
		}

		return keys;
	}


	private void AddColumnSummary(IndentedStringBuilder sb, Column col, OptionSetEnum? optionSetEnum)
	{
		sb.AppendLine("/// <summary>");
		sb.AppendLine("/// ");
		sb.AppendLine($"/// Type : {col.Type}{(optionSetEnum == null ? "" : " (" + optionSetEnum.Name + ")")}");
		sb.Append("/// Validity :  ");

		var isFirst = true;
		if ((col.Capabilities & AttributeCapabilities.Read) != AttributeCapabilities.None)
		{
			isFirst = false;
			sb.Append("Read ");
		}

		if ((col.Capabilities & AttributeCapabilities.Create) != AttributeCapabilities.None)
		{
			if (isFirst)
				isFirst = false;
			else
				sb.Append("| ");
			sb.Append("Create ");
		}

		if ((col.Capabilities & AttributeCapabilities.Update) != AttributeCapabilities.None)
		{
			if (isFirst)
				isFirst = false;
			else
				sb.Append("| ");
			sb.Append("Update ");
		}

		if ((col.Capabilities & AttributeCapabilities.AdvancedFind) != AttributeCapabilities.None)
		{
			if (!isFirst) sb.Append("| ");

			sb.Append("AdvancedFind ");
		}

		sb.AppendLine();

		sb.AppendLine("/// </summary>");
	}

	/// <summary>
	/// Writes the nested class holding the constants of one family of relationships.
	/// </summary>
	/// <param name="relations">
	/// The relationships <see cref="RelationSelector" /> kept for this family — an empty list writes
	/// nothing at all, not even the class.
	/// </param>
	/// <param name="lookupCarriedByTable">
	/// Whether the lookup column the relationships rest on belongs to <paramref name="table" /> — it
	/// does for N:1 — or to the table at the other end.
	/// </param>
	private void AddRelations(IndentedStringBuilder sb, TableCollection tables, Table table,
	                          IReadOnlyList<Relation> relations, string relationType,
	                          bool lookupCarriedByTable)
	{
		if (relations.Count == 0)
		{
			return;
		}

		sb.AppendLine($"public static class {relationType}");
		sb.AppendLine("{");
		using (sb.Indent())
		{
			foreach (var relationship in relations)
			{
				var targetTable = tables.Get(relationship.EntityName);

				sb.Append("[Relationship(");
				sb.Append(targetTable != null
					          ? $"{targetTable.Name}Definition.EntityName"
					          : $"\"{relationship.EntityName}\"");

				sb.Append($", EntityRole.{relationship.Role}, \"{relationship.NavigationPropertyName}\", ");

				// The lookup is named through the constant of the column bearing it, as long as that
				// column is generated. A N:N rests on an intersect table no project declares: its
				// lookup stays a literal.
				var lookupTable = lookupCarriedByTable ? table : targetTable;
				var lookupColumn = lookupTable?.Columns.FirstOrDefault(
					col => string.Equals(col.LogicalName, relationship.LookupFieldName,
					                     StringComparison.OrdinalIgnoreCase));

				sb.Append(lookupColumn is { Selected: true }
					          ? $"{lookupTable!.Name}Definition.Columns.{lookupColumn.Name}"
					          : $"\"{relationship.LookupFieldName}\"");

				sb.AppendLine(")]");
				sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
			}
		}

		sb.AppendLine("}");
	}
}
