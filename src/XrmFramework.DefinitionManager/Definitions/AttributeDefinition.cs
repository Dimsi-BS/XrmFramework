// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;
using DateTimeBehavior = Microsoft.Xrm.Sdk.Metadata.DateTimeBehavior;

namespace DefinitionManager
{
    class AttributeDefinition : AbstractDefinition
    {

        private string _displayName, _type;

        public AttributeDefinition()
        {
            this.PropertyChanged += AttributeDefinition_PropertyChanged;
        }

        [Column("Display Name", 1, 300)]
        public string DisplayName { get { return _displayName; } set { _displayName = value; OnPropertyChanged("DisplayName"); } }

        [Column("Column Type", 2, 100)]
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged("Type"); } }

        public EntityDefinition ParentEntity { get; set; }

        [Mergeable]
        public bool IsPrimaryIdAttribute { get; set; }

        [Mergeable]
        public bool IsPrimaryNameAttribute { get; set; }

        [Mergeable]
        public bool IsPrimaryImageAttribute { get; set; }

        [Mergeable]
        public DateTimeBehavior DateTimeBehavior { get; set; }

        private List<string> _keyNames = new();
        [Mergeable]
        public ICollection<string> KeyNames
        {
            get { return _keyNames; }
            set
            {
                if (value == null)
                {
                    _keyNames.Clear();
                }
                else
                {
                    _keyNames.AddRange(value);
                }
            }
        }

        [Mergeable]
        public ICollection<OneToManyRelationshipMetadata> Relationships
        {
            get { return _relationships; }
            set
            {
                _relationships.Clear();
                if (value != null)
                {
                    _relationships.AddRange(value);
                }
            }
        }

        private EnumDefinition _enum;
        private readonly List<OneToManyRelationshipMetadata> _relationships = new();

        [Mergeable]
        public EnumDefinition Enum
        {
            get { return _enum; }
            set
            {
                _enum = value;
                if (value != null)
                {
                    _enum.PropertyChanged += _enum_PropertyChanged;
                }
                OnPropertyChanged("EnumName");
            }
        }

        public bool ShowEnum
        {
            get
            {
                return IsSelected && Enum != null && IsActive;
            }
        }

        void _enum_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                OnPropertyChanged("EnumName");
            }
        }

        void AttributeDefinition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsSelected":
                    if (IsPrimaryIdAttribute && !IsSelected)
                    {
                        IsSelected = true;
                    }
                    OnPropertyChanged("EnumName");
                    OnPropertyChanged("ShowEnum");
                    break;
                case "IsActive":
                    OnPropertyChanged("ShowEnum");
                    break;
                case "ShowEnum":
                    if (ShowEnum)
                    {
                        Enum.Load();
                    }
                    else if (Enum != null)
                    {
                        Enum.UnLoad();
                    }
                    break;
                case "EnumName":
                    if (!string.IsNullOrEmpty(EnumName))
                    {
                        Enum.Register(this);
                    }
                    else if (Enum != null)
                    {
                        Enum.UnRegister(this);
                    }
                    break;
            }
        }

        [Column("Enum (Picklist)", 3, 100)]
        public string EnumName { get { return Enum == null || !IsSelected ? null : Enum.Name; } }

        [Mergeable]
        public bool IsValidForCreate { protected internal get; set; }

        [Mergeable]
        public bool IsValidForUpdate { protected internal get; set; }

        [Mergeable]
        public bool IsValidForRead { get; set; }

        [Mergeable]
        public bool IsValidForAdvancedFind { protected internal get; set; }

        protected override void MergeInternal(AbstractDefinition definition)
        {
            var def = (AttributeDefinition)definition;

            if (def.IsPrimaryIdAttribute || def.IsPrimaryImageAttribute || def.IsPrimaryNameAttribute || def.KeyNames.Any())
            {
                IsSelected = true;
            }
        }

        [Mergeable]
        public object Value { get; set; }

        [Mergeable]
        public int? StringMaxLength { get; set; }

        [Mergeable]
        public double? MinRange { get; set; }

        [Mergeable]
        public double? MaxRange { get; set; }

        protected override void LoadInternal(bool attach = true)
        {
            if (Enum != null)
            {
                Enum.Load(attach);
            }
        }

        protected override void UnLoadInternal()
        {
            if (Enum != null)
            {
                Enum.UnLoad();
            }
        }
    }
}
