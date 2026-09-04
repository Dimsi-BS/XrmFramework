using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using XrmFramework.Analyzers.Generators.Mapping;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators
{
    [Generator]
    public class ModelSourceFileGenerator : IIncrementalGenerator
    {
        static ModelSourceFileGenerator() => DependencyLoader.EnsureLoaded();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {

            //return;
            //var tableFiles = context.AdditionalTextsProvider.Where(a => a.Path.EndsWith(".table"));
            var files =
            context.AdditionalTextsProvider
                .Where(a => a.Path.EndsWith(".model") || a.Path.EndsWith(".table"));

            // read their contents and save their name
            var namesAndContents =
                files.Select((text, cancellationToken) => (name: Path.GetFileName(text.Path), content: text.GetText(cancellationToken)!.ToString()))
                    .Collect();

            var compilationAndModels = context.CompilationProvider.Combine(namesAndContents);

            context.RegisterSourceOutput(compilationAndModels, (productionContext, compilationModels) =>
            {
                var modelValues = compilationModels.Right;

                var coreProjectName = compilationModels.Left.AssemblyName;

                List<XrmFramework.Core.Model> models = new List<XrmFramework.Core.Model>();
                TableCollection tables = new TableCollection();
                Table? globalEnums = null;
                try
                {
                    foreach (var tuple in modelValues)
                    {
                        if (tuple.name.Contains(".model"))
                        {
                            var model = JsonConvert.DeserializeObject<XrmFramework.Core.Model>(tuple.content);

                            if (model != null)
                            {
                                models.Add(model);
                            }
                        }
                        else if (tuple.name.Contains(".table"))
                        {
                            if (tuple.name == "OptionSets.table")
                            {
                                globalEnums = JsonConvert.DeserializeObject<Table>(tuple.content);
                            }
                            else
                            {
                                var table = JsonConvert.DeserializeObject<Table>(tuple.content);

                                if (table != null)
                                {
                                    tables.Add(table);
                                }
                            }
                        }

                    }

                    WriteModelFiles(productionContext, models, tables, globalEnums);
                }
                catch (Exception e)
                {
                    // A generator that fails must say so as a diagnostic. Emitting a source
                    // file named Exception.txt hides the failure behind a compilation that
                    // "succeeds" while the models it was meant to produce are simply absent.
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        Xrm1008, Location.None, "one or more .model files", e.Message));
                }
            });
        }

        private void WriteModelFiles(SourceProductionContext productionContext, ICollection<Core.Model> models, TableCollection tables, Table? globalEnums)
        {
            // A project with no global option sets is legitimate: OptionSets.table only
            // exists once something references a global enum.
            globalEnums ??= new Table();
            foreach (var model in models)
            {
                var table = tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
                if (table == null)
                {
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        Xrm1005, Location.None, model.Name, model.TableLogicalName));
                }
                else
                {
                    // Create start of class
                    var sb = new IndentedStringBuilder();
                    var correspondingTable = tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
                    if (correspondingTable == null)
                    {
                        throw new Exception("The table corresponding to this model was not found, its logical name is : " + model.TableLogicalName);
                    }
                    sb.AppendLine("");
                    sb.AppendLine("using System;");
                    sb.AppendLine("using System.CodeDom.Compiler;");
                    sb.AppendLine("using System.ComponentModel.DataAnnotations;");
                    sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
                    sb.AppendLine("using System.Collections.Generic;");
                    sb.AppendLine("using XrmFramework;");
                    sb.AppendLine("using Newtonsoft.Json;");
                    sb.AppendLine("using XrmFramework.BindingModel;");

                    sb.AppendLine();

                    if (model.ModelNamespace != null && model.ModelNamespace != "")
                    {
                        sb.AppendLine($"namespace {model.ModelNamespace}");

                    }
                    else
                    {
                        sb.AppendLine($"namespace ProjectModels");

                    }
                    sb.AppendLine("{");

                    using (sb.Indent())
                    {
                        // Class declaration
                        sb.AppendLine("[GeneratedCode(\"XrmFramework\", \"2.0\")]");
                        sb.AppendLine("[ExcludeFromCodeCoverage]");
                        sb.AppendLine($"[CrmEntity(typeof({correspondingTable.Name}Definition))]");
                        sb.AppendLine("[JsonObject(MemberSerialization.OptIn)]");




                        sb.AppendLine($"public partial class {model.Name} : BindingModelBase");


                        sb.AppendLine("{");
                        // Properties
                        using (sb.Indent())
                        {
                            //sb.AppendLine();
                            //sb.AppendLine($"[CrmMapping({correspondingTable.Name}Definition.Columns.Id)]");
                            //sb.AppendLine("public Guid Id { get; set; }");
                            sb.AppendLine();
                            foreach (var prop in model.Properties)
                            {

                                var correspondingColumn = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == prop.LogicalName);
                                if (correspondingColumn == null)
                                {
                                    continue;
                                }

                                var propertyType = PropertyType(prop);

                                if (correspondingColumn.Selected)
                                {
                                    //This property is a column
                                    sb.Append(
                                        $"[CrmMapping({correspondingTable.Name}Definition.Columns.{correspondingColumn.Name}");

                                    if (!prop.IsValidForUpdate)
                                    {
                                        sb.Append(", IsValidForUpdate = false");
                                    }

                                    // Below the first level the query builder stops unless the
                                    // property asks for the link, which is what keeps a model from
                                    // dragging in the whole graph.
                                    if (prop.FollowLink)
                                    {
                                        sb.Append(", FollowLink = true");
                                    }

                                    sb.Append(")]");

                                    if (IsLookup(correspondingColumn.Type))
                                    {
                                        if (!WriteLookupAttribute(productionContext, sb, model, prop,
                                                correspondingTable, correspondingColumn, tables))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    //This property is a OneToMany relation
                                    var correspondingRelation =
                                        correspondingTable.OneToManyRelationships.FirstOrDefault(r =>
                                            r.Name == prop.LogicalName);
                                    if (correspondingRelation == null)
                                    {
                                        productionContext.ReportDiagnostic(Diagnostic.Create(
                                            Xrm1006, Location.None, model.Name, prop.Name,
                                            $"no one-to-many relationship named '{prop.LogicalName}' in table '{correspondingTable.LogicalName}'"));
                                        continue;
                                    }

                                    sb.AppendLine(
                                        $"[ChildRelationship({correspondingTable.Name}Definition.OneToManyRelationships.{correspondingRelation.NavigationPropertyName})]");
                                }

                                if (prop.JsonPropertyName != null)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine($"[JsonProperty(\"{prop.JsonPropertyName}\")]");
                                }

                                if (prop.JsonIgnore)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine("[JsonIgnore]");
                                }


                                if (!prop.IsValidForUpdate)
                                {
                                    // Write regular declaration
                                    if (correspondingColumn != null)
                                    {
                                        sb.AppendLine($"public {propertyType} {prop.Name} {{get; set;}}");
                                    }
                                    else
                                    {
                                        sb.AppendLine(
                                            $"public List<{propertyType}> {prop.Name} {{get;set;}} = new List<{propertyType}>();");
                                    }
                                }
                                else
                                {

                                    // Write property declaration with call to OnPropertyChanged()
                                    if (correspondingColumn != null)
                                    {
                                        string tmp = @$"
        public {propertyType} {prop.Name}
        {{
            get {{return _{prop.Name};}}
            set
            {{
                if(value == _{prop.Name})
                    return;
                _{prop.Name} = value;
                OnPropertyChanged();
            }}
        }}
                                                      ";
                                        //Console.WriteLine(tmp);
                                        sb.AppendLine(tmp);
                                    }
                                    else
                                    {
                                        string tmp2 = @$"
        public List<{propertyType}> {prop.Name}
        {{
            get {{return _{prop.Name};}}
            set
            {{
                if(value == _{prop.Name})
                    return;
                _{prop.Name} = value;
                OnPropertyChanged();
            }}
        }}= new List<{propertyType}>();
                                                      ";
                                        sb.AppendLine(tmp2);
                                        // "{" +
                                        // "   get { return _{1};}\n" +
                                        // "   set\n" +
                                        // "       {\n" +
                                        // "           if(value == _{1})\n" +
                                        // "           {\n" +
                                        // "               return;\n" +
                                        // "           }\n" +
                                        // "           _{1} = value;\n" +
                                        // "           OnPropertyChanged();\n" +
                                        // "       }\n" +
                                        // "} = new List<{0}>();\n", prop.TypeFullName, prop.Name));
                                    }

                                }

                                sb.AppendLine();
                            }




                            sb.AppendLine("#region Fields");

                            foreach (var prop in model.Properties.Where(p => p.IsValidForUpdate))
                            {
                                //Add the corresponding field
                                var correspondingColumn = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == prop.LogicalName);
                                if (correspondingColumn != null)
                                {
                                    sb.AppendLine($"private {PropertyType(prop)} _{prop.Name};");

                                }
                                else
                                {
                                    sb.AppendLine($"private List<{PropertyType(prop)}> _{prop.Name};");
                                }
                            }
                            sb.AppendLine("#endregion");

                            // The mapping goes in the same class, emitted by the same writer that
                            // serves hand-written models. It cannot be left to MappingSourceGenerator:
                            // that one discovers its candidates among the compilation's source syntax
                            // trees, which never contain another generator's output.
                            WriteMapping(productionContext, sb, model, correspondingTable, tables);
                        }
                        sb.AppendLine("}");

                    }

                    sb.AppendLine("}");
                    productionContext.AddSource($"{model.Name}.model.cs", sb.ToString());


                }

            }

        }

        /// <summary>
        ///     The type the generated property carries. A related model replaces the
        ///     declared type: the property holds that model, filled from the record behind
        ///     the lookup rather than the lookup value itself.
        /// </summary>
        private static string PropertyType(ModelProperty prop)
            => string.IsNullOrEmpty(prop.LookupTargetModel) ? prop.TypeFullName : prop.LookupTargetModel;

        private static bool IsLookup(AttributeTypeCode type)
            => type == AttributeTypeCode.Lookup
            || type == AttributeTypeCode.Customer
            || type == AttributeTypeCode.Owner;

        /// <summary>
        ///     Writes the <c>[CrmLookup]</c> that tells the reflection layer what to read through
        ///     a lookup, and returns whether the property can be emitted at all.
        /// </summary>
        /// <remarks>
        ///     A plain lookup reaching one table needs no attribute — the relationship is
        ///     unambiguous and the Guid is read straight from the EntityReference. An attribute is
        ///     written when the model asks for something more: a column of the targeted record to
        ///     project, or a target to pick among several. A related model needs none either: the
        ///     query builder reads the target off that model's own [CrmEntity].
        /// </remarks>
        private static bool WriteLookupAttribute(
            SourceProductionContext productionContext,
            IndentedStringBuilder sb,
            Core.Model model,
            ModelProperty prop,
            Table table,
            Column column,
            TableCollection tables)
        {
            var relations = table.ManyToOneRelationships
                .Where(r => r.LookupFieldName == column.LogicalName)
                .ToList();

            if (relations.Count == 0)
            {
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    Xrm1007, Location.None, model.Name, prop.Name,
                    $"lookup column '{column.LogicalName}' has no many-to-one relationship in table '{table.LogicalName}'"));
                return false;
            }

            Relation relation;

            if (!string.IsNullOrEmpty(prop.LookupTargetTableLogicalName))
            {
                relation = relations.FirstOrDefault(r => r.EntityName == prop.LookupTargetTableLogicalName);

                if (relation == null)
                {
                    productionContext.ReportDiagnostic(Diagnostic.Create(
                        Xrm1010, Location.None, model.Name, prop.Name,
                        $"LookupTargetTableLogicalName is '{prop.LookupTargetTableLogicalName}', which lookup column "
                        + $"'{column.LogicalName}' does not reach (it reaches {string.Join(", ", relations.Select(r => r.EntityName))})"));
                    return false;
                }
            }
            else if (relations.Count > 1)
            {
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    Xrm1010, Location.None, model.Name, prop.Name,
                    $"lookup column '{column.LogicalName}' reaches several tables "
                    + $"({string.Join(", ", relations.Select(r => r.EntityName))}); set LookupTargetTableLogicalName to say which one this property maps"));
                return false;
            }
            else
            {
                relation = relations[0];
            }

            // A related model carries its own [CrmEntity]; nothing more to declare here.
            if (!string.IsNullOrEmpty(prop.LookupTargetModel))
            {
                return true;
            }

            var noProjection = string.IsNullOrEmpty(prop.LookupTargetColumnLogicalName);

            // Nothing to disambiguate and nothing to project: the plain Guid needs no attribute.
            if (noProjection && relations.Count == 1)
            {
                return true;
            }

            var target = tables.FirstOrDefault(t => t.LogicalName == relation.EntityName);

            var entityRef = target != null
                ? $"{target.Name}Definition.EntityName"
                : $"\"{relation.EntityName}\"";

            // Without a projection the property is the target's own id — the idiom the
            // hand-written models use to name a polymorphic target.
            var projected = noProjection
                ? target?.Columns.FirstOrDefault(c => c.PrimaryType == PrimaryType.Id)
                : target?.Columns.FirstOrDefault(c => c.LogicalName == prop.LookupTargetColumnLogicalName);

            string columnRef;

            if (projected != null)
            {
                columnRef = $"{target.Name}Definition.Columns.{projected.Name}";
            }
            else if (!noProjection)
            {
                columnRef = $"\"{prop.LookupTargetColumnLogicalName}\"";
            }
            else
            {
                // Target table not tracked and no projection asked for: nothing reliable to name.
                return true;
            }

            sb.AppendLine();
            sb.Append($"[CrmLookup({entityRef}, {columnRef}");
            if (prop.AllowNotExisting)
            {
                sb.Append(", true");
            }
            sb.Append(")]");

            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Mapping
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Appends <c>ToBindingModel</c> / <c>ToEntity</c> to the class body being written, from
        /// the description <see cref="MappingModelFactory"/> derives from the pair of files.
        /// Anything the pair does not allow to be mapped is reported as a diagnostic instead of
        /// dropping the property from the generated class without a word.
        /// </summary>
        private static void WriteMapping(
            SourceProductionContext productionContext,
            IndentedStringBuilder sb,
            Core.Model model,
            Table table,
            TableCollection tables)
        {
            var result = MappingModelFactory.Create(model, table, tables);

            foreach (var failure in result.Failures)
            {
                productionContext.ReportDiagnostic(Diagnostic.Create(
                    DescriptorFor(failure.Id),
                    Location.None,
                    failure.ModelName,
                    failure.PropertyName,
                    failure.Detail));
            }

            if (result.Model == null)
            {
                return;
            }

            var writer = new CodeWriter();
            MappingEmitter.WriteMethods(writer, result.Model);

            sb.AppendLine();
            sb.AppendLines(writer.ToString(), skipFinalNewline: true);
        }

        private static readonly DiagnosticDescriptor Xrm1005 = new(
            "XRM1005",
            "Model references an unknown table",
            "Model '{0}' targets table '{1}', which no .table file declares",
            "XrmFramework.Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1005"));

        private static readonly DiagnosticDescriptor Xrm1006 = new(
            "XRM1006",
            "Model property cannot be mapped to a column",
            "Model '{0}': property '{1}' cannot be mapped — {2}",
            "XrmFramework.Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1006"));

        private static readonly DiagnosticDescriptor Xrm1007 = new(
            "XRM1007",
            "Lookup property without a relationship",
            "Model '{0}': property '{1}' cannot be mapped — {2}",
            "XrmFramework.Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1007"));

        private static readonly DiagnosticDescriptor Xrm1008 = new(
            "XRM1008",
            "Malformed .model file",
            "'{0}' could not be read as a model: {1}",
            "XrmFramework.Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1008"));

        private static readonly DiagnosticDescriptor Xrm1009 = new(
            "XRM1009",
            "Model property type does not match its column",
            "Model '{0}': property '{1}' {2}",
            "XrmFramework.Generators",
            // A warning, not an error: the accepted sets cover what the emitter special-cases,
            // but a project may have a legitimate mapping this does not know about, and a false
            // positive must not stop a build.
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1009"));

        private static readonly DiagnosticDescriptor Xrm1010 = new(
            "XRM1010",
            "Ambiguous lookup target",
            "Model '{0}': property '{1}' {2}",
            "XrmFramework.Generators",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            helpLinkUri: DiagnosticIds.HelpLink("XRM1010"));

        private static DiagnosticDescriptor DescriptorFor(string id) => id switch
        {
            "XRM1007" => Xrm1007,
            "XRM1009" => Xrm1009,
            "XRM1010" => Xrm1010,
            _ => Xrm1006,
        };

    }
}
