using System;
using System.Xml;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public class RenameResult
        : SPActionResultBase
    {
        public RenameResult(XmlNode node, string fileName, string newFileName)
        {
            Preconditions.CheckNotNull("node", node);

            FileName = fileName;
            NewFileName = newFileName;
            ParseNode(node);
        }

        public RenameResult(SPActionStatus status, Exception returnException, string fileName, string newFileName)
            : base(status, returnException)
        {
            FileName = fileName;
            NewFileName = newFileName;
        }

        public RenameResult(SPActionStatus status, string message, string fileName, string newFileName)
            : base(status, message)
        {
            FileName = fileName;
            NewFileName = newFileName;
        }

        public string FileName { get; }

        public string NewFileName { get; }

        public override SPDocumentPrivileges ActionType => SPDocumentPrivileges.Rename;

        private void ParseNode(XmlNode node)
        {
            Preconditions.CheckNotNull("node", node);

            if (node.Attributes?["ReturnType"] != null)
            {
                Status = node.Attributes["ReturnType"].Value.ToActionStatus();
            }
            else
            {
                Status = SPActionStatus.UnknownError;
            }

            if (node.Attributes?["ReturnMessage"] != null)
            {
                Message = node.Attributes["ReturnMessage"].Value;
            }
            else
            {
                Message = Resources.Default.No_message_returned;
            }
        }

        public override string ToString()
        {
            return string.Format("[Status={0}, Message={1}]",
                Status.ToDisplayNameLong(),
                Message.Length > 25 ? Message.Substring(0, 25) : Message);
        }
    }
}
