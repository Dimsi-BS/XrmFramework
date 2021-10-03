using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using XrmFramework.Generator.Interfaces;

namespace XrmFramework.Generator
{
    public static class TableFileInputExtensions
    {
        public static TextReader GetTableFileContentReader(this TableFileInput tableFileInput, ProjectSettings projectSettings)
        {
            if (tableFileInput == null) throw new ArgumentNullException("tableFileInput");

            if (tableFileInput.TableFileContent != null)
                return new StringReader(tableFileInput.TableFileContent);

            Debug.Assert(projectSettings != null);

            return new StreamReader(Path.Combine(projectSettings.ProjectFolder, tableFileInput.ProjectRelativePath));
        }

        public static string GetFullPath(this TableFileInput tableFileInput, ProjectSettings projectSettings)
        {
            if (tableFileInput == null) throw new ArgumentNullException("tableFileInput");

            if (projectSettings == null)
                return tableFileInput.ProjectRelativePath;

            return Path.GetFullPath(Path.Combine(projectSettings.ProjectFolder, tableFileInput.ProjectRelativePath));
        }

        public static string GetGeneratedTestFullPath(this TableFileInput tableFileInput, ProjectSettings projectSettings)
        {
            if (tableFileInput == null) throw new ArgumentNullException("tableFileInput");

            if (tableFileInput.GeneratedTestProjectRelativePath == null)
                return null;

            if (projectSettings == null)
                return tableFileInput.GeneratedTestProjectRelativePath;

            return Path.GetFullPath(Path.Combine(projectSettings.ProjectFolder, tableFileInput.GeneratedTestProjectRelativePath));
        }

        public static string GetGeneratedTestContent(this TableFileInput tableFileInput, string generatedTestFullPath)
        {
            var generatedTestFileContent = tableFileInput.GeneratedTestFileContent;
            if (generatedTestFileContent != null)
                return generatedTestFileContent;

            if (generatedTestFullPath == null)
                return null;

            try
            {
                if (!File.Exists(generatedTestFullPath))
                    return null;

                return File.ReadAllText(generatedTestFullPath);
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception, "TableFileInputExtensions.GetGeneratedTestContent");
                return null;
            }
        }
    }
}
