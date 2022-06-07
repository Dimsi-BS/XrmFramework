using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Generators
{
    [Generator]
    public class ModelSourceFileGenerator : IIncrementalGenerator
    {
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
				Table GlobalEnums = null;
				try
				{
					foreach (var tuple in modelValues)
					{
						if(tuple.name.Contains(".model"))
                        {
							var model = JsonConvert.DeserializeObject<XrmFramework.Core.Model>(tuple.content);
							//model.Name = tuple.name;
							models.Add(model);
						}
						else if(tuple.name.Contains(".table"))
                        {
							if(tuple.name == "OptionSet.table")
                            {
								GlobalEnums = JsonConvert.DeserializeObject<Table>(tuple.content);
                            }
							else
                            {
								var table = JsonConvert.DeserializeObject<Table>(tuple.content);
								tables.Add(table);
                            }
                        }
						
					}

					WriteModelFiles(productionContext, models,tables,GlobalEnums);
				}
				catch (Exception e)
				{
					productionContext.AddSource("Exception.txt", $"/*\r\n{e}\r\n*/");
				}
			});
		}

        private void WriteModelFiles(SourceProductionContext productionContext, List<XrmFramework.Core.Model> models,TableCollection tables, Table globalEnums)
        {
            if(globalEnums == null)
            {
                throw new Exception("global enums is null for some reason");
            }
			foreach(var model in models)
            {
				var table = tables.FirstOrDefault(t => t.LogicalName == model.TableLogicalName);
				if(table == null)
				{
					productionContext.AddSource($"{model.Name}.model.cs", $"// This is an empty test file {model.Name}, there are {globalEnums.Enums.Count} global enums, there are {tables.Count} tables ");

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
                    sb.AppendLine("using XrmFramework.Definitions;");
                    foreach(var otherModel in models)
                    {
                        //if(otherModel.Name != model.Name)
                        //{
                        //    if(!string.IsNullOrEmpty(otherModel.ModelNamespace))
                        //    {
                        //        sb.AppendLine("using " + otherModel.ModelNamespace + ";");
                        //
                        //    }
                        //}
                    }
                    //sb.AppendLine($"using {CoreProjectName};");
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
                        sb.AppendLine($"[CrmEntity({correspondingTable.Name}Definition.EntityName)]");
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
                                if(!correspondingColumn.Selected)
                                {
                                    continue;
                                }
                                {
                                    if (correspondingColumn != null)
                                    {   //This property is a column
                                        sb.Append($"[CrmMapping({correspondingTable.Name}Definition.Columns.{correspondingColumn.Name}");//)]");
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
                                            var correspondingRelation = correspondingTable.ManyToOneRelationships.FirstOrDefault(r => r.LookupFieldName == prop.LogicalName);
                                            if (correspondingRelation == null)
                                            {
                                                throw new Exception("No corresponding relationship found in table for " + prop.Name);
                                            }
                                            else
                                            {
                                                sb.AppendLine();
                                                sb.Append($"[CrmLookup(");
                                                var referencedTable = tables.FirstOrDefault(t => t.LogicalName == correspondingRelation.EntityName);
                                                if (referencedTable != null)
                                                {
                                                    sb.Append($"{referencedTable.Name}Definition.EntityName,");
                                                    var referencedColumn = referencedTable.Columns.FirstOrDefault(c => c.LogicalName == correspondingRelation.LookupFieldName);
                                                    if (referencedColumn != null)
                                                    {
                                                        sb.Append($"{referencedTable.Name}Definition.Columns.{referencedColumn.Name})]");
                                                    }
                                                    else
                                                    {
                                                        sb.Append($"\"{correspondingRelation.LookupFieldName}\")]");

                                                    }
                                                }
                                                else
                                                {
                                                    sb.AppendLine($"\"{correspondingRelation.EntityName}\",\"{correspondingRelation.LookupFieldName}\")]");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //This property is a OneToMany relation
                                        var correspondingRelation = correspondingTable.OneToManyRelationships.FirstOrDefault(r => r.Name == prop.LogicalName);
                                        if (correspondingRelation == null)
                                        {
                                            throw new Exception("Error, no corresponding OneToMany relation found for this property : " + prop.Name);
                                        }
                                        sb.AppendLine($"[ChildRelationship({correspondingTable.Name}Definition.OneToManyRelationships.{correspondingRelation.NavigationPropertyName})]");
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
                                            sb.AppendLine(String.Format("public {0} {1} {{get; set;}}", prop.TypeFullName, prop.Name));
                                        }
                                        else
                                        {
                                            sb.AppendLine($"public List<{prop.TypeFullName}> {prop.Name} {{get;set;}} = new List<{prop.TypeFullName}>();");
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
                            }




                            sb.AppendLine("#region Fields");

                            foreach (var prop in model.Properties.Where(p => p.IsValidForUpdate))
                            {
                                //Add the corresponding field
                                var correspondingColumn = correspondingTable.Columns.FirstOrDefault(c => c.LogicalName == prop.LogicalName);
                                if (correspondingColumn != null)
                                {
                                    sb.AppendLine(String.Format("private {0} _{1};", prop.TypeFullName, prop.Name));

                                }
                                else
                                {
                                    sb.AppendLine(String.Format("private List<{0}> _{1};", prop.TypeFullName, prop.Name));
                                }
                            }
                            sb.AppendLine("#endregion");






                        }
                        sb.AppendLine("}");

                    }

                    sb.AppendLine("}");
                    productionContext.AddSource($"{model.Name}.model.cs", sb.ToString());

					
				}

			}

		}
	}
}
