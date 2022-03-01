using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.DeployUtils.Model
{
    public class WebResource : IEquatable<WebResource>
    {
        public WebResource(FileInfo fi, DirectoryInfo root, string prefix)
        {
            AllLines.AddRange(File.ReadAllLines(fi.FullName));
            Content = AllLines.Any() ? string.Join("\r\n", AllLines) : null;

            FullName = GetRelativePath(fi.FullName, root.FullName, prefix);

            var regex = new Regex("///.*<reference.*path=\"(.*)\".*/>");

            foreach (var line in AllLines)
            {
                var match = regex.Match(line);

                if (!match.Success) continue;

                var dependencyFullName = Path.Combine(fi.DirectoryName, match.Groups[1].Value);

                Dependencies.Add(GetRelativePath(dependencyFullName, root.FullName, prefix));
            }
        }

        public WebResource(Entity wr)
        {
            Id = wr.Id;

            var wrContent = wr.GetAttributeValue<string>("content");

            if (!string.IsNullOrWhiteSpace(wrContent))
            {
                Content = Encoding.UTF8.GetString(Convert.FromBase64String(wrContent));
            }

            FullName = wr.GetAttributeValue<string>("name");

            var registeredDependencies = wr.GetAttributeValue<string>("dependencyxml");

            if (!string.IsNullOrWhiteSpace(registeredDependencies))
            {
                var dependencies = XElement.Parse(registeredDependencies);

                var elements = dependencies.XPathSelectElements("//Dependency/Library");

                foreach (var name in elements.Attributes("name").Select(a => a.Value))
                {
                    Dependencies.Add(name);
                }
            }
        }

        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Extension => Path.GetExtension(FullName);

        private List<string> AllLines { get; } = new();

        public string Content { get; }

        private SortedSet<string> Dependencies { get; } = new();

        public string GetDependenciesXml()
            => Dependencies.Any() ? new XElement("Dependencies",
            Dependencies.Select(d =>
            new XElement("Dependency", new XAttribute("componentType", "WebResource"),
                new XElement("Library",
                    new XAttribute("name", d),
                    new XAttribute("displayName", d),
                    new XAttribute("languagecode", string.Empty),
                    new XAttribute("description", string.Empty),
                    new XAttribute("libraryUniqueId", Guid.NewGuid())
                ))).Cast<object>().ToArray()
            ).ToString() : null;

        private static string GetRelativePath(string fileFullName, string folderName, string prefix)
        {
            var pathUri = new Uri(fileFullName);
            // Folders must end in a slash
            if (!folderName.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                folderName += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(folderName);

            var relativeUrl = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));

            return $"{prefix}_/{relativeUrl.Replace("\\", "/")}";
        }

        public bool Equals(WebResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FullName == other.FullName && Content == other.Content && Dependencies.SequenceEqual(other.Dependencies);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WebResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FullName != null ? FullName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Dependencies != null ? Dependencies.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
