namespace MEI.SPDocuments.TypeCodes
{
    public enum ReceiptType
    {
        Undefined,
        [EnumDescription("A/V", "A")]
        AV,
        [EnumDescription("Room", "R")]
        Room,
        [EnumDescription("Catering", "C")]
        Catering,
        [EnumDescription("MEI", "M")]
        MEI,
        [EnumDescription("Shipping", "S")]
        Shipping,
        [EnumDescription("Production", "P")]
        Production
    }

    public static class ReceiptTypeExtensions
    {
        private static readonly EnumDescriptionCollection<ReceiptType> Description = new EnumDescriptionCollection<ReceiptType>();

        public static string ToDisplayNameLong(this ReceiptType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this ReceiptType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static ReceiptType ToReceiptType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
