using System;
namespace Model
{
    public interface IXmlTransform<in T> : IXmlTransform where T : IXmlModel
    {
        void PostXmlConvertion(T model);
        void PreXmlConvertion(T model);
    }

    public interface IXmlTransform
    {
        void PostXmlConvertion(Type type, object model);
        void PreXmlConvertion(Type type, object model);
    }
}
