namespace MEI.SPDocuments.TypeCodes
{
    public enum InvalidSPDocFileNameExceptionType
    {
        Undefined,
        [EnumDescription("ElementCount", "ElementCount", "element count")]
        ElementCount,
        [EnumDescription("FirstElement", "FirstElement", "first element")]
        FirstElement,
        [EnumDescription("ElementType", "ElementType", "element type")]
        ElementType,
        [EnumDescription("Ext", "Ext")]
        Ext,
        [EnumDescription("NoDBMatch", "NoDBMatch", "no db match")]
        NoDBMatch,
        [EnumDescription("InvalidCompany", "InvalidCompany", "invalid company")]
        InvalidCompany
    }

    public static class InvalidFileNameTypeExtensions
    {
        private static readonly EnumDescriptionCollection<InvalidSPDocFileNameExceptionType> Description = new EnumDescriptionCollection<InvalidSPDocFileNameExceptionType>();

        public static string ToDisplayNameLong(this InvalidSPDocFileNameExceptionType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this InvalidSPDocFileNameExceptionType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static InvalidSPDocFileNameExceptionType ToInvalidFileNameExceptionType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
