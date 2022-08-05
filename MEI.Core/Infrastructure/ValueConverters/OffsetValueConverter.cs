using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace MEI.Core.Infrastructure.ValueConverters
{
    public class OffsetValueConverter
        : ValueConverter<Offset, long>
    {
        public OffsetValueConverter()
            : base(v => v.Nanoseconds, v => Offset.FromNanoseconds(v))
        {
        }
    }
}