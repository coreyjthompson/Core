using System;
using System.Collections.Generic;
using System.Linq;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     An attribute used to build an <see cref="EnumDescription{T}" />.
    /// </summary>
    internal class EnumDescriptionAttribute
        : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDescriptionAttribute" /> class.
        /// </summary>
        /// <param name="displayNameLong">The display name of the enum member in long form.</param>
        /// <param name="displayNameShort">
        ///     The display name of the enum member in short form. Generally is an acronym of the enum
        ///     Member.
        /// </param>
        /// <param name="enumMemberResolutions">All of the string representations of the enum member.</param>
        public EnumDescriptionAttribute(string displayNameLong, string displayNameShort, params string[] enumMemberResolutions)
        {
            DisplayNameLong = displayNameLong;
            DisplayNameShort = displayNameShort;
            EnumMemberResolutions = enumMemberResolutions.ToList();
        }

        /// <summary>
        ///     Gets or sets the display name of the enum member in long form.
        /// </summary>
        internal string DisplayNameLong { get; }

        /// <summary>
        ///     Gets or sets the display name of the enum member in short form. Generally is an acronym of the enum member.
        /// </summary>
        internal string DisplayNameShort { get; }

        /// <summary>
        ///     Gets or sets all the string representations of the enum member.
        /// </summary>
        internal List<string> EnumMemberResolutions { get; }
    }
}
