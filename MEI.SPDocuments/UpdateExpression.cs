using System;

using MEI.SPDocuments.Document;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Represents an expression used when updating documents.
    /// </summary>
    public class UpdateExpression
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateExpression" /> class.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="fieldValue">The value.</param>
        public UpdateExpression(SPFieldNames enumValue, string fieldValue)
        {
            EnumValue = enumValue;
            FieldValue = fieldValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateExpression" /> class.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="fieldValue">The value.</param>
        public UpdateExpression(SPFieldNames enumValue, int? fieldValue)
            : this(enumValue, fieldValue.HasValue ? fieldValue.Value.ToString() : string.Empty)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateExpression" /> class.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="fieldValue">The value.</param>
        public UpdateExpression(SPFieldNames enumValue, long? fieldValue)
            : this(enumValue, fieldValue.HasValue ? fieldValue.Value.ToString() : string.Empty)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateExpression" /> class.
        /// </summary>
        /// <param name="enumValue">The enum member.</param>
        /// <param name="fieldValue">The value.</param>
        public UpdateExpression(SPFieldNames enumValue, DateTime? fieldValue)
            : this(enumValue, fieldValue.HasValue ? fieldValue.Value.ToString("yyyy-MM-ddThh:mm:ssZ") : string.Empty)
        { }

        /// <summary>
        ///     Gets or sets the enum member.
        /// </summary>
        public SPFieldNames EnumValue { get; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        public string FieldValue { get; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[FieldName={0}, FieldValue={1}]", EnumValue, FieldValue);
        }
    }
}
