namespace MEI.SPDocuments.TypeCodes
{
    public enum SearchBooleanLogic
    {
        None,
        [EnumDescription("And", "And")]
        And,
        [EnumDescription("Or", "Or")]
        Or
    }

    public static class SearchBooleanLogicExtensions
    {
        private static readonly EnumDescriptionCollection<SearchBooleanLogic> Description = new EnumDescriptionCollection<SearchBooleanLogic>();

        public static string ToDisplayNameLong(this SearchBooleanLogic typeCode)
        {
            return Description.CodeToDisplayNameLong(typeCode);
        }

        public static string ToDisplayNameShort(this SearchBooleanLogic typeCode)
        {
            return Description.CodeToDisplayNameShort(typeCode);
        }

        public static SearchBooleanLogic ToSearchBooleanLogic(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
