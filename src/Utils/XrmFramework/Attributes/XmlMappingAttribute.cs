// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class MappingAttribute : Attribute
    {
        #region .ctor
        public MappingAttribute(string relativePath)
        {
            RelativePath = relativePath;
        }

        public MappingAttribute(string relativePath, string alternateRelativePath)
            : this(relativePath)
        {
            AlternateRelativePath = alternateRelativePath;
        }

        public MappingAttribute(string relativePath, Type converterType)
            : this(relativePath)
        {
            ConverterType = converterType;
        }

        public MappingAttribute(string relativePath, string alternateRelativePath, Type converterType)
            :this(relativePath, alternateRelativePath)
        {
            ConverterType = converterType;
        }
        #endregion

        public string RelativePath { get; private set; }

        public string AlternateRelativePath { get; private set; }

        public Type ConverterType { get; private set; }

        private bool _validForExport = true;
        public bool ValidForExport { get { return _validForExport; } set { _validForExport = value; } }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class XmlMappingAttribute : MappingAttribute
    {
        #region .ctor
        public XmlMappingAttribute(string relativePath) : base(relativePath)
        {
        }

        public XmlMappingAttribute(string relativePath, string alternateRelativePath)
            : base(relativePath, alternateRelativePath)
        {
        }

        public XmlMappingAttribute(string relativePath, Type converterType)
            : base(relativePath, converterType)
        {
        }

        public XmlMappingAttribute(string relativePath, string alternateRelativePath, Type converterType)
            :base(relativePath, alternateRelativePath, converterType)
        {
        }
        #endregion

        public bool IsAttribute { get; set; }
    }

    
    [AttributeUsage(AttributeTargets.Property)]
    public class DtoFieldMappingAttribute : MappingAttribute
    {
        #region .ctor
        public DtoFieldMappingAttribute(string dtoFieldName)
            : base(dtoFieldName)
        {
        }

        public DtoFieldMappingAttribute(string dtoFieldName, Type converterType)
            : base(dtoFieldName, converterType)
        {
        }
        public DtoFieldMappingAttribute(string dtoFieldName, string dtoObjectName)
            : base(dtoFieldName, dtoObjectName)
        {
        }

        public DtoFieldMappingAttribute(string dtoFieldName, string dtoObjectName, Type converterType)
            : base(dtoFieldName, dtoObjectName, converterType)
        {
        }
        #endregion
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class DtoObjectMappingAttribute : MappingAttribute
    {
        #region .ctor
        public DtoObjectMappingAttribute(string dtoClassName)
            : base(dtoClassName)
        {
        }

        public DtoObjectMappingAttribute(string dtoClassName, Type converterType)
            : base(dtoClassName, converterType)
        {
        }
        #endregion
    }
}
