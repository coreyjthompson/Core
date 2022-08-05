using System;
using System.Text;

namespace MEI.SPDocuments.TypeCodes
{
    [Flags]
    public enum SPDocumentPrivileges
    {
        None = 0,
        [EnumDescription("Upload", "u")]
        Upload = 1,
        [EnumDescription("Search", "s")]
        Search = 1 << 1,
        [EnumDescription("Rename", "r")]
        Rename = 1 << 2,
        [EnumDescription("Update", "t")]
        Update = 1 << 3,
        [EnumDescription("Delete", "d")]
        Delete = 1 << 4,
        [EnumDescription("ViewOriginal", "o", "view original")]
        ViewOriginal = 1 << 5,
        [EnumDescription("ViewWatermarked", "w", "view watermarked")]
        ViewWatermarked = 1 << 6,
        [EnumDescription("SearchVersions", "v", "search versions")]
        SearchVersions = 1 << 7,
        [EnumDescription("Save", "p")]
        Save = 1 << 8
    }

    public static class DocumentPrivilegesCodeExtensions
    {
        private static readonly EnumDescriptionCollection<SPDocumentPrivileges> Description = new EnumDescriptionCollection<SPDocumentPrivileges>();

        public static SPDocumentPrivileges ToDocumentPrivileges(this string text)
        {
            return Description.TextToCode(text);
        }

        public static string ToNameText(this SPDocumentPrivileges code)
        {
            string text = string.Empty;
            int count = 0;

            foreach (SPDocumentPrivileges item in Enum.GetValues(typeof(SPDocumentPrivileges)))
            {
                if (item == SPDocumentPrivileges.None)
                {
                    continue;
                }

                if (code.Has(item))
                {
                    if (count > 0)
                    {
                        text += "|";
                    }

                    text += Description.CodeToDisplayNameLong(item);
                    count += 1;
                }
            }

            return text;
        }

        public static string ToAbbreviatedText(this SPDocumentPrivileges code)
        {
            var sb = new StringBuilder();

            foreach (SPDocumentPrivileges permission in Enum.GetValues(typeof(SPDocumentPrivileges)))
            {
                if (code.Has(permission))
                {
                    sb.Append(Description.CodeToDisplayNameShort(permission));
                }
            }

            return sb.ToString();
        }
    }
}
