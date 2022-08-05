namespace MEI.SPDocuments.TypeCodes
{
    public enum ProgramExceptionType
    {
        Undefined,
        [EnumDescription("Location", "Location")]
        L,
        [EnumDescription("Speaker", "Speaker")]
        S
    }

    public static class ProgramExceptionTypeExtensions
    {
        private static readonly EnumDescriptionCollection<ProgramExceptionType> Description = new EnumDescriptionCollection<ProgramExceptionType>();

        public static string ToDisplayNameLong(this ProgramExceptionType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this ProgramExceptionType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static ProgramExceptionType ToProgramExceptionType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
