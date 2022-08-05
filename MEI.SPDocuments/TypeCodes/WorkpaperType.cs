namespace MEI.SPDocuments.TypeCodes
{
    public enum WorkpaperType
    {
        None,
        [EnumDescription("Alcohol", "A")]
        Alcohol,
        [EnumDescription("Catering1", "C1")]
        Catering1,
        [EnumDescription("Catering2", "C2")]
        Catering2
    }

    public static class WorkpaperTypeExtensions
    {
        private static readonly EnumDescriptionCollection<WorkpaperType> Description = new EnumDescriptionCollection<WorkpaperType>();

        public static string ToDisplayNameLong(this WorkpaperType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this WorkpaperType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static WorkpaperType ToWorkpaperType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
