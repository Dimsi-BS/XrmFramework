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
        static readonly TableCollection Tables = new TableCollection();
        
        private readonly string _definitionDirectory;
        private readonly Assembly _assembly;
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

            Assembly assembly;
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

        private Program(string definitionDirectory, Assembly assembly)
        {
            this._definitionDirectory = definitionDirectory;
            this._assembly = assembly;
        }

        private void Run()
        {
            var codedTables = GetCodedTables(_assembly);

            foreach (var table in codedTables)
            {
                var txt = JsonConvert.SerializeObject(table, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                var fileInfo = new FileInfo($"{_definitionDirectory}/{table.Name}.table");
                File.WriteAllText(fileInfo.FullName, txt);
                Console.WriteLine(table.Name);
            }
        }

        private IEnumerable<Table> GetCodedTables(Assembly definitionAssembly)
        {
            var entityDefinitionAttributeType = definitionAssembly.GetType("XrmFramework.EntityDefinitionAttribute");
            var definitionTypes = definitionAssembly.GetTypes().Where(t => t.GetCustomAttributes(entityDefinitionAttributeType, false).Any());
            
            var definitionManagerIgnoreAttributeType = (definitionAssembly.GetType("XrmFramework.Definitions.Internal.DefinitionManagerIgnoreAttribute") ??
                                                        definitionAssembly.GetType("XrmFramework.Internal.DefinitionManagerIgnoreAttribute")) ??
                                                       definitionAssembly.GetType("Model.DefinitionManagerIgnoreAttribute");

            Tables.Add(_optionSetTable);

            var types = definitionTypes.ToList();
            Console.WriteLine($"{types.Count} definitions were found.");

            foreach (var table in types.Select(t => MapToTable(t, definitionManagerIgnoreAttributeType)))
            {
                Tables.Add(table);
            }

            return Tables;

        }

        private Table MapToTable(Type type, Type definitionManagerIgnoreAttributeType)
        {
            Console.WriteLine($"Adding {type.Name} to tables.");
            if (type.GetCustomAttributes(definitionManagerIgnoreAttributeType).Any())
            {
                return null;
            }

            var table = InitTable(type);

            foreach (var field in type.GetNestedType("Columns").GetFields())
            {
                var column = 
                    MapToColumn(field, table.Enums.Add);
                
                table.Columns.Add(column);
            }

            table.ManyToManyRelationships
                .AddRange(
                    WorkOnRelationship("ManyToManyRelationships", type)
                    );
            table.ManyToOneRelationships
                .AddRange(
                    WorkOnRelationship("ManyToOneRelationships", type)
                );
            table.OneToManyRelationships
                .AddRange(
                    WorkOnRelationship("OneToManyRelationships", type)
                );

            return table;
        }

        private IEnumerable<Relation> WorkOnRelationship(string relationshipName, Type type)
        {
            if (type.GetNestedType(relationshipName) == null)
            {
                foreach (var relation in Enumerable.Empty<Relation>())
                {
                    yield return relation;
                }
            }

            foreach (var field in type.GetNestedType(relationshipName).GetFields())
            {
                yield return MapToRelation(field);
            }
        }

        private static Relation MapToRelation(FieldInfo field)
        {
            var attribute = field.CustomAttributes.Single();
            var constructorArguments = attribute.ConstructorArguments;

            var relation = new Relation
            {
                Name = field.Name,
                EntityName = (string)constructorArguments[0].Value,
                Role = (EntityRole)(int)constructorArguments.Skip(1).First().Value,
                NavigationPropertyName = (string)constructorArguments.Skip(2).First().Value,
                LookupFieldName = (string)constructorArguments[constructorArguments.Count - 1].Value
            };

            return relation;
        }

        private Column MapToColumn(FieldInfo field, Action<OptionSetEnum> addEnum)
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
                        column.PrimaryType =
                            (PrimaryAttributeType)customAttribute.ConstructorArguments.Single().Value switch
                            {
                                PrimaryAttributeType.Id => PrimaryType.Id,
                                PrimaryAttributeType.Name => PrimaryType.Name,
                                PrimaryAttributeType.Image => PrimaryType.Image,
                                _ => PrimaryType.None
                            };
                        break;
                    case "XrmFramework.OptionSetAttribute":
                        var enumType = (Type)customAttribute.ConstructorArguments.Single().Value;

                        var enu = GetEnumFromType(enumType);

                        column.EnumName = enu.LogicalName;

                        if (!enu.IsGlobal)
                            addEnum(enu);
                        else
                        {
                            if (_optionSetTable.Enums.TrueForAll(e => e.LogicalName != enu.LogicalName))
                                _optionSetTable.Enums.Add(enu);
                        }

                        break;
                }
            }

            return column;
        }

        private static Table InitTable(Type type)
        {
            return new Table
            {
                Name = type.Name.Replace("Definition", ""),
                LogicalName = type.GetField("EntityName").GetValue(null) as string,
                CollectionName = type.GetField("EntityCollectionName")?.GetValue(null) as string,
                Selected = true
            };
        }


        private OptionSetEnum GetEnumFromType(Type enumType)
        {
            var constructorArguments = enumType.CustomAttributes.First().ConstructorArguments;

            var enumName = constructorArguments.Count switch
            {
                1 => (string)constructorArguments.Single().Value,
                _ => $"{constructorArguments[0].Value}|{constructorArguments[constructorArguments.Count - 1].Value}"
            };

            var e = new OptionSetEnum
            {
                Name = enumType.Name,
                LogicalName = enumName,
                IsGlobal = constructorArguments.Count == 1,
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
