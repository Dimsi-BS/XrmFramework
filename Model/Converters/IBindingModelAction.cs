using System;
namespace Model
{
    interface IBindingModelAction<T>
     where T : Model.IBindingModel
    {
        string PostConvertion(T model);
    }
}
