using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EInvite, "eInvite", "ENVT")]
    public class eInvite
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal eInvite(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public eInvite WithValues(string programId, string imageSize)
        {
            ProgramId = programId;
            ImageSize = imageSize;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        public override string ParsableFileName => MakeFileName(ProgramId, ImageSize);

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.ImageSize, "ImageSize", SPFieldType.Text, 1)]
        public string ImageSize { get; private set; }

        public override string FileName => MakeFileName(ProgramId, ImageSize);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ImageSize))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;ImageSize";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, ImageSize);

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "jpg",
                "jpeg",
                "png",
                "tiff"
            };

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year)
        {
            if (year == DocumentYear.Undefined)
            {
                return new SearchExpressionGroup(this);
            }

            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Contains, year.ToProgramIdYear());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
            }

            return true;
        }

        public override bool Setup(object[] objects)
        {
            int userFieldCount = GetUserFieldCount();

            //Add three to userFieldCount for the contents, fileExtension, and company
            userFieldCount += 3;

            if (objects.Length != userFieldCount)
            {
                return false;
            }

            ProgramId = objects[0].ToString();
            ImageSize = objects[1].ToString();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ImageSize].InternalName))
            {
                ImageSize = values[SPFields[SPFieldNames.ImageSize].InternalName].ToString();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.ImageSize].InternalName, ImageSize }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            ImageSize = fileNameParts[2];

            return fileNameParts;
        }
    }
}
