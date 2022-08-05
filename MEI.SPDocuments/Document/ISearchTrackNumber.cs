using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface ISearchTrackNumber
    {
        ISearchExpressionGroup GetSearchExpressionGroupByTrackNumber(Company company, DocumentYear year, int trackNumber);

        ISearchExpressionGroup GetSearchExpressionGroupByHcpName(Company company,
                                                                 DocumentYear year,
                                                                 string hcpFirstName,
                                                                 string hcpLastName);
    }
}
