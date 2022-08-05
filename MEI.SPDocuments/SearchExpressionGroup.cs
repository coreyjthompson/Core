using System;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    internal class SearchExpressionGroup
        : ISearchExpressionGroup, ICompleteSearchExpressionGroup
    {
        private readonly IDocument _document;

        internal SearchExpressionGroup(IDocument document, SPFieldNames enumValue, CamlComparison comparison, string value)
            : this(document)
        {
            AddExpression(enumValue, comparison, value);
        }

        internal SearchExpressionGroup(IDocument document, SPFieldNames enumValue, CamlComparison comparison, int? value)
            : this(document)
        {
            AddExpression(enumValue, comparison, value);
        }

        internal SearchExpressionGroup(IDocument document, SPFieldNames enumValue, CamlComparison comparison, long? value)
            : this(document)
        {
            AddExpression(enumValue, comparison, value);
        }

        internal SearchExpressionGroup(IDocument document, SPFieldNames enumValue, CamlComparison comparison, DateTime? value)
            : this(document)
        {
            AddExpression(enumValue, comparison, value);
        }

        internal SearchExpressionGroup(IDocument document)
        {
            _document = document;

            DocumentType = document.DocumentType;
            SearchExpressions = new SearchExpressionCollection(document);
        }

        public SearchBooleanLogic BooleanLogicType { get; set; }

        private SearchExpressionGroup InnerSearchExpressionGroup { get; set; }

        private SearchExpressionGroup OuterSearchExpressionGroup { get; set; }

        public SearchBooleanLogic AttachLogicType { get; private set; }

        public SPDocumentType DocumentType { get; }

        public SearchExpressionCollection SearchExpressions { get; }

        public bool IsValid
        {
            get
            {
                if (DocumentType == SPDocumentType.None)
                {
                    return false;
                }

                if (SearchExpressions == null)
                {
                    return false;
                }

                if ((SearchExpressions != null) && (SearchExpressions.Count == 0))
                {
                    return false;
                }

                if (SearchExpressions.Count > 1 && (BooleanLogicType == SearchBooleanLogic.None))
                {
                    return false;
                }

                if (!SearchExpressions.IsValid)
                {
                    return false;
                }

                if (InnerSearchExpressionGroup != null)
                {
                    return InnerSearchExpressionGroup.IsValid;
                }

                return true;
            }
        }

        private void AttachExpressionGroup(SearchExpressionGroup item, SearchBooleanLogic groupAttachLogicType)
        {
            Preconditions.CheckNotNull("item", item);
            Preconditions.CheckEnum("groupAttachLogic", groupAttachLogicType, SearchBooleanLogic.None);

            InnerSearchExpressionGroup = item;
            AttachLogicType = groupAttachLogicType;

            item.OuterSearchExpressionGroup = this;
        }

        internal void AddExpression(SPFieldNames enumValue, CamlComparison comparison, string value)
        {
            SearchExpressions.Add(enumValue, comparison, value);
        }

        internal void AddExpression(SPFieldNames enumValue, CamlComparison comparison, int? value)
        {
            SearchExpressions.Add(enumValue, comparison, value);
        }

        internal void AddExpression(SPFieldNames enumValue, CamlComparison comparison, long? value)
        {
            SearchExpressions.Add(enumValue, comparison, value);
        }

        internal void AddExpression(SPFieldNames enumValue, CamlComparison comparison, DateTime? value)
        {
            SearchExpressions.Add(enumValue, comparison, value);
        }

        public string MakeQuery()
        {
            if (!IsValid)
            {
                return string.Empty;
            }

            string searchXml = string.Empty;
            var count = 0;

            foreach (SearchExpression item in SearchExpressions)
            {
                string temp = item.MakeSearchXml();

                count += 1;

                if (count > 1)
                {
                    searchXml = string.Format("<{0}>{1}{2}</{0}>",
                        BooleanLogicType.ToDisplayNameLong(),
                        searchXml,
                        temp);
                }
                else
                {
                    searchXml += temp;
                }
            }

            if (InnerSearchExpressionGroup != null)
            {
                searchXml = string.Format("<{0}>{1}{2}</{0}>",
                    AttachLogicType.ToDisplayNameLong(),
                    searchXml,
                    InnerSearchExpressionGroup.MakeQuery()
                    );
            }

            return OuterSearchExpressionGroup == null ? string.Format("<Where>{0}</Where>", searchXml) : searchXml;
        }

        public override string ToString()
        {
            return string.Format("[SearchExpressionCount={0}, BooleanLogic={1}, DocumentType={2}, AttachLogic={3}]",
                SearchExpressions.Count,
                BooleanLogicType.ToDisplayNameLong(),
                DocumentType.ToString(),
                AttachLogicType.ToDisplayNameLong());
        }

        public IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, string value)
        {
            BooleanLogicType = SearchBooleanLogic.And;
            AddExpression(field, comparison, value);

            return this;
        }

        public IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, int? value)
        {
            BooleanLogicType = SearchBooleanLogic.And;
            AddExpression(field, comparison, value);

            return this;
        }

        public IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, long? value)
        {
            BooleanLogicType = SearchBooleanLogic.And;
            AddExpression(field, comparison, value);

            return this;
        }

        public IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, DateTime? value)
        {
            BooleanLogicType = SearchBooleanLogic.And;
            AddExpression(field, comparison, value);

            return this;
        }

        public ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, string value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.Or);

            return newGroup;
        }

        public ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, int? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.Or);

            return newGroup;
        }

        public ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, long? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.Or);

            return newGroup;
        }

        public ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, DateTime? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.Or);

            return newGroup;
        }

        public ISearchExpressionGroup AttachGroupWithAnd(ISearchExpressionGroup newGroup)
        {
            AttachExpressionGroup((SearchExpressionGroup)newGroup, SearchBooleanLogic.And);

            return newGroup;
        }

        public ISearchExpressionGroup OrWithNewGroup(ISearchExpressionGroup newGroup)
        {
            AttachExpressionGroup((SearchExpressionGroup)newGroup, SearchBooleanLogic.Or);

            return newGroup;
        }

        public ICompleteSearchExpressionGroup Build()
        {
            return FindParent(this);
        }

        public IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, string value)
        {
            BooleanLogicType = SearchBooleanLogic.Or;
            AddExpression(field, comparison, value);

            return this;
        }

        public IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, int? value)
        {
            BooleanLogicType = SearchBooleanLogic.Or;
            AddExpression(field, comparison, value);

            return this;
        }

        public IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, long? value)
        {
            BooleanLogicType = SearchBooleanLogic.Or;
            AddExpression(field, comparison, value);

            return this;
        }

        public IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, DateTime? value)
        {
            BooleanLogicType = SearchBooleanLogic.Or;
            AddExpression(field, comparison, value);

            return this;
        }

        public ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, string value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.And);

            return newGroup;
        }

        public ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, int? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.And);

            return newGroup;
        }

        public ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, long? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.And);

            return newGroup;
        }

        public ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, DateTime? value)
        {
            var newGroup = new SearchExpressionGroup(_document, field, comparison, value);

            AttachExpressionGroup(newGroup, SearchBooleanLogic.And);

            return newGroup;
        }

        private SearchExpressionGroup FindParent(SearchExpressionGroup item)
        {
            if (item.OuterSearchExpressionGroup == null)
            {
                return item;
            }

            return FindParent(item.OuterSearchExpressionGroup);
        }
    }

    public interface ICompleteSearchExpressionGroup
    {
        string MakeQuery();
    }

    public interface ISearchExpressionGroup
        : IAndingSearchExpressionGroup, IOringSearchExpressionGroup
    {
        new ICompleteSearchExpressionGroup Build();

        bool IsValid { get; }
    }

    public interface IAndingSearchExpressionGroup
    {
        IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, string value);

        IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, int? value);

        IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, long? value);

        IAndingSearchExpressionGroup And(SPFieldNames field, CamlComparison comparison, DateTime? value);

        ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, string value);

        ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, int? value);

        ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, long? value);

        ISearchExpressionGroup OrWithNewGroup(SPFieldNames field, CamlComparison comparison, DateTime? value);

        ISearchExpressionGroup AttachGroupWithAnd(ISearchExpressionGroup newGroup);

        ICompleteSearchExpressionGroup Build();
    }

    public interface IOringSearchExpressionGroup
    {
        IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, string value);

        IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, int? value);

        IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, long? value);

        IOringSearchExpressionGroup Or(SPFieldNames field, CamlComparison comparison, DateTime? value);

        ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, string value);

        ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, int? value);

        ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, long? value);

        ISearchExpressionGroup AndWithNewGroup(SPFieldNames field, CamlComparison comparison, DateTime? value);

        ISearchExpressionGroup OrWithNewGroup(ISearchExpressionGroup newGroup);

        ICompleteSearchExpressionGroup Build();
    }
}
