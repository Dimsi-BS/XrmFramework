using System;

namespace Model
{
    public interface IBindingModel : IXmlModel
    {
        /// <summary>
        /// Id of the record
        /// </summary>
        Guid Id { get; set; }
    }
}