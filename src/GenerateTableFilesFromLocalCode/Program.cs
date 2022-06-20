using Microsoft.Xrm.Sdk;
using System;
using XrmFramework.Core;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GenerateTableFilesFromLocalCode
{
    internal class Program
    {
        static TableCollection tables = new TableCollection();
        EntityCollection entities;
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine();
                Console.WriteLine("Number of argument too low, the command must be entered the following way : ");
                Console.WriteLine();
                Console.WriteLine("GenerateTableFilesFromLocalCode \"path\\to\\dll\" \"path\\to\\.tableDirectory\"");
                Console.WriteLine();
                Console.ReadKey();

                return;
            }
            string userInput = "";
            Assembly assembly = null;
            string definitionDirectory;
            try
            {
                assembly = Assembly.UnsafeLoadFrom(args[0]);
            }
            catch
            {
                Console.WriteLine();
                Console.WriteLine($"Error argument \"{args[0]}\" is invalid, no assembly could be loaded from it, reminder : it should be a path to a .dll file.");
                Console.WriteLine();
                Console.ReadKey();
                return;
            }

            if(!Directory.Exists(args[1]))
            {
                Console.WriteLine();
                Console.WriteLine($"Error argument \"{args[1]}\" is invalid, reminder : it should be the path to the directory in which you want to save your .table files");
                Console.WriteLine();
                Console.ReadKey();

                return;

            }
            

            definitionDirectory = args[1];
            var codedTables = GetCodedTables(assembly);


            //tables.AddRange(GetCodedTables(assembly));
            //GetCoded entityDefinitions
            string txt;
            foreach (var table in codedTables)
            {
                txt = JsonConvert.SerializeObject(table, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                });

                var fileInfo = new FileInfo($"{definitionDirectory}/{table.Name}.table");
                File.WriteAllText(fileInfo.FullName,txt);
                Console.WriteLine(table.Name);
            }

            
        }

        public static IEnumerable<Table> GetCodedTables(Assembly definitionAssembly)
        {
            var entityDefinitionAttributeType = definitionAssembly.GetType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = definitionAssembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            var relationshipAttributeType = definitionAssembly.GetType("XrmFramework.RelationshipAttribute");
            var definitionManagerIgnoreAttributeType = definitionAssembly.GetType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute");
            if(definitionManagerIgnoreAttributeType == null)
            {
                definitionManagerIgnoreAttributeType = definitionAssembly.GetType("Model.DefinitionManagerIgnoreAttribute");

            }

            var tableList = new List<Table>();
            Console.WriteLine($"F{definitionTypes.Count()} definitions were found.");

            foreach (var t in definitionTypes)
            {
                Console.WriteLine($"Adding {t.Name} to tables.");
                if (t.GetCustomAttributes(definitionManagerIgnoreAttributeType).Any())
                {
                    continue;
                }

                var table = new Table
                {
                    Name = t.Name.Replace("Definition","")
                    ,
                    LogicalName = t.GetField("EntityName").GetValue(null) as string
                    ,
                    CollectionName = t.GetField("EntityCollectionName")?.GetValue(null) as string
                    ,
                    Selected = true
                };



                foreach (var field in t.GetNestedType("Columns").GetFields())
                {
                    table.Columns.Add(new Column
                    {
                        LogicalName = field.GetValue(null) as string
                        ,
                        Name = field.Name
                        ,
                        Selected = true
                        
                    });



                }

                foreach (var field in t.GetFields())
                {
                    if (field.Name == "EntityName" || field.Name == "EntityCollectionName")
                    {
                        continue;
                    }

                    var typeName = field.FieldType.Name;

                    //definition.AdditionalInfoCollection.Add(new AttributeDefinition
                    //{
                    //    Type = typeName
                    //    ,
                    //    Name = field.Name
                    //    ,
                    //    LogicalName = field.Name
                    //    ,
                    //    Value = field.GetValue(null).ToString()
                    //    ,
                    //    IsSelected = true
                    //});
                }

                foreach (var nestedType in t.GetNestedTypes())
                {
                    if (nestedType.Name == "Columns")
                    {
                        continue;
                    }

                    //var classDefinition = new ClassDefinition
                    //{
                    //    LogicalName = nestedType.Name
                    //    ,
                    //    Name = nestedType.Name
                    //    ,
                    //    IsEnum = nestedType.IsEnum
                    //};

                    //if (nestedType.IsEnum)
                    //{
                    //    var names = Enum.GetNames(nestedType);
                    //    var values = Enum.GetValues(nestedType);
                    //
                    //    for (var i = 0; i < names.Length; i++)
                    //    {
                    //        classDefinition.Attributes.Add(new AttributeDefinition
                    //        {
                    //            LogicalName = Name = names[i]
                    //            ,
                    //            Name = names[i]
                    //            ,
                    //            Value = (int)values.GetValue(i)
                    //            ,
                    //            IsSelected = true
                    //        });
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var field in nestedType.GetFields())
                    //    {
                    //
                    //        if (nestedType.Name == "ManyToOneRelationships" || nestedType.Name == "OneToManyRelationships" || nestedType.Name == "ManyToManyRelationships")
                    //        {
                    //            dynamic relationshipAttribute = field.GetCustomAttribute(relationshipAttributeType);
                    //
                    //            classDefinition.Attributes.Add(new RelationshipAttributeDefinition
                    //            {
                    //                LogicalName = field.GetValue(null).ToString()
                    //                ,
                    //                Name = field.Name
                    //                ,
                    //                Type = field.FieldType.Name
                    //                ,
                    //                Value = field.GetValue(null).ToString()
                    //                ,
                    //                IsSelected = true
                    //                ,
                    //                NavigationPropertyName = relationshipAttribute?.NavigationPropertyName
                    //                ,
                    //                Role = relationshipAttribute?.Role.ToString() ?? "Referenced"
                    //                ,
                    //                TargetEntityName = relationshipAttribute?.TargetEntityName
                    //            });
                    //        }
                    //        else
                    //        {
                    //            classDefinition.Attributes.Add(new AttributeDefinition
                    //            {
                    //                LogicalName = field.GetValue(null).ToString()
                    //                ,
                    //                Name = field.Name
                    //                ,
                    //                Type = field.FieldType.Name
                    //                ,
                    //                Value = field.GetValue(null).ToString()
                    //                ,
                    //                IsSelected = true
                    //            });
                    //        }
                    //    }
                    //}

                    //definition.AdditionalClassesCollection.Add(classDefinition);
                }

                tables.Add(table);
            }

            return tables;
           
        }




































    }
}
