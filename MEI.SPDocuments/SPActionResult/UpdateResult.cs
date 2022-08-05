using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.SPActionResult
{
    public class UpdateResult
        : SPActionResultBase
    {
        public UpdateResult(XmlNode node)
        {
            Preconditions.CheckNotNull("node", node);

            Attributes = new Dictionary<string, string>();

            ParseNode(node);
        }

        public UpdateResult(SPActionStatus status, Exception returnException)
            : base(status, returnException)
        {
            Attributes = new Dictionary<string, string>();
        }

        public UpdateResult(SPActionStatus status, string message)
            : base(status, message)
        {
            Attributes = new Dictionary<string, string>();
        }

        public string ErrorCode { get; private set; }

        public Dictionary<string, string> Attributes { get; }

        public string FileName { get; private set; }

        public override SPDocumentPrivileges ActionType => SPDocumentPrivileges.Update;

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
                    if (int.Parse(ErrorCode.Replace("0x", ""), NumberStyles.HexNumber) == 0)
                    {
                        Status = SPActionStatus.Success;
                    }
                    else
                    {
                        Status = SPActionStatus.Failure;
                    }
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
                        if (!Attributes.ContainsKey(att.Name))
                        {
                            Attributes.Add(att.Name, att.Value);
                        }
                    }
                }
            }

            if (Attributes.ContainsKey("ows_FileRef"))
            {
                FileName = Attributes["ows_FileRef"];
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
