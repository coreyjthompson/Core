namespace MEI.SPDocuments.TypeCodes
{
    public enum EPassStatus
    {
        Undefined,
        [EnumDescription("Approved", "A")]
        Approved,
        [EnumDescription("Destroy", "D")]
        Destroy
    }

    public static class EPassStatusExtensions
    {
        private static readonly EnumDescriptionCollection<EPassStatus> Description = new EnumDescriptionCollection<EPassStatus>();

        public static string ToDisplayNameLong(this EPassStatus typeCode)
        {
            return Description.CodeToDisplayNameLong(typeCode);
        }

        public static string ToDisplayNameShort(this EPassStatus typeCode)
        {
            return Description.CodeToDisplayNameShort(typeCode);
        }

        public static EPassStatus ToEPassStatus(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
