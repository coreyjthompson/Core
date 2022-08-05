using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface ISearchSpeaker
    {
        ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter);
    }
}
