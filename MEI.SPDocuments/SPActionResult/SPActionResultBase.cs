using System;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public abstract class SPActionResultBase
        : ISPActionResult
    {
        private string _message;

        protected SPActionResultBase()
        { }

        protected SPActionResultBase(SPActionStatus status, Exception returnException)
            : this(status)
        {
            ReturnException = returnException;
        }

        protected SPActionResultBase(SPActionStatus status, string message)
            : this(status)
        {
            _message = message;
        }

        private SPActionResultBase(SPActionStatus status)
        {
            Status = status;
        }

        public SPActionStatus Status { get; set; }

        public string Message
        {
            get
            {
                if (ReturnException != null)
                {
                    return ReturnException.ToString();
                }

                return _message;
            }

            set => _message = value;
        }

        public Exception ReturnException { get; set; }

        public abstract SPDocumentPrivileges ActionType { get; }
    }
}
