// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.BindingModel.Tests.Fakes
{
    /// <summary>
    /// Simple XML model used to exercise the <see cref="XmlBindingModelMapper"/> (via
    /// <see cref="BindingModelHelper.ToBindingModel{T}(System.Xml.Linq.XElement)"/> and
    /// <see cref="BindingModelHelper.ToXElement{T}"/>).
    /// </summary>
    [XmlMapping("contact")]
    public class SimpleXmlModel : IXmlModel
    {
        [XmlMapping("fullname")]
        public string? FullName { get; set; }

        [XmlMapping("age")]
        public int Age { get; set; }

        [XmlMapping("score")]
        public decimal Score { get; set; }

        [XmlMapping("isactive")]
        public bool IsActive { get; set; }

        [XmlMapping("birthdate")]
        public DateTime? BirthDate { get; set; }

        [XmlMapping("trackingid")]
        public Guid TrackingId { get; set; }

        [XmlMapping("children")]
        public List<ChildXmlModel> Children { get; } = new();
    }

    /// <summary>Child element used by <see cref="SimpleXmlModel"/> to test collection serialization.</summary>
    [XmlMapping("child")]
    public class ChildXmlModel : IXmlModel
    {
        [XmlMapping("name")]
        public string? Name { get; set; }

        [XmlMapping("value")]
        public int Value { get; set; }
    }
}
