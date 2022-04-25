using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using XrmFramework.Core;

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
				tableFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()))
					.Collect();

			var compilationAndTables = context.CompilationProvider.Combine(namesAndContents);

			context.RegisterSourceOutput(compilationAndTables, (productionContext, compilationTables) =>
			{
				var tablesValues = compilationTables.Right;

				var coreProjectName = compilationTables.Left.AssemblyName;

				TableCollection tables = new TableCollection();

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
			foreach (var table in tables)
			{
				var sb = new IndentedStringBuilder();

				var scopedEnums = table.Enums.Union(tables.SelectMany(t => t.Enums.Where(e => e.IsGlobal))).ToList();

				sb.AppendLine("using System;");
				sb.AppendLine("using System.CodeDom.Compiler;");
				sb.AppendLine("using System.ComponentModel.DataAnnotations;");
				sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
				sb.AppendLine("using System.ComponentModel;");
				sb.AppendLine("using XrmFramework;");
				sb.AppendLine();
				sb.AppendLine($"namespace XrmFramework.Definitions");
				sb.AppendLine("{");

				using (sb.Indent())
				{
					if (table.Columns.Any())
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
							sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
							sb.AppendLine("public static class Columns");
							sb.AppendLine("{");

							using (sb.Indent())
							{
								foreach (var col in table.Columns)
								{
									if(!col.Selected)
                                    {
										continue;
                                    }
									var enumDefinition = scopedEnums.FirstOrDefault(e => e.LogicalName == col.EnumName);

									AddColumnSummary(sb, col, enumDefinition);

									sb.AppendLine($"[AttributeMetadata(AttributeTypeCode.{col.Type.ToString()})]");
									if (col.Type == AttributeTypeCode.Lookup)
									{
										var relation = table.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == col.LogicalName);
										if (relation != null)
										{
											var tb = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);
											//var eC = this._entityCollection[relationship.ReferencedEntity];
											var rcol = tb?.Columns.FirstOrDefault(colTemp => colTemp.PrimaryType == PrimaryType.Id);

											if (tb != null)
											{
												sb.Append($"[CrmLookup({tb.Name}Definition.EntityName,");
												if (rcol != null)
												{
													sb.Append($"{tb.Name}Definition.Columns.{rcol.Name},");
												}
												else
												{
													throw new Exception("No primaryType was found for the referenced table");
													//sb.Append($"{relation.LookupFieldName},");
												}

											}
											else
											{
												sb.Append($"[CrmLookup(\"{relation.EntityName}\",");
												sb.Append($"\"{relation.LookupFieldName}\",");

											}
											sb.AppendLine($"RelationshipName = ManyToOneRelationships.{relation.Name})]");


										}
									}

									if (col.PrimaryType == PrimaryType.Id)
									{
										sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Id)]");
									}

									if (enumDefinition != null)
									{
										sb.AppendLine($"[OptionSet(typeof({enumDefinition.Name}))]");
									}

									if (col.PrimaryType == PrimaryType.Name)
									{
										sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Name)]");
									}

									if (col.PrimaryType == PrimaryType.Image)
									{
										sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Image)]");
									}
									if (col.StringLength.HasValue)
									{
										sb.AppendLine($"[StringLength({col.StringLength.Value})]");
									}

									if (col.MinRange.HasValue && col.MaxRange.HasValue)
									{
										sb.AppendLine($"[Range({col.MinRange.Value}, {col.MaxRange.Value})]");
									}

									if (table.Keys != null)
									{
										foreach (var key in table.Keys)
										{
											if (key.FieldNames.FirstOrDefault(n => n == col.LogicalName) != null)
											{
												// Write a corresponding line
												sb.AppendLine($"[AlternateKey(AlternateKeyNames.{key.Name})]");
											}
										}
									}

									if (col.DateTimeBehavior != null)
									{
										sb.AppendLine($"[DateTimeBehavior(DateTimeBehavior.{col.DateTimeBehavior})]");
									}

									sb.AppendLine($"public const string {col.Name} = \"{col.LogicalName}\";\r\n");
								}
							}

							sb.AppendLine("}");

							//if(table.Enums.Any(e=>table.Columns.Any())

							if (table.Keys != null && table.Keys.Any())
							{
								sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
								sb.AppendLine("public static class AlternateKeyNames");
								sb.AppendLine("{");
								using (sb.Indent())
								{
									foreach (var key in table.Keys)
									{
										sb.AppendLine($"public const string {key.Name} = \"{key.LogicalName}\";\r\n");
									}
								}
								sb.AppendLine("}");
							}
							
							AddRelations(sb, tables, table, table.ManyToOneRelationships,"ManyToOneRelationships");
							AddRelations(sb, tables, table, table.ManyToManyRelationships, "ManyToManyRelationships");
							AddRelations(sb, tables, table, table.OneToManyRelationships, "OneToManyRelationships");


						}

						sb.AppendLine("}");
					}

					if (table.Enums.Any())
					{
						foreach (var ose in table.Enums)
						{
							if (ose.IsGlobal && tables.All(t => t.Columns.All(c => c.EnumName != ose.LogicalName || (c.EnumName == ose.LogicalName && !c.Selected))))
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

								if (referencedColumn == null || !referencedColumn.Selected)
								{
									continue;
								}

								sb.AppendLine(string.Format(
									"[OptionSetDefinition({0}Definition.EntityName, {0}Definition.Columns.{1})]",
									table.Name, referencedColumn.Name));
							}

							sb.AppendLine($"public enum {ose.Name}");
							sb.AppendLine("{");

							using (sb.Indent())
							{
								if (ose.HasNullValue)
								{
									sb.AppendLine("Null = 0,");
								}

								foreach (var val in ose.Values)
								{
									sb.AppendLine($"[Description(\"{val.Name}\")]");

									if (!string.IsNullOrEmpty(val.ExternalValue))
									{
										sb.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
									}

									sb.AppendLine($"{val.Name} = {val.Value},");
								}
							}

							sb.AppendLine("}");
						}

					}
				}

				sb.AppendLine("}");

				//On crée le chemin pour cette table

				productionContext.AddSource($"{table.Name}.table.cs", sb.ToString());

			}
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
				if (isFirst) { isFirst = false; } else { sb.Append("| "); }
				sb.Append("Create ");
			}

			if ((col.Capabilities & AttributeCapabilities.Update) != AttributeCapabilities.None)
			{
				if (isFirst) { isFirst = false; } else { sb.Append("| "); }
				sb.Append("Update ");
			}

			if ((col.Capabilities & AttributeCapabilities.AdvancedFind) != AttributeCapabilities.None)
			{
				if (!isFirst)
				{
					sb.Append("| ");
				}

				sb.Append("AdvancedFind ");
			}
			sb.AppendLine();

			sb.AppendLine("/// </summary>");
		}

		private void AddRelations(IndentedStringBuilder sb,TableCollection tables,Table table, List<Relation> relations,string relationType)
        {
			if(relations.Any())
            {
				sb.AppendLine($"public static class {relationType}");
				sb.AppendLine("{");
				using (sb.Indent())
				{
					foreach (var relationship in relations)
					{
						if(relationType != "ManyToOneRelationships")
                        {
							if(!tables.Any(t=>t.LogicalName == relationship.EntityName))
                            {
								continue;
                            }
                        }
						sb.Append("[Relationship(");
						var targetTable = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
						if (targetTable != null)
						{
							sb.Append($"{targetTable.Name}Definition.EntityName");
						}
						else
						{
							sb.Append($"\"{relationship.EntityName}\"");
						}

						sb.Append($", EntityRole.{relationship.Role}, \"{relationship.NavigationPropertyName}\", ");


						if(relationType == "ManyToOneRelationships")
                        {
							if (relationship.Role == EntityRole.Referencing)
							{
								//var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);


								//var re = tb?.OneToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);
								var rc = table.Columns.FirstOrDefault(col => col.LogicalName == relationship.LookupFieldName);

								if (rc != null && rc.Selected)
								{
									sb.Append($"{table.Name}Definition.Columns.{rc.Name}");
								}
								else
								{
									sb.Append($"\"{relationship.LookupFieldName}\"");
								}
							}
							else
							{
								var rc = table.Columns.FirstOrDefault(col => col.LogicalName == relationship.LookupFieldName);

								//var r = tb?.OneToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);


								if (rc != null)
								{
									sb.Append($"{table.Name}Definition.Columns.{rc.Name}");
								}
								else
								{
									sb.Append($"\"{relationship.LookupFieldName}\"");
								}
							}

							sb.AppendLine(")]");
							sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
						}
						else
                        {
							if (relationship.Role == EntityRole.Referencing)
							{
								var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
								var rc = tb?.Columns.FirstOrDefault(col => col.LogicalName == relationship.LookupFieldName);

								if ((rc != null && rc.Selected) && tb != null)
								{
									sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
								}
								else
								{
									sb.Append($"\"{relationship.LookupFieldName}\"");
								}
							}
							else
							{
								var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);

								var rc = tb?.Columns.FirstOrDefault(col => col.LogicalName == relationship.LookupFieldName);

								if ((rc != null && rc.Selected) && tb != null)
								{
									sb.Append($"{tb.Name}Definition.Columns.{rc.Name}");
								}
								else
								{
									sb.Append($"\"{relationship.LookupFieldName}\"");
								}
							}

							sb.AppendLine(")]");
							sb.AppendLine($"public const string {relationship.Name} = \"{relationship.Name}\";");
						}

						

						
					}
				}
				sb.AppendLine("}");
			}
        }
	}
}
