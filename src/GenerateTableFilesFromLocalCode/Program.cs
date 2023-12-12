using Microsoft.Xrm.Sdk;
using System;
using XrmFramework.Core;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XrmFramework;
using EntityRole = XrmFramework.EntityRole;

namespace GenerateTableFilesFromLocalCode
{
    internal class Program
    {
        static TableCollection tables = new TableCollection();
        EntityCollection entities;
        private readonly string definitionDirectory;
        private readonly Assembly assembly;
        private readonly Table _optionSetTable = new Table { LogicalName = "globalEnums", Name = "OptionSets" };

        static void Main(string[] args)
        {
            if (args.Length != 2)
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

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine();
                Console.WriteLine($"Error argument \"{args[1]}\" is invalid, reminder : it should be the path to the directory in which you want to save your .table files");
                Console.WriteLine();
                Console.ReadKey();

                return;

            }

            var program = new Program(args[1], assembly);

            program.Run();
        }

        public Program(string definitionDirectory, Assembly assembly)
        {
            this.definitionDirectory = definitionDirectory;
            this.assembly = assembly;
        }

        private void Run()
        {
            var codedTables = GetCodedTables(assembly);

            string txt;
            foreach (var table in codedTables)
            {
                txt = JsonConvert.SerializeObject(table, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                var fileInfo = new FileInfo($"{definitionDirectory}/{table.Name}.table");
                File.WriteAllText(fileInfo.FullName, txt);
                Console.WriteLine(table.Name);
            }
        }

        public IEnumerable<Table> GetCodedTables(Assembly definitionAssembly)
        {
            var entityDefinitionAttributeType = definitionAssembly.GetType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = definitionAssembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            var relationshipAttributeType = definitionAssembly.GetType("XrmFramework.RelationshipAttribute");
            var definitionManagerIgnoreAttributeType = definitionAssembly.GetType("XrmFramework.Internal.DefinitionManagerIgnoreAttribute");
            if (definitionManagerIgnoreAttributeType == null)
            {
                definitionManagerIgnoreAttributeType = definitionAssembly.GetType("Model.DefinitionManagerIgnoreAttribute");

            }

            tables.Add(_optionSetTable);

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
                    Name = t.Name.Replace("Definition", "")
                    ,
                    LogicalName = t.GetField("EntityName").GetValue(null) as string
                    ,
                    CollectionName = t.GetField("EntityCollectionName")?.GetValue(null) as string
                    ,
                    Selected = true
                };



                foreach (var field in t.GetNestedType("Columns").GetFields())
                {
                    var column = new Column
                    {
                        LogicalName = field.GetValue(null) as string,
                        Name = field.Name,
                        Selected = true

                    };

                    foreach (var customAttribute in field.CustomAttributes)
                    {
                        switch (customAttribute.AttributeType.FullName)
                        {
                            case "XrmFramework.AttributeMetadataAttribute":
                                column.Type = (AttributeTypeCode)customAttribute.ConstructorArguments.Single().Value;
                                break;
                            case "XrmFramework.PrimaryAttributeAttribute":
                                column.PrimaryType = ((PrimaryAttributeType)customAttribute.ConstructorArguments.Single().Value) switch
                                {
                                    PrimaryAttributeType.Id => PrimaryType.Id,
                                    PrimaryAttributeType.Name => PrimaryType.Name,
                                    PrimaryAttributeType.Image => PrimaryType.Image,
                                    _ => throw new ArgumentOutOfRangeException()
                                };
                                break;
                            case "XrmFramework.OptionSetAttribute":
                                var enumType = (Type)customAttribute.ConstructorArguments.Single().Value;

                                var enu = GetEnumFromType(enumType);

                                column.EnumName = enu.LogicalName;

                                if (!enu.IsGlobal)
                                    table.Enums.Add(enu);
                                else
                                {
                                    if (_optionSetTable.Enums.All(e => e.LogicalName != enu.LogicalName))
                                        _optionSetTable.Enums.Add(enu);
                                }
                                break;
                        }
                    }


                    table.Columns.Add(column);
                }


                Action<string, ICollection<Relation>> workOnRelationship = (relationshipName, list) =>
                {
                    if (t.GetNestedType(relationshipName) != null)
                    {
                        foreach (var field in t.GetNestedType(relationshipName).GetFields())
                        {
                            var attribute = field.CustomAttributes.Single();

                            list.Add(new Relation
                            {
                                Name = field.Name,
                                EntityName = (string)attribute.ConstructorArguments.First().Value,
                                Role = (EntityRole)(int)attribute.ConstructorArguments.Skip(1).First().Value,
                                NavigationPropertyName = (string)attribute.ConstructorArguments.Skip(2).First().Value,
                                LookupFieldName = (string)attribute.ConstructorArguments.Last().Value
                            });
                        }
                    }
                };

                workOnRelationship("ManyToManyRelationships", table.ManyToManyRelationships);
                workOnRelationship("ManyToOneRelationships", table.ManyToOneRelationships);
                workOnRelationship("OneToManyRelationships", table.OneToManyRelationships);

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

        private OptionSetEnum GetEnumFromType(Type enumType)
        {
            var enumAttribute = enumType.CustomAttributes.First();

            var enumName = enumAttribute.ConstructorArguments.Count == 1 ? (string)enumAttribute.ConstructorArguments.Single().Value : $"{enumAttribute.ConstructorArguments.First().Value}|{enumAttribute.ConstructorArguments.Last().Value}";

            var e = new OptionSetEnum
            {
                Name = enumType.Name,
                LogicalName = enumName,
                IsGlobal = enumAttribute.ConstructorArguments.Count == 1,
            };

            foreach (var field in enumType.GetFields())
            {
                if (field.IsSpecialName)
                    continue;

                var value = (int)field.GetValue(null);
                var name = field.Name;

                if (value == 0 && name == "Null")
                    e.HasNullValue = true;
                else
                    e.Values.Add(new OptionSetEnumValue { Name = name, Value = value });
            }
            return e;
        }
    }
}
