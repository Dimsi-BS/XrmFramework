// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace XrmFramework.BindingModel
{
    [DataContract]
    public abstract class BindingModelBase : IBindingModel, INotifyPropertyChanged
    {

        protected BindingModelBase()
        {
            PropertyChanged += BindingModelBase_PropertyChanged;
        }

        void BindingModelBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_initializedProperties == null)
            {
                _initializedProperties = new HashSet<string>();
            }

            _initializedProperties.Add(e.PropertyName);

            var dependantAttributes = this.GetType().GetProperty(e.PropertyName).GetCustomAttributes<DependentAttribute>();

            foreach (var depAttribute in dependantAttributes)
            {
                OnPropertyChanged(depAttribute.AttributeName);
            }
        }

        private HashSet<string> _initializedProperties = new HashSet<string>();

        public IReadOnlyCollection<string> InitializedProperties
        {
            get
            {
                if (_initializedProperties == null)
                {
                    _initializedProperties = new HashSet<string>();
                    PropertyChanged += BindingModelBase_PropertyChanged;

                    InitProperties();
                }

                return new List<string>(_initializedProperties);
            }
        }

        protected virtual void InitProperties()
        {
            var emptyObject = Activator.CreateInstance(GetType());

            foreach (var prop in GetType().GetProperties())
            {
                if (!Equals(prop.GetValue(this), prop.GetValue(emptyObject)))
                {
                    _initializedProperties.Add(prop.Name);
                }
            }
        }

        public virtual Guid Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
