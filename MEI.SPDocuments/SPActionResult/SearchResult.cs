using System;
using System.Collections.Generic;
using System.Linq;

namespace MEI.SPDocuments.SPActionResult
{
    [Serializable]
    public class SearchResult
    {
        private readonly IEncryptor _encryptor;

        public SearchResult(IEncryptor encryptor, Microsoft.SharePoint.Client.File file, SPFieldCollection spFields, string baseSiteUrl)
        {
            _encryptor = encryptor;

            UserFields = new Dictionary<string, string>();
            Versions = new List<SearchVersionsResult>();

            ParseNode(file, spFields, baseSiteUrl);
        }

        public SearchResult(IEncryptor encryptor, SearchResult searchResult)
        {
            _encryptor = encryptor;

            Created = searchResult.Created;
            EncryptedDocumentAbsoluteUrl = searchResult.EncryptedDocumentAbsoluteUrl;
            DocumentName = searchResult.DocumentName;
            FileRef = searchResult.FileRef;
            EncryptedDocumentRelativeUrl = searchResult.EncryptedDocumentRelativeUrl;
            FileSize = searchResult.FileSize;
            Id = searchResult.Id;
            Modified = searchResult.Modified;
            UniqueId = searchResult.UniqueId;
            Title = searchResult.Title;
            Versions = searchResult.Versions;
            UserFields = searchResult.UserFields;
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

        public string EncryptedDocumentAbsoluteUrl { get; private set; }

        public string EncryptedDocumentRelativeUrl { get; private set; }

        public string DocumentName { get; private set; }

        public int FileSize { get; private set; }

        public string Id { get; private set; }

        public DateTime Modified { get; private set; }

        public string UniqueId { get; private set; }

        public IList<SearchVersionsResult> Versions { get; }

        public string Title { get; private set; }

        public Dictionary<string, string> UserFields { get; }

        public bool IsDisabled { get; set; }

        public string FileRef { get; private set; }

        private void ParseNode(Microsoft.SharePoint.Client.File file, IList<SPField> spFields, string baseSiteUrl)
        {
            FileRef = file.ListItemAllFields["FileRef"].ToString();

            if (DateTime.TryParse(file.ListItemAllFields["Created"].ToString(), out DateTime tempDateTime))
            {
                Created = tempDateTime;
            }

            if (DateTime.TryParse(file.ListItemAllFields["Modified"].ToString(), out DateTime tempDateTime2))
            {
                Modified = tempDateTime2;
            }

            if (int.TryParse(file.ListItemAllFields["File_x0020_Size"].ToString(), out int tempInteger))
            {
                FileSize = tempInteger;
            }

            Id = file.ListItemAllFields["ID"].ToString();

            UniqueId = file.ListItemAllFields["UniqueId"].ToString();

            Title = file.ListItemAllFields["Title"].ToString();

            foreach (SPField uf in spFields)
            {
                if (!uf.IsUserDefined || UserFields.Keys.Contains(uf.InternalName))
                {
                    continue;
                }

                UserFields.Add(uf.InternalName, file.ListItemAllFields[uf.InternalName].ToString());
            }

            DocumentName = file.ListItemAllFields["FileLeafRef"].ToString();

            if (file.ListItemAllFields.FieldValues.ContainsKey("FileRef"))
            {
                EncryptedDocumentAbsoluteUrl = _encryptor.Encrypt(baseSiteUrl + file.ListItemAllFields["FileRef"]);
                EncryptedDocumentRelativeUrl = _encryptor.Encrypt(file.ListItemAllFields["FileRef"].ToString());
            }
        }

        public override string ToString()
        {
            return string.Format("[Created={0}, Name={1}, FileSize={2}, Id={3}, Modified={4}, UniqueId={5}, Title={6}]",
                Created,
                DocumentName,
                FileSize,
                Id,
                Modified,
                UniqueId,
                Title);
        }
    }

    internal class DecryptedSearchResult
        : SearchResult
    {
        private readonly IEncryptor _encryptor;

        public DecryptedSearchResult(IEncryptor encryptor, SearchResult searchResult)
            : base(encryptor, searchResult)
        {
            _encryptor = encryptor;
        }

        public string DecryptedDocumentAbsoluteUrl => _encryptor.Decrypt(EncryptedDocumentAbsoluteUrl);

        public string DecryptedDocumentRelativeUrl => _encryptor.Decrypt(EncryptedDocumentRelativeUrl);
    }
}
