using System;
using System.Collections.ObjectModel;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    internal class SearchExpressionCollection
        : Collection<SearchExpression>
    {
        private readonly IDocument _document;

        public SearchExpressionCollection(IDocument document)
        {
            _document = document;
        }

        public bool IsValid
        {
            get
            {
                if (_document == null)
                {
                    return false;
                }

                foreach (SearchExpression item in Items)
                {
                    if (!item.IsValid)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        protected override void InsertItem(int index, SearchExpression item)
        {
            item.Field = _document.SPFields[item.EnumValue];

            base.InsertItem(index, item);
        }

        public void Add(SPFieldNames enumValue, CamlComparison comparisonType, string value)
        {
            Add(new SearchExpression(_document, enumValue, comparisonType, value));
        }

        public void Add(SPFieldNames enumValue, CamlComparison comparisonType, int? value)
        {
            Add(new SearchExpression(_document, enumValue, comparisonType, value));
        }

        public void Add(SPFieldNames enumValue, CamlComparison comparisonType, long? value)
        {
            Add(new SearchExpression(_document, enumValue, comparisonType, value));
        }

        public void Add(SPFieldNames enumValue, CamlComparison comparisonType, DateTime? value)
        {
            Add(new SearchExpression(_document, enumValue, comparisonType, value));
        }

        public override string ToString()
        {
            return string.Format("[Count={0}]", Items.Count);
        }
    }
}
