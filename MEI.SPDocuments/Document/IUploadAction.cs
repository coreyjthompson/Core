using MEI.SPDocuments.SPActionResult;

namespace MEI.SPDocuments.Document
{
    public interface IUploadAction
    {
        void ActionBeforeUpload(IDocumentBroker documentBroker);

        int ActionAfterUpload(UploadResult result);
    }
}
