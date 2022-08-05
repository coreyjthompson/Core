namespace MEI.SPDocuments.TypeCodes
{
    public enum MenuType
    {
        Undefined,
        [EnumDescription("Breakfast", "B")]
        Breakfast,
        [EnumDescription("Dessert", "T")]
        Dessert,
        [EnumDescription("Dinner", "D")]
        Dinner,
        [EnumDescription("Drink", "K")]
        Drink,
        [EnumDescription("Holiday", "H")]
        Holiday,
        [EnumDescription("Lunch", "L")]
        Lunch
    }

    public static class MenuTypeExtensions
    {
        private static readonly EnumDescriptionCollection<MenuType> Description = new EnumDescriptionCollection<MenuType>();

        public static string ToDisplayNameLong(this MenuType code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this MenuType code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static MenuType ToMenuType(this string text)
        {
            return Description.TextToCode(text);
        }
    }
}
