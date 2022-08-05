using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments
{
    public interface IDocumentInfoAggregator
    {
        DocumentTypeInfo BuildDocumentTypeInfo(Type type);

        string CodeToAcronym(SPDocumentType code);

        string CodeToName(SPDocumentType code);

        string CodeToDisplayName(SPDocumentType code);

        SPDocumentType AcronymToCode(string acronym);

        IDictionary<SPDocumentType, DocumentTypeInfo> DocumentTypeInfos { get; }
    }

    internal class DocumentInfoAggregator
        : IDocumentInfoAggregator
    {
        private readonly SPDocumentsOptions _options;

        public DocumentInfoAggregator(IOptions<SPDocumentsOptions> options)
        {
            _options = options.Value;

            DocumentTypeInfos = BuildDocumentTypeInfos();
        }

        public DocumentTypeInfo BuildDocumentTypeInfo(Type type)
        {
            var documentType = SPDocumentType.None;
            string name = string.Empty;
            string displayName = string.Empty;
            string acronym = string.Empty;
            string folderName = string.Empty;
            string prefixText = string.Empty;
            var isDisabled = false;

            object[] attributes = type.GetCustomAttributes(true);
            var hasDocumentInfoAttribute = false;
            string baseDocumentPath = _options.BaseDocumentPath;
            string baseWebPath = _options.BaseWebPath;

            foreach (Attribute attr in attributes)
            {
                if (attr is DocumentInfoAttribute a1)
                {
                    hasDocumentInfoAttribute = true;
                    if (a1.DocumentType == SPDocumentType.None)
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.SPDocumentTypeCode. " + type.Name);
                    }

                    documentType = a1.DocumentType;

                    if (string.IsNullOrEmpty(a1.Name))
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.Name. " + type.Name);
                    }

                    name = a1.Name;

                    if (string.IsNullOrEmpty(a1.PrefixText))
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.PrefixText. " + type.Name);
                    }

                    prefixText = a1.PrefixText;

                    if (string.IsNullOrEmpty(a1.Acronym))
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.Acronym. " + type.Name);
                    }

                    acronym = a1.Acronym;

                    if (string.IsNullOrEmpty(a1.DisplayName))
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.DisplayName. " + type.Name);
                    }

                    displayName = a1.DisplayName;

                    if (string.IsNullOrEmpty(a1.FolderName))
                    {
                        throw new ApplicationException("This type has an invalid DocumentInfoAttribute.FolderName. " + type.Name);
                    }

                    folderName = a1.FolderName;
                }
                else if (attr is DocumentEnabledAttribute a)
                {
                    isDisabled = !a.IsEnabled;
                }
            }

            if (!hasDocumentInfoAttribute)
            {
                return null;

                //Throw New ApplicationException("This type must have a DocumentInfoAttribute. " & type.Name)
            }

            var info = new DocumentTypeInfo(documentType,
                name,
                displayName,
                acronym,
                folderName,
                prefixText,
                baseDocumentPath,
                baseWebPath,
                isDisabled);

            info.SPFields.Add(SPFieldNames.Id, "ID", SPFieldType.Counter);
            info.SPFields.Add(SPFieldNames.Created, "Created", SPFieldType.DateTime);
            info.SPFields.Add(SPFieldNames.Title, "Title", SPFieldType.Text);
            info.SPFields.Add(SPFieldNames.Modified, "Modified", SPFieldType.DateTime);
            info.SPFields.Add(SPFieldNames.FileName, "FileName", "FileLeafRef", SPFieldType.Text);
            info.SPFields.Add(SPFieldNames.FileRef, "FileRef", SPFieldType.Text);
            info.SPFields.Add(SPFieldNames.BaseName, "BaseName", SPFieldType.Text);

            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(true);

                foreach (Attribute attr in customAttributes)
                {
                    if (attr is SPFieldInfoAttribute a)
                    {
                        if (a.GetFileIndex == null)
                        {
                            info.SPFields.Add(a.EnumValue, a.DisplayName, a.InternalName, a.FieldType, true);
                        }
                        else
                        {
                            info.SPFields.Add(a.EnumValue, a.DisplayName, a.InternalName, a.FieldType, true, a.GetFileIndex.Value);
                        }
                    }
                }
            }

            return info;
        }

        private IDictionary<SPDocumentType, DocumentTypeInfo> BuildDocumentTypeInfos()
        {
            var infos = new Dictionary<SPDocumentType, DocumentTypeInfo>();

            Type type = typeof(IDocument);
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> types = assembly.GetTypes().Where(p => type.IsAssignableFrom(p) && p.IsClass).ToList();

            foreach (Type item in types)
            {
                DocumentTypeInfo info = BuildDocumentTypeInfo(item);
                if (info == null)
                {
                    continue;
                }

                infos.Add(info.DocumentType, info);
            }

            return infos;
        }

        public string CodeToAcronym(SPDocumentType code)
        {
            Preconditions.CheckEnum("code", code, SPDocumentType.None);

            return DocumentTypeInfos[code].Acronym;
        }

        public string CodeToName(SPDocumentType code)
        {
            Preconditions.CheckEnum("code", code, SPDocumentType.None);

            return DocumentTypeInfos[code].Name;
        }

        public string CodeToDisplayName(SPDocumentType code)
        {
            Preconditions.CheckEnum("code", code, SPDocumentType.None);

            return DocumentTypeInfos[code].DisplayName;
        }

        public SPDocumentType AcronymToCode(string acronym)
        {
            foreach (KeyValuePair<SPDocumentType, DocumentTypeInfo> kv in DocumentTypeInfos)
            {
                if (kv.Value.Acronym.ToLower() == acronym.ToLower())
                {
                    return kv.Key;
                }
            }

            foreach (KeyValuePair<SPDocumentType, DocumentTypeInfo> kv in DocumentTypeInfos)
            {
                if (kv.Value.Name.ToLower() == acronym.ToLower())
                {
                    return kv.Key;
                }
            }

            throw new ArgumentException(string.Format(Resources.Default.Invalid_acronym_value__0, acronym), nameof(acronym));
        }

        public IDictionary<SPDocumentType, DocumentTypeInfo> DocumentTypeInfos { get; }
    }
}
