using System;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Represents a field in a SharePoint document library and its associated attributes.
    /// </summary>
    public class SPField
        : IEquatable<SPField>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SPField" /> class.
        /// </summary>
        /// <param name="enumValue">The enum member which corresponds to this instance.</param>
        /// <param name="displayName">The display name of the field.</param>
        /// <param name="internalName">The internal name of the field.</param>
        /// <param name="fieldType">Data type of the field.</param>
        /// <param name="isUserDefined">if set to <c>true</c> the field is user defined; else it is not.</param>
        internal SPField(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType, bool isUserDefined)
            : this(enumValue, displayName, internalName, fieldType, isUserDefined, null)
        {}

        internal SPField(SPFieldNames enumValue, string displayName, string internalName, SPFieldType fieldType, bool isUserDefined, int? getFileIndex)
        {
            EnumValue = enumValue;
            DisplayName = displayName;
            InternalName = internalName;
            FieldType = fieldType;
            IsUserDefined = isUserDefined;
            GetFileIndex = getFileIndex;
        }

        /// <summary>
        ///     Gets the display name of the field.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///     Gets the data type of the field.
        /// </summary>
        public SPFieldType FieldType { get; }

        /// <summary>
        ///     Gets the internal name of the field.
        /// </summary>
        public string InternalName { get; }

        /// <summary>
        ///     Gets a value indicating whether the field is user defined.
        /// </summary>
        public bool IsUserDefined { get; }

        /// <summary>
        ///     Gets the enum member which corresponds to this instance.
        /// </summary>
        public SPFieldNames EnumValue { get; }

        public int? GetFileIndex { get; }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        public bool Equals(SPField other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.DisplayName, DisplayName) && Equals(other.FieldType, FieldType)
                                                                 && Equals(other.InternalName, InternalName)
                                                                 && other.IsUserDefined.Equals(IsUserDefined)
                                                                 && (other.EnumValue == EnumValue);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[DisplayName={0}, InternalName={1}, FieldType={2}, IsUserDefined={3}, EnumName={4}]",
                DisplayName,
                InternalName,
                FieldType.ToDisplayNameLong(),
                IsUserDefined,
                EnumValue);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />;
        ///     otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is null.</exception>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!ReferenceEquals(obj.GetType(), typeof(SPField)))
            {
                return false;
            }

            return Equals((SPField)obj);
        }

        /// <summary>
        ///     Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            long hashCode = 0;
            if (DisplayName != null)
            {
                hashCode = Convert.ToInt32(((hashCode * 397) ^ DisplayName.GetHashCode()) % int.MaxValue);
            }

            hashCode = Convert.ToInt32((hashCode * 397) ^ (FieldType.GetHashCode() % int.MaxValue));
            if (InternalName != null)
            {
                hashCode = Convert.ToInt32(((hashCode * 397) ^ InternalName.GetHashCode()) % int.MaxValue);
            }

            hashCode = Convert.ToInt32((hashCode * 397) ^ (IsUserDefined.GetHashCode() % int.MaxValue));
            hashCode = Convert.ToInt32((hashCode * 397) ^ ((int)EnumValue % int.MaxValue));
            return Convert.ToInt32(hashCode % int.MaxValue);
        }

        public static bool operator ==(SPField left, SPField right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SPField left, SPField right)
        {
            return !Equals(left, right);
        }
    }
}
