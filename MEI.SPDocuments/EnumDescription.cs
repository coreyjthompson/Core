using System.Collections.Generic;
using System.Linq;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Represents the description of an Enum.
    /// </summary>
    /// <typeparam name="T">The data type of the Enum.</typeparam>
    /// <remarks>
    ///     Used in conversion from a string to an Enum member and vice versa.
    /// </remarks>
    public class EnumDescription<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDescription{T}" /> class.
        /// </summary>
        /// <param name="enumMember">The enum member.</param>
        /// <param name="displayNameLong">The display name of the enum member in long form.</param>
        /// <param name="displayNameShort">
        ///     The display name of the enum member in short form. Generally is an acronym of the enum
        ///     member.
        /// </param>
        /// <param name="enumMemberResolutions">All the possible string representations of the enum member.</param>
        public EnumDescription(T enumMember, string displayNameLong, string displayNameShort, params string[] enumMemberResolutions)
        {
            EnumMember = enumMember;
            DisplayNameLong = displayNameLong;
            DisplayNameShort = displayNameShort;
            EnumMemberResolutions = enumMemberResolutions.ToList();

            CleanNameResolutions();
        }

        /// <summary>
        ///     Gets or sets the enum member.
        /// </summary>
        public T EnumMember { get; }

        /// <summary>
        ///     Gets or sets all the string representations of the enum member.
        /// </summary>
        public List<string> EnumMemberResolutions { get; }

        /// <summary>
        ///     Gets or sets the display name of the enum member in long form.
        /// </summary>
        public string DisplayNameLong { get; }

        /// <summary>
        ///     Gets or sets the display name of the enum member in short form. Generally is an acronym of the enum member.
        /// </summary>
        public string DisplayNameShort { get; }

        private void CleanNameResolutions()
        {
            for (var i = 0; i <= EnumMemberResolutions.Count - 1; i++)
            {
                EnumMemberResolutions[i] = EnumMemberResolutions[i].ToLower();
            }

            if (!EnumMemberResolutions.Contains(DisplayNameLong.ToLower()))
            {
                EnumMemberResolutions.Add(DisplayNameLong.ToLower());
            }

            if (!EnumMemberResolutions.Contains(DisplayNameShort.ToLower()))
            {
                EnumMemberResolutions.Add(DisplayNameShort.ToLower());
            }
        }
    }
}
