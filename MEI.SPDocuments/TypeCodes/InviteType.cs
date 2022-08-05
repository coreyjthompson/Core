namespace MEI.SPDocuments.TypeCodes
{
    public enum InviteType
    {
        Undefined,
        [EnumDescription("E-Version", "E")]
        EVersion,
        [EnumDescription("Print Version", "P")]
        PrintVersion,
        [EnumDescription("Rep Version", "R")]
        RepVersion,
        [EnumDescription("E-Blast Top", "ET")]
        EBlastTop,
        [EnumDescription("E-Blast Bottom", "EB")]
        EBlastBottom,
        [EnumDescription("None", "N")]
        None,
        [EnumDescription("Sample", "S")]
        Sample,
        [EnumDescription("Remote Electronic ", "ER")]
        RemoteElectronic,
        [EnumDescription("Remote Print", "PR")]
        RemotePrint
    }

    public static class InviteTypeExtensions
    {
        private static readonly EnumDescriptionCollection<InviteType> Description = new EnumDescriptionCollection<InviteType>();

        public static string ToDisplayNameLong(this InviteType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this InviteType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static InviteType ToInviteType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
