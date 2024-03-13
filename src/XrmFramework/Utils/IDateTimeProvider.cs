using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.Utils
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateTime UtcNow { get; }

        DateTime Today { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
#pragma warning disable XRM0300
        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Today => DateTime.Today;
#pragma warning restore XRM0300
    }
}
