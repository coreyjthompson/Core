using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SignInSheet, "SignInSheet", "SIS", "SIS", "SignIn Sheet")]
    public class SignInSheet
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        private readonly IEmailer _emailer;
        private readonly SPDocumentsOptions _options;

        internal SignInSheet(IRepository repository,
                             IDbUtilities dbUtilities,
                             DocumentTypeInfo documentTypeInfo,
                             IEmailer emailer,
                             IOptions<SPDocumentsOptions> options)
            : base(repository, dbUtilities, documentTypeInfo)
        {
            _emailer = emailer;
            _options = options.Value;
        }

        public SignInSheet WithValues(string programId)
        {
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ProgramId);

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
            Contents = (byte[])objects[1];
            FileExtension = objects[2].ToString();
            Company = (Company)objects[3];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            return fileNameParts;
        }

        public override int ActionAfterUpload(UploadResult result)
        {
            if ((Company == Company.Abbott) && DocumentYear.Compare(DocumentYear.Year2012, "=>")
                                            && (result.Status == SPActionStatus.Success))
            {
                Repository.CreateTicklersOnSignInSheetUpload(ProgramId, Company, DocumentYear);
            }
            else if (result.Status != SPActionStatus.Success)
            {
                string message = string.Format("Sign-In Sheet Upload to SharePoint was not successful.{0}", Environment.NewLine);
                message = string.Format("{0}ProgramId: {1}{2}FileName: {3}{2}{2}{4}",
                    message,
                    ProgramId,
                    Environment.NewLine,
                    FileName,
                    result.Message);

                SendSignInSheetUploadTimeoutEmail(message);
            }

            return 0;
        }

        private void SendSignInSheetUploadTimeoutEmail(string body)
        {
            char delimiter = Convert.ToChar(";");
            string[] toAddresses = _options.SignInUploadTimeoutAlert.Split(delimiter);
            const string subject = "SignInSheet Upload Timeout";

            _emailer.SendEmail(subject, body, toAddresses);
        }
    }
}
