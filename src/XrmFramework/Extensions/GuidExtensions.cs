using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Convert given string to Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="entityLogicalName"></param>
        /// <returns></returns>
        public static EntityReference ToEntityReference(this Guid guid, string entityLogicalName)
        {
            entityLogicalName.ShouldNotBeNull();

            return new EntityReference(entityLogicalName, guid);
        }

    }
}