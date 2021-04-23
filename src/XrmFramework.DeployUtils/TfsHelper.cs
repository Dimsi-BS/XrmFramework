// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XrmFramework.DeployUtils
{
    public static class TfsHelper
    {
        public class FileInfo
        {
            public FileInfo(string newName)
            {
                NewName = newName;
            }
            public FileInfo(string newName, string oldName)
                : this(newName)
            {
                OldName = oldName;
            }
            public string NewName { get; private set; }
            public string OldName { get; private set; }
        }

        public static void EnsureReferencesInProjectFile(string projectPath, IEnumerable<FileInfo> fileNames, string relativePath, string linkPath = null)
        {
            string content = string.Empty;
            var updateNeeded = false;

            using (var stream = File.OpenText(projectPath))
            {
                content = stream.ReadToEnd();
            }
            var xml = XDocument.Parse(content, LoadOptions.PreserveWhitespace);

            foreach (var file in fileNames)
            {
                updateNeeded |= AddMissingFile(xml, file, Path.GetDirectoryName(projectPath), relativePath, linkPath);
            }
            if (updateNeeded)
            {
                xml.Save(projectPath);
            }
        }

        private const string ProjectNs = "http://schemas.microsoft.com/developer/msbuild/2003";

        private static bool AddMissingFile(XDocument xml, FileInfo file, string rootPath, string relativePath, string linkPath)
        {
            var compileName = XName.Get("Compile", ProjectNs);
            var newFilePath = Path.Combine(relativePath, file.NewName);

            var firstCompileNode = xml.Descendants(compileName).FirstOrDefault();

            var itemGroup = firstCompileNode != null ? firstCompileNode.Parent : xml.Descendants(XName.Get("ItemGroup", ProjectNs)).FirstOrDefault();

            if (itemGroup == null)
            {
                throw new Exception("Unable to edit LoggedServices Shared Project");
            }

            if (itemGroup.Descendants(compileName).Any(x => x.Attributes().Any(a => String.Compare(a.Value, newFilePath, StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(file.OldName))
            {
                var oldFilePath = Path.Combine(relativePath, file.OldName);
                var oldCompileInfo = itemGroup.Descendants(compileName).FirstOrDefault(x => x.Attributes().Any(a => a.Value.Contains(oldFilePath)));

                if (oldCompileInfo != null)
                {
                    oldCompileInfo.Remove();
                    var oldFileInfo = new System.IO.FileInfo(Path.Combine(rootPath, oldFilePath));
                    oldFileInfo.Delete();
                }
            }

            var compileInfo = new XElement(compileName);
            compileInfo.Add(new XAttribute(XName.Get("Include"), newFilePath));
            itemGroup.Add(compileInfo);
            return true;
        }
    }
}
