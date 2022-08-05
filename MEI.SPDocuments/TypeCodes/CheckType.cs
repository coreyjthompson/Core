namespace MEI.SPDocuments.TypeCodes
{
    public enum CheckType
    {
        Undefined,
        [EnumDescription("A/V", "A")]
        AV,
        [EnumDescription("MEI", "M")]
        MEI,
        [EnumDescription("Catering", "C")]
        Catering,
        [EnumDescription("Expense", "E")]
        Expense,
        [EnumDescription("Honorarium", "H")]
        Honorarium,
        [EnumDescription("Limousine", "L")]
        Limousine,
        [EnumDescription("Production", "P")]
        Production,
        [EnumDescription("Room", "R")]
        Room,
        [EnumDescription("Fee", "F")]
        Fee,
        [EnumDescription("Cancellation Honoraria", "CH")]
        CancellationHonoraria,
        [EnumDescription("Cancellation Other", "CO")]
        CancellationOther
    }

    public static class CheckTypeExtensions
    {
        private static readonly EnumDescriptionCollection<CheckType> Description = new EnumDescriptionCollection<CheckType>();

        public static string ToDisplayNameLong(this CheckType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this CheckType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static CheckType ToCheckType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
