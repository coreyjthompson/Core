namespace MEI.SPDocuments.TypeCodes
{
    public enum SPFieldType
    {
        None,
        [EnumDescription("Text", "Text")]
        Text,
        [EnumDescription("Number", "Number")]
        Number,
        [EnumDescription("Note", "Note")]
        Note,
        [EnumDescription("Counter", "Counter")]
        Counter,
        [EnumDescription("DateTime", "DateTime", "date time")]
        DateTime,
        [EnumDescription("Boolean", "Boolean")]
        Boolean,
        [EnumDescription("Guid", "Guid")]
        Guid,
        [EnumDescription("Integer", "Integer")]
        Integer,
        [EnumDescription("Choice", "Choice")]
        Choice
    }

    public static class FieldTypeExtensions
    {
        private static readonly EnumDescriptionCollection<SPFieldType> Description = new EnumDescriptionCollection<SPFieldType>();

        public static string ToDisplayNameLong(this SPFieldType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this SPFieldType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static SPFieldType ToFieldType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
