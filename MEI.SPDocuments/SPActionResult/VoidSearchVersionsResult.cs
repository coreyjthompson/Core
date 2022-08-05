using System;
using System.Linq;
using System.Xml;

namespace MEI.SPDocuments.SPActionResult
{
    [Serializable]
    public class VoidSearchVersionsResult
    {
        public VoidSearchVersionsResult(double version,
                                        Uri absoluteUrl,
                                        DateTime created,
                                        string createdBy,
                                        int fileSize,
                                        string comment,
                                        bool isCurrentVersion)
        {
            Version = version;
            AbsoluteUrl = absoluteUrl;
            Created = created;
            CreatedBy = createdBy;
            FileSize = fileSize;
            Comment = comment;
            IsCurrentVersion = isCurrentVersion;
        }

        public VoidSearchVersionsResult(SearchVersionsResult svr)
        {
            Version = svr.Version;
            AbsoluteUrl = svr.AbsoluteUrl;
            Created = svr.Created;
            CreatedBy = svr.CreatedBy;
            FileSize = svr.FileSize;
            Comment = svr.Comment;
            IsCurrentVersion = svr.IsCurrentVersion;
        }

        public VoidSearchVersionsResult(XmlNode node)
        {
            ParseNode(node);
        }

        public string Comment { get; private set; }

        public int DisplayVersionNumber { get; set; }

        public string DisplayName
        {
            get
            {
                string[] fileName = AbsoluteUrl.ToString().Split(Convert.ToChar("/"));
                AbsoluteUrl = null;

                return string.Format("v{0} - {1}", Version - 1, fileName[fileName.Length - 1]);
            }
        }

        public DateTime Created { get; private set; }

        public string CreatedBy { get; private set; }

        public Uri AbsoluteUrl { get; private set; }

        public int FileSize { get; private set; }

        public bool IsCurrentVersion { get; private set; }

        public double Version { get; private set; }

        public bool IsDisabled { get; set; }

        private void ParseNode(XmlNode node)
        {
            Preconditions.CheckNotNull("node", node);
            Preconditions.CheckNotNull("node.Attributes(\"version\")", node.Attributes?["version"]);
            Preconditions.CheckNotNull("node.Attributes(\"url\")", node.Attributes?["url"]);

            AbsoluteUrl = new Uri(node.Attributes?["url"].Value ?? throw new InvalidOperationException("node must have attributes"));

            if (node.Attributes["version"].Value.StartsWith("@"))
            {
                IsCurrentVersion = true;

                if (double.TryParse(node.Attributes["version"].Value.Substring(1), out double tempVersion))
                {
                    Version = tempVersion;
                }
                else
                {
                    Version = -1;
                }
            }
            else
            {
                IsCurrentVersion = false;

                if (double.TryParse(node.Attributes["version"].Value, out double _))
                {
                    Version = Convert.ToDouble(node.Attributes["version"].Value);
                }
                else
                {
                    Version = -1;
                }
            }

            if (node.Attributes["created"] != null)
            {
                if (DateTime.TryParse(node.Attributes["created"].Value, out DateTime tempDateTime))
                {
                    Created = tempDateTime;
                }
            }

            if (node.Attributes["createdBy"] != null)
            {
                CreatedBy = node.Attributes["createdBy"].Value;
            }

            if (node.Attributes["size"] != null)
            {
                if (int.TryParse(node.Attributes["size"].Value, out int tempInteger))
                {
                    FileSize = tempInteger;
                }
            }

            if (node.Attributes["comment"] != null)
            {
                Comment = node.Attributes["comment"].Value;
            }
        }

        public override string ToString()
        {
            return string.Format("[AbsoluteUrl={0}, Version={1}]", AbsoluteUrl.AbsoluteUri, Version);
        }
    }
}
