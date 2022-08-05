using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentEnabled(false)]
    [DocumentInfo(SPDocumentType.HotelConfirmationCancellation,
        "HotelConfirmationCancellation",
        "HCC",
        "HCC",
        "Hotel Confirmation Cancellation")]
    public class HotelConfirmationCancellation
        : SPDocumentBase, ISearchSpeaker, ISearchProgram, ISearchYear
    {
        internal HotelConfirmationCancellation(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public HotelConfirmationCancellation WithValues(string programId, int? speakerCounter, int? spSeriesId)
        {
            ProgramId = programId;
            SpeakerCounter = speakerCounter;
            SPSeriesId = spSeriesId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "ProgramID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.SpSeriesId, "SPSeriesId", SPFieldType.Text, 2)]
        public int? SPSeriesId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SpeakerCounter, SPSeriesId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (!SPSeriesId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;SpeakerCounter;SPSeriesId";

        public override string UniqueValues => string.Format("{0};{1};{2}", ProgramId, SpeakerCounter, SPSeriesId);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter);
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

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter,
                    SpeakerCounter.Value.ToString());
            }

            //TODO: make validator for SPSeriesId
            //If Not Validator.ValidateSeriesId(Company, DocumentYear, SPSeriesId.Value) Then
            //	ThrowFileNameExceptionNoDBMatch(SPFieldNames.SPSeriesId, SPSeriesId.Value.ToString())
            //End If

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
            SpeakerCounter = Convert.ToInt32(objects[1].ToString());
            SPSeriesId = Convert.ToInt32(objects[2].ToString());
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

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpSeriesId].InternalName))
            {
                SPSeriesId = Convert.ToInt32(values[SPFields[SPFieldNames.SpSeriesId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.SpSeriesId].InternalName, SPSeriesId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            if (!int.TryParse(fileNameParts[3], out int tempSPSeriesId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpSeriesId, "Integer");
            }

            SPSeriesId = tempSPSeriesId;

            return fileNameParts;
        }
    }
}
