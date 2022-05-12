using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Linq;
using XrmFramework.Generator.Generators;

namespace XrmFramework.MSBuild
{
    public class GenerateCodeBehindTableFiles : Task
    {
       [Required]
       public string CoreProjectPath { get; set; }
       //
       //
       [Required]
       public ITaskItem[] TableFiles { get; set; }
       //
       //public ITaskItem[] CodeBehindFiles { get; set; }

        public override bool Execute()
        {

            var coreProjectPath = CoreProjectPath.Remove(CoreProjectPath.Length-1);
            //fz
            Log.LogMessage(MessageImportance.High,"The table class generation task has been launched");
            var cPathSplit = coreProjectPath.Split('\\');
            var coreProjectName = cPathSplit[cPathSplit.Length - 1];
            Log.LogMessage(MessageImportance.High, $"Core project name is {coreProjectName}");


            var tableFiles = TableFiles?.Select(i => i.ItemSpec).ToArray() ?? Array.Empty<string>();
            if(TableFiles == null)
            {
                Log.LogMessage(MessageImportance.High, "TableFiles variable is null");
                return true;
            }

            //Log path to tablefiles if necessary
            foreach (var tableFile in tableFiles)
            {
                //Log.LogMessage(MessageImportance.High,$"{CoreProjectPath}{tableFile}");

            }

            DefinitionClassGenerator.GenerateTableClassesAtBuild(CoreProjectPath, coreProjectName, tableFiles);


            return true;
        }
    }
    
    
}
