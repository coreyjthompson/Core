using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface ISearchVendor
    {
        ISearchExpressionGroup GetSearchExpressionGroupByVendor(Company company, DocumentYear year, int vendorId);
    }
}
