// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using Model.Sdk;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CrmLookupAttribute : Attribute
    {
        public CrmLookupAttribute(string targetEntityName, string attributeName, bool allowNotExisting = false)
        {
            TargetEntityName = targetEntityName;
            AttributeName = attributeName;
            AllowNotExisting = allowNotExisting;
        }
        public CrmLookupAttribute(Type definitionType, string attributeName, bool allowNotExisting = false)
        {
            DefinitionType = definitionType;
            AttributeName = attributeName;
            AllowNotExisting = allowNotExisting;
        }

        public string RelationshipName { get; set; }

        private Type DefinitionType { get; set; }

        public string TargetEntityName { get; private set; }

        public string AttributeName { get; private set; }

        public bool AllowNotExisting { get; private set; }
    }
}