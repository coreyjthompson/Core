using System;

namespace MEI.SPDocuments.TypeCodes
{
    public enum DocumentYear
    {
        Undefined,

        [EnumDescription("2005", "2005")]
        Year2005,

        [EnumDescription("2006", "2006")]
        Year2006,

        [EnumDescription("2007", "2007")]
        Year2007,

        [EnumDescription("2008", "2008")]
        Year2008,

        [EnumDescription("2009", "2009")]
        Year2009,

        [EnumDescription("2010", "2010")]
        Year2010,

        [EnumDescription("2011", "2011")]
        Year2011,

        [EnumDescription("2012", "2012")]
        Year2012,

        [EnumDescription("2013", "2013")]
        Year2013,

        [EnumDescription("2014", "2014")]
        Year2014,

        [EnumDescription("2015", "2015")]
        Year2015,

        [EnumDescription("2016", "2016")]
        Year2016,

        [EnumDescription("2017", "2017")]
        Year2017,

        [EnumDescription("2018", "2018")]
        Year2018,

        [EnumDescription("2019", "2019")]
        Year2019,

        [EnumDescription("2020", "2020")]
        Year2020
    }

    public static class DocumentYearExtensions
    {
        private static readonly EnumDescriptionCollection<DocumentYear> Description = new EnumDescriptionCollection<DocumentYear>();

        public static string ToDisplayNameLong(this DocumentYear code)
        {
            return Description.CodeToDisplayNameLong(code);
        }

        public static string ToDisplayNameShort(this DocumentYear code)
        {
            return Description.CodeToDisplayNameShort(code);
        }

        public static DocumentYear ToDocumentYear(this string text)
        {
            return Description.TextToCode(text);
        }

        public static string ToProgramIdYear(this DocumentYear code)
        {
            string yearName = Description.CodeToDisplayNameLong(code);
            if (yearName.Length < 2)
            {
                return string.Empty;
            }

            return "-" + yearName.Substring(yearName.Length - 2, 2);
        }

        /// <summary>
        ///     Compares two document year codes based on a operator passed as a parameter
        /// </summary>
        /// <param name="code1"></param>
        /// <param name="code2"></param>
        /// <param name="compareOperator"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool Compare(this DocumentYear code1, DocumentYear code2, string compareOperator)
        {
            Preconditions.CheckEnum("code1", code1, DocumentYear.Undefined);
            Preconditions.CheckEnum("code2", code2, DocumentYear.Undefined);

            switch (compareOperator)
            {
                case ">":
                    return Convert.ToInt32(Description.CodeToDisplayNameLong(code1))
                           > Convert.ToInt32(Description.CodeToDisplayNameLong(code2));
                case "<":
                    return Convert.ToInt32(Description.CodeToDisplayNameLong(code1))
                           < Convert.ToInt32(Description.CodeToDisplayNameLong(code2));
                case "=>":
                case ">=":
                    return Convert.ToInt32(Description.CodeToDisplayNameLong(code1))
                           >= Convert.ToInt32(Description.CodeToDisplayNameLong(code2));
                case "=<":
                case "<=":
                    return Convert.ToInt32(Description.CodeToDisplayNameLong(code1))
                           <= Convert.ToInt32(Description.CodeToDisplayNameLong(code2));
                case "=":
                case "==":
                    return Convert.ToInt32(Description.CodeToDisplayNameLong(code1))
                           == Convert.ToInt32(Description.CodeToDisplayNameLong(code2));
                default:
                    throw new ArgumentException(string.Format("invalid operator. {0}", compareOperator));
            }
        }
    }
}
