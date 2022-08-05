using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SpeakerNomination, "SpeakerNomination", "SN", "SpeakerNomination", "Speaker Nomination")]
    public class SpeakerNomination
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal SpeakerNomination(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public SpeakerNomination WithValues(int? speakerNominationId, int? speakerCounter, DocumentYear documentYear)
        {
            SpeakerCounter = speakerCounter;
            SpeakerNominationId = speakerNominationId;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text, 1)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.SpeakerNominationId, "SpeakerNominationID", SPFieldType.Text, 0)]
        public int? SpeakerNominationId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 2)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(SpeakerNominationId, DocumentYear.ToDisplayNameLong());

        public override string ParsableFileName => MakeFileName(SpeakerNominationId.ToString(), SpeakerCounter.ToString(), DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SpeakerNominationId.HasValue)
                {
                    return false;

                    //SpeakerCounter is optional
                }

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SpeakerNominationId;DocumentYear";

        public override string UniqueValues => string.Format("{0};{1}", SpeakerNominationId, DocumentYear.ToProgramIdYear());

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            var seg = new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            DataTable dt = Repository.GetSNNominationIdsBySpeakerCounter(company, year, speakerCounter);

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.SpeakerNominationId, CamlComparison.Equal, DbUtilities.FromDbValue<int>(dr["ID"]));
            }

            return seg;

            // NOTE: Below is a fix for a logic bug that will be put in after we fix the data it relates to
            //Dim speakerCounterSegment As New SearchExpressionGroup(SPDocumentTypeCode.SpeakerNomination, SearchBooleanLogicCode.And)

            //speakerCounterSegment.AddExpression(SpeakerNominationFieldName.SpeakerCounter, CamlComparisonCode.Equal, speakerCounterToSearch)

            //Dim idsSegment As New SearchExpressionGroup(SPDocumentTypeCode.SpeakerNomination, SearchBooleanLogicCode.Or)

            //Dim dt As DataTable = _infoHelper.GetSNNominationIdsBySpeakerCounter(currentCompany, currentYear, speakerCounterToSearch)

            //If dt.Rows.Count = 0 Then
            //    Return Nothing
            //End If

            //For Each dr As DataRow In dt.Rows
            //    idsSegment.AddExpression(SpeakerNominationFieldName.SpeakerNominationId, CamlComparisonCode.Equal, DbUtilities.FromDbValue(Of Integer)(dr("ID")))
            //Next

            //idsSegment.AttachExpressionGroup(speakerCounterSegment, SearchBooleanLogicCode.And)
            //Return idsSegment
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

            if (SpeakerNominationId != null && Repository.GetSpeakerNominationIdsBySpeakerNominationId(Company, DocumentYear, SpeakerNominationId.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerNominationId, SpeakerNominationId.Value.ToString());
            }

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
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

            SpeakerNominationId = Convert.ToInt32(objects[0]);
            if (objects[1] != null)
            {
                SpeakerCounter = Convert.ToInt32(objects[1]);
            }

            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerNominationId].InternalName))
            {
                SpeakerNominationId = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerNominationId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = ((string)values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SpeakerNominationId].InternalName, SpeakerNominationId.ToString() },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempSpeakerNominationId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerNominationId, "Integer");
            }

            SpeakerNominationId = tempSpeakerNominationId;

            if (!string.IsNullOrEmpty(fileNameParts[2]))
            {
                if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
                }

                SpeakerCounter = tempSpeakerCounter;
            }

            string tempDocumentYear = fileNameParts[3];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }
    }
}
