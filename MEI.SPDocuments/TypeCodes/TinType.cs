namespace MEI.SPDocuments.TypeCodes
{
    public enum TinType
    {
        Undefined,
        [EnumDescription("EIN", "EIN")]
        Ein,
        [EnumDescription("SSN", "SSN")]
        Ssn,
        [EnumDescription("Both", "Both")]
        Both
    }

    public static class TinTypeExtensions
    {
        private static readonly EnumDescriptionCollection<TinType> Description = new EnumDescriptionCollection<TinType>();

        public static string ToDisplayNameLong(this TinType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this TinType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static TinType ToTinType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
