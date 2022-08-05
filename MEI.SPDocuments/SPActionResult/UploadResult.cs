using System;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public class UploadResult
        : SPActionResultBase
    {

        public UploadResult(SPActionStatus status, Exception returnException)
            : base(status, returnException)
        { }

        public UploadResult(SPActionStatus status, string message)
            : base(status, message)
        { }

        public override SPDocumentPrivileges ActionType => SPDocumentPrivileges.Upload;

        public override string ToString()
        {
            return string.Format("[Status={0}, Message={1}]",
                Status.ToDisplayNameLong(),
                Message.Length > 25 ? Message.Substring(0, 25) : Message);
        }
    }
}
