// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.Generated
{
    /// <summary>
    /// Simule la classe émise par <c>XrmFramework.PluginManifest.Generator</c>, embarquée
    /// dans cet assembly de test, pour vérifier la lecture du const par
    /// <c>PluginManifestReader.ReadManifestJson</c>.
    /// </summary>
    public static class PluginManifest
    {
        public const string Json =
            "{\"plugins\":[{\"fullName\":\"Emb.MyPlugin\",\"steps\":[" +
            "{\"message\":\"Create\",\"stage\":\"PreOperation\",\"mode\":\"Synchronous\",\"entityName\":\"account\"," +
            "\"methodName\":\"OnCreate\",\"methodNames\":[\"OnCreate\"],\"filteringAttributes\":[],\"order\":1," +
            "\"impersonationUsername\":\"\",\"unsecureConfig\":null," +
            "\"preImage\":{\"allAttributes\":false,\"attributes\":[]},\"postImage\":{\"allAttributes\":false,\"attributes\":[]}}" +
            "]}],\"workflows\":[],\"customApis\":[]}";
    }
}
