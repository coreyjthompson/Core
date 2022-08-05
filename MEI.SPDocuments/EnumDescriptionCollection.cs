using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     A collection of <see cref="EnumDescription{T}" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumDescriptionCollection<T>
        : Collection<EnumDescription<T>>
        where T : struct, IConvertible
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDescriptionCollection{T}" /> class.
        /// </summary>
        /// <remarks>
        ///     Builds itself based on Enum data type it was initialized with.
        /// </remarks>
        public EnumDescriptionCollection()
        {
            BuildEnumDescriptionCollection();
        }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///     The element at the specified index.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="enumMember" /> is not found in collection
        /// </exception>
        public EnumDescription<T> this[T enumMember]
        {
            get => Items[GetEnumMemberIndex(enumMember)];
            set => Items[GetEnumMemberIndex(enumMember)] = value;
        }

        private void BuildEnumDescriptionCollection()
        {
            Preconditions.CheckArgument("T", typeof(T).IsEnum, Resources.Default.T_must_be_an_enumerated_type);

            foreach (FieldInfo enumMember in typeof(T).GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                foreach (Attribute enumMemberAttribute in enumMember.GetCustomAttributes(true))
                {
                    if (enumMemberAttribute is EnumDescriptionAttribute att)
                    {
                        Add((T)enumMember.GetValue(null), att.DisplayNameLong, att.DisplayNameShort, att.EnumMemberResolutions.ToArray());
                    }
                }
            }
        }

        /// <summary>
        ///     Determines whether the collection contains an item with the specified <paramref name="enumMember" />.
        /// </summary>
        /// <param name="enumMember">The enum member.</param>
        /// <returns>
        ///     <c>true</c> if the collection contains an item with the specified <paramref name="enumMember" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool ContainsTypeValue(T enumMember)
        {
            return Items.Any(thi => thi.EnumMember.Equals(enumMember));
        }

        /// <summary>
        ///     Determines whether the collection contains an item with the specified <paramref name="displayNameLong" />.
        /// </summary>
        /// <param name="displayNameLong">The display name of the enum member in long form.</param>
        /// <returns>
        ///     <c>true</c> if the collection contains an item with the specified <paramref name="displayNameLong" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool ContainsDisplayNameLong(string displayNameLong)
        {
            return Items.Any(thi => thi.DisplayNameLong.ToLower() == displayNameLong.ToLower());
        }

        /// <summary>
        ///     Determines whether the collection contains an item with the specified <paramref name="displayNameShort" />.
        /// </summary>
        /// <param name="displayNameShort">The display name of the enum member in short form.</param>
        /// <returns>
        ///     <c>true</c> if collection contains an item with the specified <paramref name="displayNameShort" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool ContainsDisplayNameShort(string displayNameShort)
        {
            return Items.Any(thi => thi.DisplayNameShort.ToLower() == displayNameShort.ToLower());
        }

        /// <summary>
        ///     Gets the index of the specified <paramref name="enumMember" />.
        /// </summary>
        /// <param name="enumMember">The enum member.</param>
        /// <returns>An <see cref="Int32" /> equal to the index of the specified <paramref name="enumMember" />.</returns>
        private int GetEnumMemberIndex(T enumMember)
        {
            for (int i = 0; i <= Items.Count - 1; i++)
            {
                if (Items[i].EnumMember.Equals(enumMember))
                {
                    return i;
                }
            }

            throw new ArgumentException(Resources.Default.enumMember_does_not_exist_in_collection);
        }

        /// <summary>
        ///     Inserts the specified <paramref name="item" /> into the collection at the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index in the collection to insert the item.</param>
        /// <param name="item">The item to insert into the collection.</param>
        protected override void InsertItem(int index, EnumDescription<T> item)
        {
            Preconditions.CheckArgument("item", !ContainsTypeValue(item.EnumMember), Resources.Default.enumMember_already_exists_in_collection);
            Preconditions.CheckArgument("item",
                !ContainsDisplayNameLong(item.DisplayNameLong),
                Resources.Default.displayNameLong_already_exists_in_collection);
            Preconditions.CheckArgument("item",
                !ContainsDisplayNameShort(item.DisplayNameShort),
                Resources.Default.displayNameShort_already_exists_in_collection);

            base.InsertItem(index, item);
        }

        /// <summary>
        ///     Adds an <see cref="EnumDescription{T}" /> to the collection represented by the given values.
        /// </summary>
        /// <param name="enumMember">The enum member.</param>
        /// <param name="displayNameLong">The display name of the enum member in long form.</param>
        /// <param name="displayNameShort">The display name of the enum member in short form.</param>
        /// <param name="enumMemberResolutions">All the string representations of the enum member.</param>
        public void Add(T enumMember, string displayNameLong, string displayNameShort, params string[] enumMemberResolutions)
        {
            Add(new EnumDescription<T>(enumMember, displayNameLong, displayNameShort, enumMemberResolutions));
        }

        /// <summary>
        ///     Converts an enum member to its respective display name in long form.
        /// </summary>
        /// <param name="code">The enum member to convert.</param>
        /// <returns>A <see cref="string" /> representing the enum member's display name in long form.</returns>
        public string CodeToDisplayNameLong(T code)
        {
            if (!ContainsTypeValue(code))
            {
                return string.Empty;
            }

            return this[code].DisplayNameLong;
        }

        /// <summary>
        ///     Converts an enum member to its respective display name in short form.
        /// </summary>
        /// <param name="code">The enum member to convert.</param>
        /// <returns>A <see cref="string" /> representing the enum member's display name in short form.</returns>
        public string CodeToDisplayNameShort(T code)
        {
            if (!ContainsTypeValue(code))
            {
                return string.Empty;
            }

            return this[code].DisplayNameShort;
        }

        /// <summary>
        ///     Converts the specified <paramref name="text" /> to its respective enum member.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>An enum member.</returns>
        /// <remarks>Used the EnumMemberResolutions for the translation.</remarks>
        public T TextToCode(string text)
        {
            foreach (EnumDescription<T> thi in Items)
            {
                if (thi.EnumMemberResolutions.Contains(text.ToLower()))
                {
                    return thi.EnumMember;
                }
            }

            return default;
        }
    }
}
