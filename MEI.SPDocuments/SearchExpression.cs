using System;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    internal class SearchExpression
    {
        private readonly IDocument _document;

        internal SearchExpression(IDocument document, SPFieldNames enumValue, CamlComparison comparison, string value)
        {
            _document = document;
            EnumValue = enumValue;
            ExpressionValue = value;
            Comparison = comparison;
        }

        internal SearchExpression(IDocument document, SPFieldNames enumValue, CamlComparison comparison, int? value)
        {
            _document = document;
            EnumValue = enumValue;
            Comparison = comparison;

            if (value.HasValue)
            {
                ExpressionValue = value.Value.ToString();
            }
        }

        internal SearchExpression(IDocument document, SPFieldNames enumValue, CamlComparison comparison, long? value)
        {
            _document = document;
            EnumValue = enumValue;
            Comparison = comparison;

            if (value.HasValue)
            {
                ExpressionValue = value.Value.ToString();
            }
        }

        internal SearchExpression(IDocument document, SPFieldNames enumValue, CamlComparison comparison, DateTime? value)
        {
            _document = document;
            EnumValue = enumValue;
            Comparison = comparison;

            if (value.HasValue)
            {
                ExpressionValue = value.Value.ToString("yyyy-MM-ddThh:mm:ssZ");
            }
        }

        public CamlComparison Comparison { get; }

        public SPFieldNames EnumValue { get; }

        public string ExpressionValue { get; }

        public SPField Field { get; internal set; }

        public bool IsValid
        {
            get
            {
                if (Comparison == CamlComparison.None)
                {
                    return false;
                }

                if (EnumValue <= 0)
                {
                    return false;
                }

                if (Field == null)
                {
                    return false;
                }

                return true;
            }
        }

        public string MakeSearchXml()
        {
            switch (Comparison)
            {
                case CamlComparison.IsNull:
                case CamlComparison.IsNotNull:
                    return string.Format("<{0}><FieldRef Name='{1}' /></{0}>",
                        Comparison.ToDisplayNameLong(),
                        Field.InternalName);
                default:
                    return string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'>{3}</Value></{0}>",
                        Comparison.ToDisplayNameLong(),
                        Field.InternalName,
                        Field.FieldType.ToDisplayNameLong(),
                        ExpressionValue);
            }
        }

        public IAndingSearchExpressionGroup And(SPFieldNames enumValue, CamlComparison comparison, int? value)
        {
            var searchExpressionGroup = new SearchExpressionGroup(_document);
            searchExpressionGroup.AddExpression(EnumValue, Comparison, value);

            return searchExpressionGroup.And(enumValue, comparison, value);
        }

        public IOringSearchExpressionGroup Or(SPFieldNames enumValue, CamlComparison comparison, int? value)
        {
            var searchExpressionGroup = new SearchExpressionGroup(_document);
            searchExpressionGroup.AddExpression(EnumValue, Comparison, value);

            return searchExpressionGroup.Or(enumValue, comparison, value);
        }

        public override string ToString()
        {
            return string.Format("[FieldName={0}, FieldValue={1}, CamlComparisonType={2}]",
                EnumValue,
                ExpressionValue,
                Comparison.ToDisplayNameLong());
        }
    }
}
