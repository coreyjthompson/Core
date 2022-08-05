using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.AttendeeSignInSheet, "AttendeeSignInSheet", "ASIS", "ASIS", "Attendee SignIn Sheet")]
    public class AttendeeSignInSheet
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal AttendeeSignInSheet(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public AttendeeSignInSheet WithValues(string programId, string attendeeId, string extension)
        {
            ProgramId = programId;
            AttendeeId = attendeeId;
            Extension = extension;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "tiff",
                "bmp",
                "jpg",
                "gif",
                "png"
            };

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "ProgramID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.AttendeeId, "Attendee_x0020_ID", SPFieldType.Text, "AttendeeID", 1)]
        public string AttendeeId { get; private set; }

        [SPFieldInfo(SPFieldNames.Extension, "Extension", SPFieldType.Text, 2)]
        public string Extension { get; private set; }

        public override string FileName => MakeFileName(ProgramId, AttendeeId, Extension);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId";

        public override string UniqueValues => ProgramId;

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
            AttendeeId = objects[1].ToString();
            Extension = objects[2].ToString();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.AttendeeId].InternalName))
            {
                AttendeeId = (string)values[SPFields[SPFieldNames.AttendeeId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Extension].InternalName))
            {
                Extension = (string)values[SPFields[SPFieldNames.Extension].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.AttendeeId].InternalName, AttendeeId },
                       { SPFields[SPFieldNames.Extension].InternalName, Extension }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            Preconditions.CheckNotNullOrEmpty("fileNameToParse", fileNameToParse);

            string[] fileNameParts;
            int userFieldCount = GetUserFieldCount();

            //add one to userFieldCount for the prefix text
            userFieldCount += 1;

            string[] fileSeparator = { "__" };

            if (Path.HasExtension(fileNameToParse))
            {
                fileNameParts = Path.GetFileNameWithoutExtension(fileNameToParse).Split(fileSeparator, StringSplitOptions.None);
            }
            else
            {
                fileNameParts = fileNameToParse.Split(fileSeparator, StringSplitOptions.None);
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

            ProgramId = fileNameParts[1];
            AttendeeId = fileNameParts[2];
            Extension = fileNameParts[3];

            return fileNameParts;
        }
    }
}
