using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IDtoAttributeConverter
    {
        object ConvertFromDtoAttribute(object value);

        object ConvertToDtoAttribute(object value);
    }
}
