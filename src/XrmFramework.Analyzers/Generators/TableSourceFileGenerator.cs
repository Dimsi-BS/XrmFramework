using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                tableFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString())).Collect();

            context.RegisterSourceOutput(namesAndContents, (productionContext, tablesValues) =>
            {
                
                var CoreProjectName = "FrameworkTests";

                TableCollection tables = new TableCollection();
                List<OptionSetEnum> globalEnums = new List<OptionSetEnum>();
                Dictionary<string, string> tablePaths = new Dictionary<string, string>();
                string optionSetPath = "";
                FileInfo fileInfo;
                String text;
                Table currentTable;

                productionContext.AddSource("myTest.cs", "//joefzfozifeo");
                foreach (var tuple in tablesValues)
                {
                    if (!tuple.name.Contains("OptionSet"))
                    {
                        JObject jTable = JObject.Parse(tuple.content);


                        var table = jTable.ToObject<Table>();
                        //var testTable = JsonConvert.DeserializeObject<Table>(tuple.content);
                        /*currentTable = JsonConvert.DeserializeObject<Table>(text,new JsonSerializerSettings
						{
							DefaultValueHandling = DefaultValueHandling.Ignore
						});*/

                        if (jTable["Cols"].Any())
                        {
                            Column currentColumn;
                            foreach (var jColumn in jTable["Cols"])
                            {
                                currentColumn = jColumn.ToObject<Column>();
                                table.Columns.Add(currentColumn);
                            }
                        }
                        tables.Add(table);
                    }
                    else
                    {
                        globalEnums = JsonConvert.DeserializeObject<List<OptionSetEnum>>(tuple.content, new JsonSerializerSettings
                        {
                            DefaultValueHandling = DefaultValueHandling.Ignore
                        });

                    }


                }

                foreach (var table in tables)
                {
                    var sb = new IndentedStringBuilder();

                    sb.AppendLine("");
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.CodeDom.Compiler;");
                    sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                    sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                    sb.AppendLine("using System.ComponentModel;");
                    sb.AppendLine("using XrmFramework;");
                    sb.AppendLine();
                    sb.AppendLine($"namespace {CoreProjectName}");
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

                            // foreach (var t in item.AdditionalInfoCollection.Definitions)
                            // {
                            //     sb.AppendLine();
                            //     sb.AppendLine(
                            //         $"public const {t.Type} {t.Name} = {(t.Type == "String" ? "\"" + (string)t.Value + "\"" : t.Value)};");
                            // }

                            sb.AppendLine();
                            sb.AppendLine("[SuppressMessage(\"Microsoft.Design\", \"CA1034:NestedTypesShouldNotBeVisible\")]");
                            sb.AppendLine("public static class Columns");
                            sb.AppendLine("{");

                            using (sb.Indent())
                            {

                                foreach (var col in table.Columns)
                                {

                                    AddColumnSummary(sb, col);


                                    sb.AppendLine($"[AttributeMetadata(AttributeTypeCode.{col.Type.ToString()})]");
                                    if (col.Type == AttributeTypeCode.Lookup)
                                    {
                                        var relation = table.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == col.LogicalName);
                                        if (relation != null)
                                        {
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);
                                            //var eC = this._entityCollection[relationship.ReferencedEntity];
                                            var rcol = tb?.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id);

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
                                                    sb.Append($"{relation.LookupFieldName},");
                                                }

                                            }
                                            else
                                            {
                                                sb.Append($"[CrmLookup(\"{relation.EntityName}\",");
                                                sb.Append($"\"{relation.LookupFieldName}\",");

                                            }
                                            sb.AppendLine($"RelationshipName = ManyToOneRelationships.{relation.Name})]");


                                        }
                                        else
                                        {

                                        }




                                    }







                                    if (col.PrimaryType == PrimaryType.Id)
                                    {
                                        sb.AppendLine("[PrimaryAttribute(PrimaryAttributeType.Id)]");
                                    }
                                    if (col.EnumName != null && col.EnumName != "")
                                    {
                                        foreach (var e in table.Enums)
                                        {
                                            if (e.LogicalName == col.EnumName)
                                            {

                                                sb.AppendLine($"[OptionSet(typeof({e.Name}))]");
                                                break;
                                            }
                                        }


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
                                                // Write a corresonding line
                                                sb.AppendLine($"[AlternateKey(AlternateKeyNames.{key.Name})]");
                                            }
                                        }
                                    }


                                    if (col.DateTimeBehavior != null)
                                    {
                                        var behavior = string.Empty;
                                        if (col.DateTimeBehavior == DateTimeBehavior.DateOnly)
                                        {
                                            behavior = "DateOnly";
                                        }
                                        else if (col.DateTimeBehavior ==
                                                 DateTimeBehavior.TimeZoneIndependent)
                                        {
                                            behavior = "TimeZoneIndependent";
                                        }
                                        else if (col.DateTimeBehavior ==
                                                 DateTimeBehavior.UserLocal)
                                        {
                                            behavior = "UserLocal";
                                        }

                                        sb.AppendLine($"[DateTimeBehavior(DateTimeBehavior.{behavior})]");
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

                            if (table.ManyToOneRelationships.Any())
                            {
                                sb.AppendLine("public static class ManyToOneRelationships");
                                sb.AppendLine("{");
                                using (sb.Indent())
                                {
                                    foreach (var relationship in table.ManyToOneRelationships)
                                    {
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

                                        if (relationship.Role == EntityRole.Referencing)
                                        {
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);


                                            //var re = tb?.OneToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);
                                            var rc = table.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);

                                            if (rc != null)
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
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                            var rc = table.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);

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


                                }
                                sb.AppendLine("}");
                            }


                            if (table.ManyToManyRelationships.Any())
                            {
                                sb.AppendLine("public static class ManyToManyRelationships");
                                sb.AppendLine("{");
                                using (sb.Indent())
                                {
                                    foreach (var relationship in table.ManyToManyRelationships)
                                    {
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

                                        if (relationship.Role == EntityRole.Referencing)
                                        {
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                            var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                            //var re = tb?.ManyToManyRelationships.FirstOrDefault(r => r.Name == relationship.Name);


                                            if (rc != null)
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

                                            var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                            if (rc != null)
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
                                sb.AppendLine("}");
                            }
                            if (table.OneToManyRelationships.Any())
                            {
                                sb.AppendLine("public static class OneToManyRelationships");
                                sb.AppendLine("{");
                                using (sb.Indent())
                                {
                                    foreach (var relationship in table.OneToManyRelationships)
                                    {
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

                                        if (relationship.Role == EntityRole.Referencing)
                                        {
                                            var tb = tables.FirstOrDefault(t => t.LogicalName == relationship.EntityName);
                                            var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                            //var re = tb?.ManyToOneRelationships.FirstOrDefault(r => r.Name == relationship.Name);


                                            if (rc != null)
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

                                            //var re = tb?.ManyToOneRelationships.FirstOrDefault(r => r.Name == relationship.Name);
                                            var rc = tb?.Columns.FirstOrDefault(c => c.LogicalName == relationship.LookupFieldName);


                                            if (rc != null)
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
                                sb.AppendLine("}");
                            }

                            if (table.Enums.Any())
                            {
                                foreach (var ose in table.Enums)
                                {
                                    sb.AppendLine();
                                    if (ose.IsGlobal)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        var referencedColumns = table.Columns.Where(c => c.EnumName == ose.LogicalName);
                                        //var attribute = def.ReferencedBy.First();

                                        if (!referencedColumns.Any())
                                        {
                                            continue;
                                        }
                                        foreach (var col in referencedColumns)
                                        {
                                            sb.AppendLine(string.Format("[OptionSetDefinition({0}Definition.EntityName, {0}Definition.Columns.{1})]",
                                            table.Name, col.Name));
                                        }

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
                    }

                    sb.AppendLine("}");

                    //On crée le chemin pour cette table

                    productionContext.AddSource($"{table.Name}.table.cs", sb.ToString());







                }

                if (globalEnums.Any())
                {
                    var fc = new IndentedStringBuilder();
                    fc.AppendLine("using System.ComponentModel;");
                    fc.AppendLine("using XrmFramework;");
                    fc.AppendLine();
                    fc.AppendLine($"namespace {CoreProjectName}");
                    fc.AppendLine("{");

                    using (fc.Indent())
                    {
                        foreach (var ose in globalEnums)
                        {
                            fc.AppendLine();
                            if (ose.IsGlobal)
                            {
                                fc.AppendLine($"[OptionSetDefinition(\"{ose.LogicalName}\")]");
                            }
                            else
                            {
                                continue;
                            }

                            fc.AppendLine($"public enum {ose.Name}");
                            fc.AppendLine("{");

                            using (fc.Indent())
                            {

                                if (ose.HasNullValue)
                                {
                                    fc.AppendLine("Null = 0,");
                                }

                                foreach (var val in ose.Values)
                                {
                                    fc.AppendLine($"[Description(\"{val.Name}\")]");

                                    if (!string.IsNullOrEmpty(val.ExternalValue))
                                    {
                                        fc.AppendLine($"[ExternalValue(\"{val.ExternalValue}\")]");
                                    }

                                    fc.AppendLine($"{val.Name} = {val.Value},");
                                }
                            }

                            fc.AppendLine("}");
                        }
                    }
                    fc.AppendLine("}");
                    productionContext.AddSource($"OptionSetDefinition.table.cs", fc.ToString());
                }



                /*productionContext.AddSource($"lolololoo.table.cs", @$"
namespace Titi
{{ 
	public static class lolooloDefinition
	{{ 
		public const string EntityName = ""lolololoo""; 

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
}}");*/



            });
        }



        private void AddColumnSummary(IndentedStringBuilder? sb, Column col)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// ");
            sb.AppendLine($"/// Type : {(AttributeTypeCode)col.Type}{(col.EnumName == null ? "" : " (" + col.EnumName + ")")}");
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
                if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                sb.Append("AdvancedFind ");
            }
            sb.AppendLine();

            sb.AppendLine("/// </summary>");
        }
    }
}
