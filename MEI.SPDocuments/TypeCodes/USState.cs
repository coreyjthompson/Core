namespace MEI.SPDocuments.TypeCodes
{
    public enum USState
    {
        Undefined,
        [EnumDescription("Alabama", "AL")]
        Alabama,
        [EnumDescription("Alaska", "AK")]
        Alaska,
        [EnumDescription("American Samoa", "AS")]
        AmericanSamoa,
        [EnumDescription("Arizona", "AZ")]
        Arizona,
        [EnumDescription("Arkansas", "AR")]
        Arkansas,
        [EnumDescription("California", "CA")]
        California,
        [EnumDescription("Colorado", "CO")]
        Colorado,
        [EnumDescription("Connecticut", "CT")]
        Connecticut,
        [EnumDescription("Delaware", "DE")]
        Delaware,
        [EnumDescription("District of Columbia", "DC")]
        DistrictOfcolumbia,
        [EnumDescription("Florida", "FL")]
        Florida,
        [EnumDescription("Georgia", "GA")]
        Georgia,
        [EnumDescription("Hawaii", "HI")]
        Hawaii,
        [EnumDescription("Idaho", "ID")]
        Idaho,
        [EnumDescription("Illinois", "IL")]
        Illinois,
        [EnumDescription("Indiana", "IN")]
        Indiana,
        [EnumDescription("Iowa", "IA")]
        Iowa,
        [EnumDescription("Kansas", "KS")]
        Kansas,
        [EnumDescription("Kentucky", "KY")]
        Kentucky,
        [EnumDescription("Louisiana", "LA")]
        Louisiana,
        [EnumDescription("Maine", "ME")]
        Maine,
        [EnumDescription("Maryland", "MD")]
        Maryland,
        [EnumDescription("Massachusetts", "MA")]
        Massachusetts,
        [EnumDescription("Michigan", "MI")]
        Michigan,
        [EnumDescription("Minnesota", "MN")]
        Minnesota,
        [EnumDescription("Mississippi", "MS")]
        Mississippi,
        [EnumDescription("Missouri", "MO")]
        Missouri,
        [EnumDescription("Montana", "MT")]
        Montana,
        [EnumDescription("Nebraska", "NE")]
        Nebraska,
        [EnumDescription("Nevada", "NV")]
        Nevada,
        [EnumDescription("New Hampshire", "NH")]
        NewHampshire,
        [EnumDescription("New Jersey", "NJ")]
        NewJersey,
        [EnumDescription("New Mexico", "NM", "Blue Sky")]
        NewMexico,
        [EnumDescription("New York", "NY")]
        NewYork,
        [EnumDescription("North Carolina", "NC")]
        NorthCarolina,
        [EnumDescription("North Dakota", "ND")]
        NorthDakota,
        [EnumDescription("Ohio", "OH")]
        Ohio,
        [EnumDescription("Oklahoma", "OK")]
        Oklahoma,
        [EnumDescription("Oregon", "OR")]
        Oregon,
        [EnumDescription("Pennsylvania", "PA")]
        Pennsylvania,
        [EnumDescription("Rhode Island", "RI")]
        RodeIsland,
        [EnumDescription("South Carolina", "SC")]
        SouthCarolina,
        [EnumDescription("South Dakota", "SD")]
        SouthDakota,
        [EnumDescription("Tennessee", "TN")]
        Tennessee,
        [EnumDescription("Texas", "TX")]
        Texas,
        [EnumDescription("Utah", "UT")]
        Utah,
        [EnumDescription("Vermont", "VT")]
        Vermont,
        [EnumDescription("Virginia", "VA")]
        Virginia,
        [EnumDescription("Washington", "WA")]
        Washington,
        [EnumDescription("West Virginia", "WV")]
        WestVirginia,
        [EnumDescription("Wisconsin", "WI")]
        Wisconsin,
        [EnumDescription("Wyoming", "WY")]
        Wyoming
    }

    public static class USStateExtensions
    {
        private static readonly EnumDescriptionCollection<USState> Description = new EnumDescriptionCollection<USState>();

        public static string ToDisplayNameLong(this USState typeCode)
        {
            return Description.CodeToDisplayNameLong(typeCode);
        }

        public static string ToDisplayNameShort(this USState typeCode)
        {
            return Description.CodeToDisplayNameShort(typeCode);
        }

        public static USState ToUSState(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
