namespace MEI.SPDocuments.TypeCodes
{
    public enum SPActionStatus
    {
        None,
        [EnumDescription("Success", "Success")]
        Success,
        [EnumDescription("Failure", "Failure")]
        Failure,
        [EnumDescription("InvalidFileName", "InvalidFileName", "invalid file name", "invalid filename")]
        InvalidFileName,
        [EnumDescription("UnknownError", "UnknownError", "unknown error")]
        UnknownError
    }

    public static class ActionStatusExtensions
    {
        private static readonly EnumDescriptionCollection<SPActionStatus> Description = new EnumDescriptionCollection<SPActionStatus>();

        public static string ToDisplayNameLong(this SPActionStatus code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this SPActionStatus code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static SPActionStatus ToActionStatus(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
