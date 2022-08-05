namespace MEI.SPDocuments.TypeCodes
{
    public enum ExpenseType
    {
        Undefined,
        [EnumDescription("Air", "ai")]
        Air,
        [EnumDescription("AirCharge", "ac", "air charge")]
        AirCharge,
        [EnumDescription("AudioVisual", "av", "audio visual")]
        AudioVisual,
        [EnumDescription("CancelFee", "cn", "cancel fee")]
        CancelFee,
        [EnumDescription("Catering", "ca")]
        Catering,
        [EnumDescription("ChangeFee", "ch", "change fee")]
        ChangeFee,
        [EnumDescription("GroundTransport", "gt", "ground transport")]
        GroundTransport,
        [EnumDescription("Honoraria", "hn")]
        Honoraria,
        [EnumDescription("Hotel", "ho")]
        Hotel,
        [EnumDescription("HotelCharge", "hc", "hotel charge")]
        HotelCharge,
        [EnumDescription("InvitationOnlyFee", "io", "invitation only fee")]
        InvitationOnlyFee,
        [EnumDescription("Limo", "lm")]
        Limo,
        [EnumDescription("Meals", "ml")]
        Meals,
        [EnumDescription("MeiOnsiteExp", "mo", "mei onsite exp")]
        MeiOnsiteExp,
        [EnumDescription("PenaltyFee", "pf", "penalty fee")]
        PenaltyFee,
        [EnumDescription("Production", "pr")]
        Production,
        [EnumDescription("Rooms", "rm")]
        Rooms,
        [EnumDescription("Shipping", "sh")]
        Shipping,
        [EnumDescription("SpkrOther", "so", "spkr other")]
        SpkrOther,
        [EnumDescription("Taxi", "tx")]
        Taxi,
        [EnumDescription("Travel", "tr")]
        Travel
    }

    public static class ExpenseTypeExtensions
    {
        private static readonly EnumDescriptionCollection<ExpenseType> Description = new EnumDescriptionCollection<ExpenseType>();

        public static string ToDisplayNameLong(this ExpenseType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this ExpenseType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static ExpenseType ToExpenseType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
