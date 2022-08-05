using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.TravelInvoice, "TravelInvoice", "TINV")]
    public class TravelInvoice
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal TravelInvoice(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public TravelInvoice WithValues(string programId, int travelDocId, int tramsHistoryId)
        {
            ProgramId = programId;
            TravelDocId = travelDocId;
            TramsHistoryId = tramsHistoryId;

            return this;
        }

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.TravelDocId, "TravelDocId", SPFieldType.Text, 1)]
        public int TravelDocId { get; private set; }

        [SPFieldInfo(SPFieldNames.TramsHistoryId, "TramsHistoryId", SPFieldType.Text,2 )]
        public int TramsHistoryId { get; private set; }

        [SPFieldInfo(SPFieldNames.InvoiceNumber, "InvoiceNumber", SPFieldType.Text, 3)]
        public string InvoiceNumber { get; set; }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string FileName => MakeFileName(TravelDocId, TramsHistoryId, ProgramId);

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
            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            DataTable dt = Repository.GetProgsInSeriesByProgramId(company, year, programId);

            foreach (DataRow r in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.ProgramId, CamlComparison.Equal, r["ProgramID"].ToString());
            }

            seg.AddExpression(SPFieldNames.ProgramId, CamlComparison.Equal, programId);

            return seg;
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

            TravelDocId = Convert.ToInt32(objects[0]);
            TramsHistoryId = Convert.ToInt32(objects[1]);
            ProgramId = objects[2].ToString();
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

            if (values.ContainsKey(SPFields[SPFieldNames.TravelDocId].InternalName))
            {
                TravelDocId = (int)values[SPFields[SPFieldNames.TravelDocId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.TramsHistoryId].InternalName))
            {
                TramsHistoryId = (int)values[SPFields[SPFieldNames.TramsHistoryId].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.TravelDocId].InternalName, TravelDocId.ToString() },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.TramsHistoryId].InternalName, TramsHistoryId.ToString() },
                       { SPFields[SPFieldNames.InvoiceNumber].InternalName, InvoiceNumber }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            Preconditions.CheckNotNullOrEmpty("fileNameToParse", fileNameToParse);

            string[] fileNameParts;
            int userFieldCount = GetUserFieldCount();

            //add one to userFieldCount for the prefix text
            userFieldCount += 1;

            if (AllowedFileTypes.Count > 0)
            {
                if (!AllowedFileTypes.Contains("pdf"))
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

            int actualFieldCount = userFieldCount - 1;

            if (fileNameParts.Length != actualFieldCount)
            {
                throw new InvalidSPDocFileNameException(
                    string.Format(Resources.Default.Invalid__0__filename___1__The_filename_must_contain__2__parts_,
                        Name,
                        fileNameToParse,
                        actualFieldCount),
                    InvalidSPDocFileNameExceptionType.ElementCount);
            }

            if (fileNameParts[0] != PrefixText)
            {
                throw new InvalidSPDocFileNameException(
                    string.Format(Resources.Default.Invalid__0__filename___1__The_filename_must_begin_with__2__, Name, fileNameToParse, PrefixText),
                    InvalidSPDocFileNameExceptionType.FirstElement);
            }

            TravelDocId = Convert.ToInt32(fileNameParts[1]);
            TramsHistoryId = Convert.ToInt32(fileNameParts[2]);
            ProgramId = fileNameParts[3];

            //' Lookup invoice number from TravelDocSearch table
            InvoiceNumber = Repository.GetInvoiceNumberForTravelDocId(TravelDocId);

            return fileNameParts;
        }
    }
}
