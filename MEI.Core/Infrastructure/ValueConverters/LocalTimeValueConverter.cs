using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace MEI.Core.Infrastructure.ValueConverters
{
    public class LocalTimeValueConverter
        : ValueConverter<LocalTime, long>
    {
        public LocalTimeValueConverter()
            : base(v => v.TickOfDay, v => LocalTime.FromTicksSinceMidnight(v))
        {
        }
    }
}