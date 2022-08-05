using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.MSPASecondAddressAmendmentProof, "MSPASecondAddressAmendmentProof", "SAAP", "SAAP")]
    public class MSPASecondAddressAmendmentProof
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal MSPASecondAddressAmendmentProof(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public MSPASecondAddressAmendmentProof WithValues(DocumentYear documentYear, int secondAddressAmendmentId, int speakerCounter, int? proofIndex, MEIApprovedSelection meiApproved, FieldOpsApprovedSelection fieldOpsApproved)
        {
            DocumentYear = documentYear;
            SecondAddressAmendmentId = secondAddressAmendmentId;
            SpeakerCounter = speakerCounter;
            ProofIndex = proofIndex;
            MEIApproved = meiApproved;
            FieldOpsApproved = fieldOpsApproved;

            // potential todo:  Increment proofIndex in this constructor instead of the Get of the property?

            return this;
        }

        [SPFieldInfo(SPFieldNames.SecondAddressAmendmentId, "SecondAddressAmendmentId", SPFieldType.Integer, 0)]
        public int SecondAddressAmendmentId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Integer, 1)]
        public int SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProofIndex, "ProofIndex", SPFieldType.Integer, 2)]
        public int? ProofIndex { get; private set; }

        [SPFieldInfo(SPFieldNames.MEIApproved, "MEIApproved", SPFieldType.Choice, 3)]
        public MEIApprovedSelection MEIApproved { get; private set; }

        [SPFieldInfo(SPFieldNames.FieldOpsApproved, "FieldOpsApproved", SPFieldType.Choice, 4)]
        public FieldOpsApprovedSelection FieldOpsApproved { get; private set; }

        public override string FileName => MakeFileName(SpeakerCounter, SecondAddressAmendmentId, ProofIndex, DocumentYear.ToDisplayNameLong());

        public override string ParsableFileName =>
            MakeFileName(SpeakerCounter,
                SecondAddressAmendmentId,
                ProofIndex,
                MEIApproved,
                FieldOpsApproved,
                DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (SpeakerCounter == 0)
                {
                    return false;
                }

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                if (SecondAddressAmendmentId == 0)
                {
                    return false;

                    // ElseIf ProofIndex Is Nothing OrElse ProofIndex = 0 Then
                    //     Return False
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => throw new NotImplementedException();

        public override string UniqueValues => throw new NotImplementedException();

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text)]
        public override DocumentYear DocumentYear { get; internal set; }

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

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Equal, year.ToDisplayNameLong());
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

            SecondAddressAmendmentId = Convert.ToInt32(objects[0]);
            if (objects[1] != null)
            {
                SpeakerCounter = Convert.ToInt32(objects[1]);
            }

            if (objects[2] != null)
            {
                ProofIndex = Convert.ToInt32(objects[2]);
            }

            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.ToString());
            }

            if (Repository.GetSecondAddressAmendmentProofById(Company, DocumentYear, SecondAddressAmendmentId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SecondAddressAmendmentId, SecondAddressAmendmentId.ToString());
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SecondAddressAmendmentId].InternalName, SecondAddressAmendmentId.ToString() },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.ProofIndex].InternalName, ProofIndex.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() },
                       { SPFields[SPFieldNames.MEIApproved].InternalName, MEIApproved.ToString() },
                       { SPFields[SPFieldNames.FieldOpsApproved].InternalName, FieldOpsApproved.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!string.IsNullOrEmpty(fileNameParts[1]))
            {
                if (!int.TryParse(fileNameParts[1], out int tempSpeakerCounter))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
                }

                SpeakerCounter = tempSpeakerCounter;
            }

            if (!string.IsNullOrEmpty(fileNameParts[2]))
            {
                if (!int.TryParse(fileNameParts[2], out int tempSecondAddressAmendmentProofId))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SecondAddressAmendmentId, "Integer");
                }

                SecondAddressAmendmentId = tempSecondAddressAmendmentProofId;
            }

            if (!string.IsNullOrEmpty(fileNameParts[4]))
            {
                try
                {
                    MEIApproved = (MEIApprovedSelection)Enum.Parse(typeof(MEIApprovedSelection), fileNameParts[4]);
                }
                catch (Exception)
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.MEIApproved, "MEIApprovedSelection");
                }
            }

            if (!string.IsNullOrEmpty(fileNameParts[5]))
            {
                try
                {
                    FieldOpsApproved = (FieldOpsApprovedSelection)Enum.Parse(typeof(FieldOpsApprovedSelection), fileNameParts[5]);
                }
                catch (Exception)
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.FieldOpsApproved, "FieldOpsApprovedSelection");
                }
            }

            string tempDocumentYear = fileNameParts[6];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }

        public override void ActionBeforeUpload(IDocumentBroker documentBroker)
        {
            var searchExpressions = new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, SpeakerCounter);

            searchExpressions.And(SPFieldNames.SecondAddressAmendmentId, CamlComparison.Equal, SecondAddressAmendmentId)
                .And(SPFieldNames.DocumentYear, CamlComparison.Equal, DocumentYear.ToDisplayNameLong());

            IList<SearchResult> resultsOfSearch = documentBroker.Search(SPDocumentType.MSPASecondAddressAmendmentProof, Company, searchExpressions);

            var currentPoofIndex = 0;

            foreach (SearchResult item in resultsOfSearch)
            {
                string proofIndex = item.UserFields[SPFields[SPFieldNames.ProofIndex].InternalName];

                if (Convert.ToInt32(proofIndex) > currentPoofIndex)
                {
                    currentPoofIndex = Convert.ToInt32(proofIndex);
                }
            }

            ProofIndex = currentPoofIndex + 1;
        }
    }
}
