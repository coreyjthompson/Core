using System;
using System.ComponentModel;

namespace MEI.Core.Helpers
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            var field = type.GetField(name);
            if (field == null)
            {
                return null;
            }

            var attr =
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attr?.Description;
        }

        public static string ToName(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return name ?? null;
        }
    }
}
