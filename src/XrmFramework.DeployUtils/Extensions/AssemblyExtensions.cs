using System.Reflection;

namespace XrmFramework.DeployUtils.Extensions
{
    internal static class AssemblyExtensions
    {
        public static string GetName(this Assembly assembly)
            => GetAssemblyInfo(assembly).Name;

        public static AssemblyInfo GetAssemblyInfo(this Assembly assembly)
        {
            var fullNameSplit = assembly.FullName.Split(',');

            var infos = new AssemblyInfo
            {
                Name = fullNameSplit[0]
            };

            for (var i = 1; i < fullNameSplit.Length; i++)
            {
                var equalSplit = fullNameSplit[i].Trim().Split('=');

                switch (equalSplit[0].ToLowerInvariant().Trim())
                {
                    case "version":
                        infos.Version = equalSplit[1].Trim();
                        break;
                    case "culture":
                        infos.Culture = equalSplit[1].Trim();
                        break;
                    case "publickeytoken":
                        infos.PublicKeyToken = equalSplit[1].Trim();
                        break;
                }
            }

            return infos;
        }
    }

    public class AssemblyInfo
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Culture { get; set; }

        public string PublicKeyToken { get; set; }
    }
}
