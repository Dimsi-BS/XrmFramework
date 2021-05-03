using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace XrmFramework.MSBuild
{
    public class GenerateLoggedServices : Task
    {

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string Configuration { get; set; }

        public override bool Execute()
        {
            var projectName = ProjectPath;

            var globalProperties = new Dictionary<string, string>();
            globalProperties.Add("Configuration", Configuration);
            var output = new Dictionary<string, string>();
            var buildOk = BuildEngine5.BuildProjectFile(projectName, null, globalProperties, output, null);

            if (!buildOk)
            {
                return false;
            }



            return true;
        }
    }
}
