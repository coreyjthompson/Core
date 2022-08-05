using System;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using NodaTime;

namespace MEI.Core.Infrastructure.ValueConverters
{
    public class LocalDateValueConverter
        : ValueConverter<LocalDate, DateTime>
    {
        public LocalDateValueConverter()
            : base(v => v.ToDateTimeUnspecified(), v => LocalDate.FromDateTime(v))
        {
        }
    }
}