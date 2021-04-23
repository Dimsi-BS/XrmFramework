namespace XrmFramework.BindingModel
{
    public abstract class ModelPropertyConverter : IConverter
    {
        public virtual object ConvertFrom(object value)
        {
            return null;
        }
    }

    public interface IConverter
    {
        object ConvertFrom(object value);
    }
}
