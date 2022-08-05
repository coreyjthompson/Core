namespace MEI.SPDocuments.TypeCodes
{
    public enum UseType
    {
        None,
        [EnumDescription("Development", "Development")]
        Development,
        [EnumDescription("Production", "Production")]
        Production
    }

    public static class UseTypeExtensions
    {
        private static readonly EnumDescriptionCollection<UseType> Description = new EnumDescriptionCollection<UseType>();

        public static string ToDisplayNameLong(this UseType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this UseType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static UseType ToUseType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
