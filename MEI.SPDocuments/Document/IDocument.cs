using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    public interface IDocument
    {
        string SearchFieldValue { get; }

        bool IsDisabled { get; }

        SPDocumentType DocumentType { get; }

        string Acronym { get; }

        string Name { get; }

        string DisplayName { get; }

        string FolderName { get; }

        Company Company { get;  }

        byte[] Contents { get; }

        string FileExtension { get; }

        string FileName { get; }

        string ParsableFileName { get; }

        string ParsableFileNameWithoutExtension { get; }

        bool IsValid { get; }

        SPFieldCollection SPFields { get; }

        string SavedFilePath { get; set; }

        IList<string> AllowedFileTypes { get; }

        string PrefixText { get; }

        DocumentYear DocumentYear { get; }

        string UniqueIdentifiers { get; }

        string UniqueValues { get; }

        string OriginalFileName { get; }

        string OriginalFilePath { get; }

        IDictionary<string, string> GetUserFieldValues();

        string[] ParseFileName(string fileNameToParse);

        bool Setup(object[] objects);

        bool AbstractSetup(Hashtable values);

        bool ValidateFields();

        ISearchExpressionGroup GetGetFileSearchExpressionGroup(string delimitedSearchValues);

        string GetParsableFileNameWithCompanyYear(Company company, DocumentYear year);

        string GetParsableFileNameWithCompanyYearAndxIndex(Company company, DocumentYear year);

        WatermarkProfile GetWaterMarkProfile(string connectionString);

        SPDocumentBase With(string fileName, byte[] contents, Company company);

        SPDocumentBase With(string filePath, Company company);

        SPDocumentBase With(string filePath, string savedFilePath, Company company);

        SPDocumentBase With(byte[] contents, string fileExtension, Company company);

        SPDocumentBase With(string savedFilePath, Company company, string fileExtension);

        SPDocumentBase With(Company company, string fileExtension);
    }

    public abstract class SPDocumentBase
        : IDocument, IUploadAction
    {
        protected readonly IRepository Repository;
        protected readonly IDbUtilities DbUtilities;
        private readonly DocumentTypeInfo _info;

        private string _fileExtension;

        protected SPDocumentBase(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
        {
            Repository = repository;
            DbUtilities = dbUtilities;
            _info = documentTypeInfo;
        }

        public virtual SPDocumentBase WithCompany(Company company)
        {
            Company = company;

            return this;
        }

        public virtual SPDocumentBase With(string fileName, byte[] contents, Company company)
        {
            Preconditions.CheckNotNullOrEmpty("fileName", fileName);
            Preconditions.CheckNotNull("contents", contents);
            Preconditions.CheckEnum("company", company, Company.Undefined);

            OriginalFileName = fileName;

            if (Path.HasExtension(fileName))
            {
                FileExtension = Path.GetExtension(fileName);
            }

            Company = company;
            Contents = contents;

            ParseFileName(fileName);

            return this;
        }

        public virtual SPDocumentBase With(string filePath, Company company)
        {
            Preconditions.CheckNotNullOrEmpty("filePath", filePath);
            Preconditions.CheckEnum("company", company, Company.Undefined);

            using (var fStream = new FileStream(filePath, FileMode.Open))
            {
                //Array.Resize(ref Contents, Convert.ToInt32(fStream.Length));
                Contents = new byte[fStream.Length];
                fStream.Read(Contents, 0, Convert.ToInt32(fStream.Length));
            }

            OriginalFilePath = filePath;
            OriginalFileName = Path.GetFileName(filePath);

            if (Path.HasExtension(filePath))
            {
                FileExtension = Path.GetExtension(filePath);
            }

            Company = company;

            ParseFileName(Path.GetFileName(filePath));

            return this;
        }

        public virtual SPDocumentBase With(string filePath, string savedFilePath, Company company)
        {
            Preconditions.CheckNotNullOrEmpty("filePath", filePath);
            Preconditions.CheckNotNullOrEmpty("savedFilePath", savedFilePath);
            Preconditions.CheckEnum("company", company, Company.Undefined);

            OriginalFilePath = filePath;
            OriginalFileName = Path.GetFileName(filePath);

            if (Path.HasExtension(filePath))
            {
                FileExtension = Path.GetExtension(filePath);
            }

            Company = company;
            SavedFilePath = savedFilePath;

            ParseFileName(Path.GetFileName(filePath));

            return this;
        }

        public virtual SPDocumentBase With(byte[] contents, string fileExtension, Company company)
        {
            Preconditions.CheckNotNull("contents", contents);
            Preconditions.CheckNotNullOrEmpty("fileExtension", fileExtension);
            Preconditions.CheckEnum("company", company, Company.Undefined);

            FileExtension = fileExtension;
            Contents = contents;
            Company = company;

            return this;
        }

        public virtual SPDocumentBase With(string savedFilePath, Company company, string fileExtension)
        {
            Preconditions.CheckNotNullOrEmpty("savedFilePath", savedFilePath);
            Preconditions.CheckEnum("company", company, Company.Undefined);
            Preconditions.CheckNotNullOrEmpty("fileExtension", fileExtension);

            FileExtension = fileExtension;
            SavedFilePath = savedFilePath;
            Company = company;

            return this;
        }

        public virtual SPDocumentBase With(Company company, string fileExtension)
        {
            Preconditions.CheckEnum("company", company, Company.Undefined);
            Preconditions.CheckNotNullOrEmpty("fileExtension", fileExtension);

            FileExtension = fileExtension;
            Company = company;

            return this;
        }

        public virtual string SearchFieldValue => "";

        public bool IsDisabled
        {
            get
            {
                if (_info == null)
                {
                    return false;
                }

                return _info.IsDisabled;
            }
        }

        public string OriginalFileName { get; private set; }

        public string OriginalFilePath { get; private set; }

        public SPDocumentType DocumentType
        {
            get
            {
                if (_info == null)
                {
                    return SPDocumentType.None;
                }

                return _info.DocumentType;
            }
        }

        public string Acronym
        {
            get
            {
                if (_info == null)
                {
                    return string.Empty;
                }

                return _info.Acronym;
            }
        }

        public string Name
        {
            get
            {
                if (_info == null)
                {
                    return string.Empty;
                }

                return _info.Name;
            }
        }

        public string DisplayName
        {
            get
            {
                if (_info == null)
                {
                    return string.Empty;
                }

                return _info.DisplayName;
            }
        }

        public string FolderName
        {
            get
            {
                if (_info == null)
                {
                    return string.Empty;
                }

                return _info.FolderName;
            }
        }

        public Company Company { get; protected set; }

        public string PrefixText
        {
            get
            {
                if (_info == null)
                {
                    return string.Empty;
                }

                return _info.PrefixText;
            }
        }

        public byte[] Contents { get; set; }

        public string FileExtension
        {
            get => _fileExtension;

            set => _fileExtension = value.Replace(".", string.Empty);
        }

        public abstract string FileName { get; }

        public abstract string UniqueIdentifiers { get; }

        public abstract string UniqueValues { get; }

        public virtual string ParsableFileName => FileName;

        public string ParsableFileNameWithoutExtension => Path.GetFileNameWithoutExtension(ParsableFileName);

        public virtual bool IsValid
        {
            get
            {
                if (Company == Company.Undefined)
                {
                    return false;
                }

                if ((Contents == null) && string.IsNullOrEmpty(SavedFilePath))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(FileExtension))
                {
                    return false;
                }

                return true;
            }
        }

        public string SavedFilePath { get; set; }

        public virtual IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf"
            };

        public virtual DocumentYear DocumentYear { get; internal set; }

        public SPFieldCollection SPFields
        {
            get
            {
                if (_info == null)
                {
                    return new SPFieldCollection();
                }

                return _info.SPFields;
            }
        }

        public abstract IDictionary<string, string> GetUserFieldValues();

        public virtual string[] ParseFileName(string fileNameToParse)
        {
            Preconditions.CheckNotNullOrEmpty("fileNameToParse", fileNameToParse);

            string[] fileNameParts;
            int userFieldCount = GetUserFieldCount();

            // add one to userFieldCount for the prefix text
            userFieldCount += 1;

            if (AllowedFileTypes.Count > 0)
            {
                if (!AllowedFileTypes.Contains(_fileExtension.ToLower()))
                {
                    throw new InvalidSPDocFileNameException(
                        string.Format(Resources.Default.Invalid__0__filename___1__The_file_must_have_one_of_these_extensions__2__,
                            Name,
                            fileNameToParse,
                            string.Join(";", AllowedFileTypes.ToArray())),
                        InvalidSPDocFileNameExceptionType.Ext);
                }
            }

            if (Path.HasExtension(fileNameToParse))
            {
                fileNameParts = Path.GetFileNameWithoutExtension(fileNameToParse).Split(new[] { "__" }, StringSplitOptions.None);
            }
            else
            {
                fileNameParts = fileNameToParse.Split(new[] { "__" }, StringSplitOptions.None);
            }

            if (fileNameParts.Length != userFieldCount)
            {
                throw new InvalidSPDocFileNameException(
                    string.Format(Resources.Default.Invalid__0__filename___1__The_filename_must_contain__2__parts_,
                        Name,
                        fileNameToParse,
                        userFieldCount),
                    InvalidSPDocFileNameExceptionType.ElementCount);
            }

            if (fileNameParts[0] != PrefixText)
            {
                throw new InvalidSPDocFileNameException(
                    string.Format(Resources.Default.Invalid__0__filename___1__The_filename_must_begin_with__2__, Name, fileNameToParse, PrefixText),
                    InvalidSPDocFileNameExceptionType.FirstElement);
            }

            return fileNameParts;
        }

        public abstract bool Setup(object[] objects);

        public abstract bool AbstractSetup(Hashtable values);

        public abstract bool ValidateFields();

        public ISearchExpressionGroup GetGetFileSearchExpressionGroup(string delimitedSearchValues)
        {
            var seg = new SearchExpressionGroup(this);

            string[] expressions = delimitedSearchValues.Split(";".ToCharArray());
            List<SPField> userFields = SPFields.Where(x => x.IsUserDefined && x.GetFileIndex != null).OrderBy(y => y.GetFileIndex).ToList();

            for (var index = 0; index < expressions.Length; index++)
            {
                if (string.IsNullOrEmpty(expressions[index]))
                {
                    continue;
                }

                seg.And(userFields[index].EnumValue, CamlComparison.Equal, expressions[index]);
            }

            return seg;
        }

        public virtual string GetParsableFileNameWithCompanyYear(Company company, DocumentYear year)
        {
            return string.Format("{0}__{1}{2}", ParsableFileNameWithoutExtension, company.ToDisplayNameLong(), year.ToDisplayNameLong());
        }

        public virtual string GetParsableFileNameWithCompanyYearAndxIndex(Company company, DocumentYear year)
        {
            string xIndex = Repository.GetDocXIndex(Acronym).ToString();

            return string.Format("{0}__{1}{2}({3})",
                ParsableFileNameWithoutExtension,
                company.ToDisplayNameLong(),
                year.ToDisplayNameLong(),
                xIndex.PadLeft(10, '0'));
        }

        public virtual WatermarkProfile GetWaterMarkProfile(string connectionString)
        {
            return null;
        }

        /// <summary>
        ///     Provides ability for actions to be performed on a document prior to an upload.
        /// </summary>
        public virtual void ActionBeforeUpload(IDocumentBroker documentBroker)
        {}

        /// <summary>
        ///     Provides ability for actions to be performed on a document after an upload.
        /// </summary>
        /// <param name="result">The result of the upload.</param>
        /// <returns>An integer indicating the result of the actions.</returns>
        public virtual int ActionAfterUpload(UploadResult result)
        {
            return -1;
        }

        protected int GetUserFieldCount()
        {
            return SPFields.Count(x => x.IsUserDefined);
        }

        protected string MakeFileName(params object[] values)
        {
            Preconditions.CheckNotNull("values", values);

            if (!IsValid)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            sb.Append(PrefixText);

            foreach (object value in values)
            {
                if (value == null)
                {
                    sb.Append("__");
                }
                else
                {
                    sb.AppendFormat("__{0}", value);
                }
            }

            sb.AppendFormat(".{0}", _fileExtension);

            return sb.ToString();
        }

        protected void ThrowFileNameExceptionInvalidType(string invalidFileName, string partName, string expectedType)
        {
            throw new InvalidSPDocFileNameException(
                string.Format(Resources.Default.ParseFileName_Invalid__fileName__must_be_of_type, invalidFileName, partName, expectedType),
                InvalidSPDocFileNameExceptionType.ElementType);
        }

        protected void ThrowFileNameExceptionInvalidType(string invalidFileName, SPFieldNames enumValue, string expectedType)
        {
            ThrowFileNameExceptionInvalidType(invalidFileName, SPFields[enumValue].DisplayName, expectedType);
        }

        protected void ThrowFileNameExceptionNoDBMatch(string fieldName, string value)
        {
            throw new InvalidSPDocFileNameException(string.Format(Resources.Default.ValidateFields_Invalid__filename__not_found_in_database,
                    fieldName,
                    Company.ToDisplayNameLong(),
                    DocumentYear.ToDisplayNameLong(),
                    value),
                InvalidSPDocFileNameExceptionType.NoDBMatch);
        }

        protected void ThrowFileNameExceptionNoDBMatch(SPFieldNames enumValue, string value)
        {
            ThrowFileNameExceptionNoDBMatch(SPFields[enumValue].DisplayName, value);
        }

        protected DocumentYear ExtractDocumentYear(string programId)
        {
            if (string.IsNullOrEmpty(programId))
            {
                return DocumentYear.Undefined;
            }

            if (programId.Length < 2)
            {
                return DocumentYear.Undefined;
            }

            return string.Format("20{0}", programId.Substring(programId.Length - 2, 2)).ToDocumentYear();
        }

        private string MakeQueryField(string fieldName, string valueType, string value)
        {
            Preconditions.CheckNotNullOrEmpty("fieldName", fieldName);

            if (string.IsNullOrEmpty(value))
            {
                return string.Format("<FieldRef Name='{0}'></Field>", fieldName);
            }

            return string.Format("<FieldRef Name='{0}' /><Value Type='{1}'>{2}</Value>", fieldName, valueType, SecurityElement.Escape(value));
        }
    }

    public enum SPFieldNames
    {
        None,

        Id,
        Created,
        Title,
        Modified,
        FileName,
        BaseName,
        FileRef,

        AdHocSlideKitId,
        AgreementId,
        ApprovalId,
        AttendeeId,
        CheckType,
        DateTime,
        Description,
        DisputeId,
        DocumentSearchDocumentTypeId,
        DocumentSearchProductId,
        DocumentTitle,
        DocumentType,
        DocumentYear,
        EndDate,
        ExceptionType,
        ExpenseCounter,
        Extension,
        FieldOpsApproved,
        FileType,
        Found,
        HandoutId,
        HcpFirstName,
        HcpLastName,
        ImageSize,
        InviteId,
        InviteType,
        InvoiceNumber,
        Keywords,
        LetterType,
        MaterialKitId,
        MEIApproved,
        MenuType,
        ParticipantCounter,
        PifId,
        PlacematId,
        PosterId,
        ProgramId,
        ProofIndex,
        RecordLocator,
        SaveTheDateId,
        SearchDateTime,
        SearchValue,
        SecondAddressAmendmentId,
        SequenceNumber,
        Site,
        SiteLocation,
        SlideDeckTitle,
        SlideKitId,
        SpeakerCounter,
        SpeakerFirstName,
        SpeakerLastName,
        SpeakerName,
        SpeakerNominationId,
        SpSeriesId,
        StartDate,
        State,
        StatusCode,
        Territory,
        TestColumn,
        ThankYouId,
        TicketNumber,
        TinType,
        TrackNumber,
        TramsHistoryId,
        TravelDocId,
        TrifoldId,
        UploadDate,
        UploadFileName,
        UploadUserName,
        VendorId,
        VersionNumber
    }
}
