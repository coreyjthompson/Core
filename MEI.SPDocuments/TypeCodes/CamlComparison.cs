namespace MEI.SPDocuments.TypeCodes
{
    public enum CamlComparison
    {
        None,
        [EnumDescription("BeginsWith", "BeginsWith", "begins with")]
        BeginsWith,
        [EnumDescription("Contains", "Contains")]
        Contains,
        [EnumDescription("DateRangesOverlap", "DateRangesOverlap", "date ranges overlap")]
        DateRangesOverlap,
        [EnumDescription("Eq", "Eq", "equal")]
        Equal,
        [EnumDescription("Gt", "Gt", "greaterthan", "greater than")]
        GreaterThan,
        [EnumDescription("Geq", "Geq", "greaterthanequal", "greater than equal")]
        GreaterThanOrEqualTo,
        [EnumDescription("IsNotNull", "IsNotNull", "is not null")]
        IsNotNull,
        [EnumDescription("IsNull", "IsNull", "is null")]
        IsNull,
        [EnumDescription("Lt", "Lt", "lessthan", "less than")]
        LessThan,
        [EnumDescription("Leq", "Leq", "lessthanequalto", "less than equal to")]
        LessThanOrEqualTo,
        [EnumDescription("Neq", "Neq", "notequal", "not equal")]
        NotEqual
    }

    public static class CamlComparisonExtensions
    {
        private static readonly EnumDescriptionCollection<CamlComparison> Description = new EnumDescriptionCollection<CamlComparison>();

        public static string ToDisplayNameLong(this CamlComparison code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this CamlComparison code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static CamlComparison ToCamlComparison(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
