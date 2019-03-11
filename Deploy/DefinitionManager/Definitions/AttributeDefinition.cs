using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;

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

        [Column("Attribute Type", 2, 100)]
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

        private List<string> _keyNames = new List<string>();
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
        private readonly List<OneToManyRelationshipMetadata> _relationships = new List<OneToManyRelationshipMetadata>();

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
        public bool IsValidForCreate { protected get; set; }

        [Mergeable]
        public bool IsValidForUpdate { protected get; set; }

        [Mergeable]
        public bool IsValidForRead { protected get; set; }

        [Mergeable]
        public bool IsValidForAdvancedFind { protected get; set; }

        public string Summary
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("\t\t\t/// <summary>");
                sb.AppendLine("\t\t\t/// ");
                sb.AppendFormat("\t\t\t/// Type : {0}{1}\r\n", Type, Enum == null ? "" : " (" + Enum.Name + ")");
                sb.Append("\t\t\t/// Validity :  ");

                var isFirst = true;
                if (IsValidForRead)
                {
                    isFirst = false;
                    sb.Append("Read ");
                }

                if (IsValidForCreate)
                {
                    if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                    sb.Append("Create ");
                }

                if (IsValidForUpdate)
                {
                    if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                    sb.Append("Update ");
                }

                if (IsValidForAdvancedFind)
                {
                    if (isFirst) { isFirst = false; } else { sb.Append("| "); }
                    sb.Append("AdvancedFind ");
                }
                sb.Append("\r\n");

                sb.AppendLine("\t\t\t/// </summary>");


                return sb.ToString();
            }
        }

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
