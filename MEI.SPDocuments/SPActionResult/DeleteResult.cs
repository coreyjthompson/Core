using System;
using System.Collections;
using System.Globalization;
using System.Xml;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public class DeleteResult
        : SPActionResultBase
    {
        public DeleteResult(XmlNode node, string fileName)
        {
            Preconditions.CheckNotNull("node", node);
            Preconditions.CheckNotNullOrEmpty("fileName", fileName);

            FileName = fileName;
            Attributes = new Hashtable();

            ParseNode(node);
        }

        public DeleteResult(SPActionStatus status, Exception returnException, string fileName)
            : base(status, returnException)
        {
            FileName = fileName;
            Attributes = new Hashtable();
        }

        public DeleteResult(SPActionStatus status, string message, string fileName)
            : base(status, message)
        {
            FileName = fileName;
            Attributes = new Hashtable();
        }

        public string ErrorCode { get; private set; }

        public Hashtable Attributes { get; }

        public string FileName { get; private set; }

        public override SPDocumentPrivileges ActionType => SPDocumentPrivileges.Delete;

        private void ParseNode(XmlNode node)
        {
            Preconditions.CheckNotNull("node", node);

            var xmlDocument = new XmlDocument();
            var npsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            npsmgr.AddNamespace("ab", "http://schemas.microsoft.com/sharepoint/soap/");
            npsmgr.AddNamespace("z", "#RowsetSchema");
            XmlNodeList entries = node.SelectNodes("//ab:Result", npsmgr);

            if ((entries == null) || (entries.Count <= 0))
            {
                throw new ArgumentException(Resources.Default.node_does_not_contain_any_results);
            }

            foreach (XmlNode n in entries)
            {
                XmlNode errorCodeNode = n.SelectSingleNode("//ab:ErrorCode", npsmgr);
                if (errorCodeNode != null)
                {
                    ErrorCode = errorCodeNode.InnerText;
                }

                XmlNode errorTextNode = n.SelectSingleNode("//ab:ErrorText", npsmgr);
                if (errorTextNode != null)
                {
                    Message = errorTextNode.InnerText;
                }

                if (!string.IsNullOrEmpty(ErrorCode))
                {
                    Status = int.Parse(ErrorCode.Replace("0x", ""), NumberStyles.HexNumber) == 0
                        ? SPActionStatus.Success
                        : SPActionStatus.Failure;
                }

                XmlNodeList results = n.SelectNodes("z:row", npsmgr);
                if (results == null)
                {
                    continue;
                }

                foreach (XmlNode n2 in results)
                {
                    if (n2.Attributes == null)
                    {
                        continue;
                    }

                    foreach (XmlAttribute att in n2.Attributes)
                    {
                        if (Attributes.ContainsKey(att.Name))
                        {
                            continue;
                        }

                        Attributes.Add(att.Name, att.Value);
                    }
                }
            }

            if (Attributes.ContainsKey("ows_FileRef"))
            {
                FileName = Attributes["ows_FileRef"].ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("[ErrorCode={0}, Status={1}, Message={2}, FileName={3}]",
                ErrorCode,
                Status.ToDisplayNameLong(),
                Message.Length > 25 ? Message.Substring(0, 25) : Message,
                FileName);
        }
    }
}
