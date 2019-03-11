using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model
{
    public class OptionSetToSwaggerAttribute : RegularExpressionAttribute
    {
        public OptionSetToSwaggerAttribute(Type enumType, bool addEmptyValue = true) : base(GetPattern(enumType, addEmptyValue))
        {
        }

        private static string GetPattern(Type enumType, bool addEmptyValue)
        {
            var pattern = new StringBuilder("(");
            if (addEmptyValue) pattern.Append(" ");
            var isFirst = !addEmptyValue;

            foreach (int value in Enum.GetValues(enumType))
            {
                if (value == 0 && Enum.GetName(enumType, value) == "Null")
                {
                    continue;
                }

                if (isFirst)
                {
                    isFirst = false;}
                else
                {
                    pattern.Append("|");
                }


                pattern.Append($"{value}");
            }

            pattern.Append(")");

            return pattern.ToString();
        }
    }
}
