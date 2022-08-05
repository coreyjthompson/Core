using System;
using System.Collections.ObjectModel;
using System.Linq;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     A collection of <see cref="SPField" />.
    /// </summary>
    public class SPFieldCollection
        : Collection<SPField>
    {
        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///     The element at the specified index.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     item with the specified <paramref name="enumValue" /> was not found in collection
        /// </exception>
        public SPField this[SPFieldNames enumValue] => Items.FirstOrDefault(x => x.EnumValue == enumValue);

        /// <summary>
        ///     Inserts the specified <paramref name="item" /> into the collection at the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The item to insert into the collection.</param>
        protected override void InsertItem(int index, SPField item)
        {
            Preconditions.CheckNotNullOrEmpty("item.InternalName", item.InternalName);

            if (Convert.ToInt32(item.EnumValue) == 0)
            {
                throw new ApplicationException(Resources.Default.can_not_add_undefined_enumName);
            }

            if (Items.Any(x => x.InternalName == item.InternalName))
            {
                throw new ApplicationException(string.Format(Resources.Default.internalName_already_exists_in_collection__0, item.InternalName));
            }

            if (Items.Any(x => x.EnumValue == item.EnumValue))
            {
                throw new ApplicationException(string.Format(Resources.Default.enumName_already_exists_in_collection__0, item.EnumValue));
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        ///     Adds an item represented by the specified fields to the collection.
        /// </summary>
        /// <param name="enumValue">NThe enum member.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="fieldType">Data type of the field.</param>
        public void Add(SPFieldNames enumValue, string displayName, SPFieldType fieldType)
        {
            Add(enumValue, displayName, fieldType, false);
        }

        public void Add(SPFieldNames enumValue, string displayName, SPFieldType fieldType, int getFileIndex)
        {
            Add(enumValue, displayName, fieldType, false, getFileIndex);
        }

        /// <summary>
        ///     Adds an item represented by the specified fields to the collection.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="fieldType">Data type of the field.</param>
        /// <param name="isUserDefined">if set to <c>true</c> the field is user defined; else it is not user defined.</param>
        public void Add(SPFieldNames enumValue, string displayName, SPFieldType fieldType, bool isUserDefined)
        {
            Add(enumValue, displayName, displayName, fieldType, isUserDefined);
        }

        public void Add(SPFieldNames enumValue, string displayName, SPFieldType fieldType, bool isUserDefined, int getFileIndex)
        {
            Add(enumValue, displayName, displayName, fieldType, isUserDefined, getFileIndex);
        }

        /// <summary>
        ///     Adds an item represented by the specified fields to the collection.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="internalName">The internal name.</param>
        /// <param name="fieldType">Data type of the field.</param>
        public void Add(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType)
        {
            Add(enumValue, displayName, internalName, fieldType, false);
        }

        public void Add(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType, int getFileIndex)
        {
            Add(enumValue, displayName, internalName, fieldType, false, getFileIndex);
        }

        /// <summary>
        ///     Adds an item represented by the specified fields to the collection.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="internalName">The internal name.</param>
        /// <param name="fieldType">Data type of the field.</param>
        /// <param name="isUserDefined">if set to <c>true</c> the field is user defined; else it is not user defined.</param>
        public void Add(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType, bool isUserDefined)
        {
            Add(new SPField(enumValue, displayName, internalName, fieldType, isUserDefined));
        }

        public void Add(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType, bool isUserDefined, int getFileIndex)
        {
            Add(new SPField(enumValue, displayName, internalName, fieldType, isUserDefined, getFileIndex));
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[Count={0}]", Items.Count);
        }
    }
}
