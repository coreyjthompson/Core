using System;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public interface ISPActionResult
    {
        SPActionStatus Status { get; set; }
        string Message { get; set; }
        Exception ReturnException { get; set; }
        SPDocumentPrivileges ActionType { get; }
    }
}
