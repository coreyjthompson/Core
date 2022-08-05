using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace MEI.SPDocuments.SPActionResult
{
    [Serializable]
    public class VoidSearchResult
    {
        private string _fileRef;

        public VoidSearchResult(XmlNode node, SPFieldCollection spFields, string siteUrl)
        {
            Preconditions.CheckNotNull("node", node);

            UserFields = new Dictionary<string, string>();
            Versions = new List<VoidSearchVersionsResult>();

            ParseNode(node, spFields, siteUrl);
        }

        public VoidSearchResult(DataRow dr, SPFieldCollection spFields, string siteUrl)
        {
            Preconditions.CheckNotNull("dr", dr);

            UserFields = new Dictionary<string, string>();
            Versions = new List<VoidSearchVersionsResult>();

            ParseNode(dr, spFields, siteUrl);
        }

        public VoidSearchResult(DateTime created,
                                string linkFileName,
                                string fileRef,
                                int fileSize,
                                string id,
                                DateTime modified,
                                string uniqueId,
                                string title,
                                IList<VoidSearchVersionsResult> versions,
                                Dictionary<string, string> userFields)
        {
            Created = created;
            DocumentAbsoluteUrl = null;
            DocumentName = linkFileName;
            _fileRef = fileRef;
            DocumentRelativeUrl = null;
            FileSize = fileSize;
            Id = id;
            Modified = modified;
            UniqueId = uniqueId;
            Title = title;
            Versions = versions;
            UserFields = userFields;

            UserFields = new Dictionary<string, string>();
            Versions = new List<VoidSearchVersionsResult>();
        }

        public VoidSearchResult(SearchResult sr)
        {
            Created = sr.Created;
            DocumentAbsoluteUrl = null;
            DocumentName = sr.DocumentName;

            //_fileRef = sr.fileRef
            DocumentRelativeUrl = null;
            FileSize = sr.FileSize;
            Id = sr.Id;
            Modified = sr.Modified;
            UniqueId = sr.UniqueId;
            Title = sr.Title;

            //_versions = sr.Versions
            UserFields = sr.UserFields;

            UserFields = new Dictionary<string, string>();
            Versions = new List<VoidSearchVersionsResult>();
        }

        public DateTime Created { get; private set; }

        public double Version
        {
            get
            {
                if (Versions.Count == 0)
                {
                    return 1;
                }

                return Versions.Count - 1;
            }
        }

        public Uri DocumentAbsoluteUrl { get; private set; }

        public string DocumentName { get; private set; }

        public string DocumentRelativeUrl { get; private set; }

        public int FileSize { get; private set; }

        public string Id { get; private set; }

        public DateTime Modified { get; private set; }

        public string UniqueId { get; private set; }

        public IList<VoidSearchVersionsResult> Versions { get; }

        public string Title { get; private set; }

        public Dictionary<string, string> UserFields { get; }

        public bool IsDisabled { get; set; }

        private void ParseNode(XmlNode node, IList<SPField> spFields, string siteUrl)
        {
            Preconditions.CheckNotNull("node", node);
            Preconditions.CheckNotNull("spFields", spFields);
            Preconditions.CheckNotNullOrEmpty("siteUrl", siteUrl);

            if (node.Attributes?["ows_FileRef"] != null)
            {
                _fileRef = node.Attributes["ows_FileRef"].Value;
            }

            if (node.Attributes?["ows_Created"] != null)
            {
                if (DateTime.TryParse(node.Attributes["ows_Created"].Value, out DateTime tempDateTime))
                {
                    Created = tempDateTime;
                }
            }

            if (node.Attributes?["ows_Modified"] != null)
            {
                if (DateTime.TryParse(node.Attributes["ows_Modified"].Value, out DateTime tempDateTime))
                {
                    Modified = tempDateTime;
                }
            }

            string[] temp1 = node.Attributes?["ows_File_x0020_Size"]?.Value.Split("#".ToCharArray());

            if (temp1?.Length == 2)
            {
                if (int.TryParse(temp1[1], out int tempInteger))
                {
                    FileSize = tempInteger;
                }
            }

            if (node.Attributes?["ows_ID"] != null)
            {
                Id = node.Attributes["ows_ID"].Value;
            }

            if (node.Attributes?["ows_UniqueId"] != null)
            {
                UniqueId = node.Attributes["ows_UniqueId"].Value;
            }

            if (node.Attributes?["ows_Title"] != null)
            {
                Title = node.Attributes["ows_Title"].Value;
            }

            foreach (SPField uf in spFields)
            {
                if (!uf.IsUserDefined || UserFields.Keys.Contains(uf.InternalName))
                {
                    continue;
                }

                if (node.Attributes?[string.Format("ows_{0}", uf.InternalName)] != null)
                {
                    UserFields.Add(uf.InternalName, node.Attributes[string.Format("ows_{0}", uf.InternalName)].Value);
                }
            }

            string[] fileRefs = _fileRef.Split("#".ToCharArray());

            if (fileRefs.Length == 2)
            {
                DocumentName = node.Attributes?["ows_LinkFilename"].Value;
                DocumentAbsoluteUrl = null;
                DocumentRelativeUrl = null;
            }
        }

        private void ParseNode(DataRow dr, IList<SPField> spFields, string siteUrl)
        {
            Preconditions.CheckNotNull("dr", dr);
            Preconditions.CheckNotNull("spFields", spFields);
            Preconditions.CheckNotNullOrEmpty("siteUrl", siteUrl);

            if (dr["ows_FileRef"] != null)
            {
                _fileRef = dr["ows_FileRef"].ToString();
            }

            if (dr["ows_Created"] != null)
            {
                if (DateTime.TryParse(dr["ows_Created"].ToString(), out DateTime tempDateTime))
                {
                    Created = tempDateTime;
                }
            }

            if (dr["ows_Modified"] != null)
            {
                if (DateTime.TryParse(dr["ows_Modified"].ToString(), out DateTime tempDateTime))
                {
                    Modified = tempDateTime;
                }
            }

            string[] temp1 = dr["ows_File_x0020_Size"]?.ToString().Split("#".ToCharArray());

            if (temp1?.Length == 2)
            {
                if (int.TryParse(temp1[1], out int tempInteger))
                {
                    FileSize = tempInteger;
                }
            }

            if (dr["ows_ID"] != null)
            {
                Id = dr["ows_ID"].ToString();
            }

            if (dr["ows_UniqueId"] != null)
            {
                UniqueId = dr["ows_UniqueId"].ToString();
            }

            if (dr["ows_Title"] != null)
            {
                Title = dr["ows_Title"].ToString();
            }

            foreach (SPField uf in spFields)
            {
                if (!uf.IsUserDefined || UserFields.Keys.Contains(uf.InternalName))
                {
                    continue;
                }

                if (dr[string.Format("ows_{0}", uf.InternalName)] != null)
                {
                    UserFields.Add(uf.InternalName, dr[string.Format("ows_{0}", uf.InternalName)].ToString());
                }
            }

            if (dr["ows_LinkFilename"] != null)
            {
                DocumentName = dr["ows_LinkFilename"].ToString();
            }

            string[] fileRefs = _fileRef.Split("#".ToCharArray());
            if (fileRefs.Length == 2)
            {
                DocumentAbsoluteUrl = null;
                DocumentRelativeUrl = null;
            }
        }

        public override string ToString()
        {
            return string.Format("[Id:{0}, Name={1}]", Id, DocumentName);
        }
    }
}
