using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PW9, "PW9")]
    public class PW9
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal PW9(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PW9 WithValues(int? participantCounter, TinType tinType, DocumentYear documentYear)
        {
            ParticipantCounter = participantCounter;
            TinType = tinType;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "Year", SPFieldType.Text, 1)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.ParticipantCounter, "ParticipantCounter", SPFieldType.Text, 0)]
        public int? ParticipantCounter { get; private set; }

        [SuppressMessage("Microsoft.Naming",
            "CA1702:CompoundWordsShouldBeCasedCorrectly",
            MessageId = "TinType",
            Justification = "Tin is a descrete word. (acronym for tax identification number).")]
        [SPFieldInfo(SPFieldNames.TinType, "TINSSN", SPFieldType.Choice, 2)]
        public TinType TinType { get; private set; }

        public override string FileName => MakeFileName(ParticipantCounter, DocumentYear.ToDisplayNameLong(), TinType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!ParticipantCounter.HasValue)
                {
                    return false;
                }

                if (TinType == TinType.Undefined)
                {
                    return false;
                }

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ParticipantCounter;DocumentYear;TinType";

        public override string UniqueValues =>
            string.Format("{0};{1};{2}", ParticipantCounter, DocumentYear.ToProgramIdYear(), TinType.ToDisplayNameShort());

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            //NOTE: For every company except Kowa ...
            //      This always returns an error because the stored procedure (usp_GetParticipantCounterByProgramID) does not exist.
            //      In databases where the stored procedure does exist (e.g. SQL4, Kowa2017) it points to a column (tblAttendees.ParticipantCounter)
            //      that does not exist in the Abbott database
            //      So dont return results unless the current company is Kowa
            if (company == Company.Kowa)
            {
                foreach (DataRow dr in Repository.GetParticipantCounterByProgramId(company, year, programId).Rows)
                {
                    seg.AddExpression(SPFieldNames.ParticipantCounter, CamlComparison.Equal, dr["participantCounter"].ToString());
                }
            }

            return seg;
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year)
        {
            if (year == DocumentYear.Undefined)
            {
                return new SearchExpressionGroup(this);
            }

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Equal, year.ToDisplayNameLong());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (ParticipantCounter != null && Repository.GetParticipantCounters(Company, DocumentYear, ParticipantCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ParticipantCounter, ParticipantCounter.Value.ToString());
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

            ParticipantCounter = Convert.ToInt32(objects[0]);
            DocumentYear = objects[1].ToString().ToDocumentYear();
            TinType = objects[2].ToString().ToTinType();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ParticipantCounter].InternalName))
            {
                ParticipantCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ParticipantCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.TinType].InternalName))
            {
                TinType = ((string)values[SPFields[SPFieldNames.TinType].InternalName]).ToTinType();
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = ((string)values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ParticipantCounter].InternalName, ParticipantCounter.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() },
                       { SPFields[SPFieldNames.TinType].InternalName, TinType.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempParticipantCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ParticipantCounter, "Integer");
            }

            ParticipantCounter = tempParticipantCounter;

            TinType = fileNameParts[3].ToTinType();

            if (TinType == TinType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.TinType, "TinTypeCode");
            }

            DocumentYear = fileNameParts[2].ToDocumentYear();

            if (DocumentYear == DocumentYear.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DocumentYear, "DocumentYear");
            }

            return fileNameParts;
        }
    }
}
