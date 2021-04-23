using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public static class DecimalExtensions
    {
        public static Money ToMoney(this decimal? dec)
        {
            return dec == null ? null : new Money(dec.Value);
        }
    }
}
