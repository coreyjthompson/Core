using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface ISearchProgram
    {
        ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId);
    }
}
