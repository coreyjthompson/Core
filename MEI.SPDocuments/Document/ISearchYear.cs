using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface ISearchYear
    {
        ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year);
    }
}
