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

                                if (correspondingColumn.Selected)
                                {
                                    //This property is a column
                                    sb.Append(
                                        $"[CrmMapping({correspondingTable.Name}Definition.Columns.{correspondingColumn.Name}"); //)]");
                                    if (prop.IsValidForUpdate)
                                    {
                                        sb.Append(")]");

                                    }
                                    else
                                    {
                                        sb.Append(",IsValidForUpdate = false)]");
                                    }

                                    if (correspondingColumn.Type == AttributeTypeCode.Lookup)
                                    {
                                        //Get the corresponding relationship info in the table
                                        var correspondingRelation =
                                            correspondingTable.ManyToOneRelationships.FirstOrDefault(r =>
                                                r.LookupFieldName == prop.LogicalName);
                                        if (correspondingRelation == null)
                                        {
                                            // The mapping cannot name a target the table does not
                                            // declare. Reported and skipped, so the class and its
                                            // mapping agree on which properties exist.
                                            productionContext.ReportDiagnostic(Diagnostic.Create(
                                                Xrm1007, Location.None, model.Name, prop.Name,
                                                $"lookup column '{prop.LogicalName}' has no many-to-one relationship in table '{correspondingTable.LogicalName}'"));
                                            continue;
                                        }

                                        sb.AppendLine();
                                        sb.Append($"[CrmLookup(");
                                        var referencedTable = tables.FirstOrDefault(t =>
                                            t.LogicalName == correspondingRelation.EntityName);
                                        if (referencedTable != null)
                                        {
                                            sb.Append($"{referencedTable.Name}Definition.EntityName,");
                                            var referencedColumn = referencedTable.Columns.FirstOrDefault(c =>
                                                c.LogicalName == correspondingRelation.LookupFieldName);
                                            if (referencedColumn != null)
                                            {
                                                sb.Append(
                                                    $"{referencedTable.Name}Definition.Columns.{referencedColumn.Name})]");
                                            }
                                            else
                                            {
                                                sb.Append($"\"{correspondingRelation.LookupFieldName}\")]");

                                            }
                                        }
                                        else
                                        {
                                            sb.AppendLine(
                                                $"\"{correspondingRelation.EntityName}\",\"{correspondingRelation.LookupFieldName}\")]");
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

                                // Add other possible attributes


                                if (!prop.IsValidForUpdate)
                                {
                                    // Write regular declaration
                                    if (correspondingColumn != null)
                                    {
                                        sb.AppendLine($"public {prop.TypeFullName} {prop.Name} {{get; set;}}");
                                    }
                                    else
                                    {
                                        sb.AppendLine(
                                            $"public List<{prop.TypeFullName}> {prop.Name} {{get;set;}} = new List<{prop.TypeFullName}>();");
                                    }
                                }
                                else
                                {

                                    // Write property declaration with call to OnPropertyChanged()
                                    if (correspondingColumn != null)
                                    {
                                        string tmp = @$"
        public {prop.TypeFullName} {prop.Name}
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
        public List<{prop.TypeFullName}> {prop.Name}
        {{
            get {{return _{prop.Name};}}
            set
            {{
                if(value == _{prop.Name})
                    return;
                _{prop.Name} = value;
                OnPropertyChanged();
            }}
        }}= new List<{prop.TypeFullName}>();
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
                                    sb.AppendLine($"private {prop.TypeFullName} _{prop.Name};");

                                }
                                else
                                {
                                    sb.AppendLine($"private List<{prop.TypeFullName}> _{prop.Name};");
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

        private static DiagnosticDescriptor DescriptorFor(string id) => id switch
        {
            "XRM1007" => Xrm1007,
            _ => Xrm1006,
        };

    }
}
