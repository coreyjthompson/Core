using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.AdhocSlidekit, "AdhocSlidekit", "AHSK", "AHSK", "Adhoc Slidekit")]
    public class AdhocSlidekit
        : SPDocumentBase, ISearchSpeaker, ISearchProgram
    {
        internal AdhocSlidekit(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public AdhocSlidekit WithValues(int? adhocSlideKitId, string uploadFileName, DateTime? uploadDate, int? pifId)
        {
            AdHocSlideKitId = adhocSlideKitId;
            UploadFileName = uploadFileName;
            UploadDate = uploadDate;
            PifId = pifId;

            return this;
        }

        public override DocumentYear DocumentYear
        {
            get
            {
                if (!UploadDate.HasValue)
                {
                    return DocumentYear.Undefined;
                }

                return UploadDate.Value.Year.ToString().ToDocumentYear();
            }
        }

        [SPFieldInfo(SPFieldNames.AdHocSlideKitId, "AdhocSlidekitId", SPFieldType.Text, 0)]
        public int? AdHocSlideKitId { get; private set; }

        [SPFieldInfo(SPFieldNames.UploadFileName, "UploadFileName", SPFieldType.Text, 1)]
        public string UploadFileName { get; private set; }

        [SPFieldInfo(SPFieldNames.UploadDate, "UploadDate", SPFieldType.DateTime, 2)]
        public DateTime? UploadDate { get; private set; }

        [SPFieldInfo(SPFieldNames.PifId, "PifId", SPFieldType.Text, 3)]
        public int? PifId { get; private set; }

        public override string FileName => MakeFileName(AdHocSlideKitId, UploadFileName, PifId);

        public override string ParsableFileName =>
            MakeFileName(AdHocSlideKitId.ToString(),
                UploadFileName,
                UploadDate == null ? string.Empty : UploadDate.Value.ToString("yyyy-MM-dd_HH-mm-ss"),
                PifId.ToString());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!AdHocSlideKitId.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(UploadFileName))
                {
                    return false;
                }

                if (!UploadDate.HasValue)
                {
                    return false;
                }

                if (!PifId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "ppt",
                "pptx",
                "doc",
                "docx"
            };

        public override string UniqueIdentifiers => "AdHocSlideKit;UploadFileName";

        public override string UniqueValues => string.Format("{0};{1}", AdHocSlideKitId, UploadFileName);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            DataTable dt = Repository.GetAdhocSlidekitIdsByProgramId(company, year, programId);

            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.AdHocSlideKitId,
                    CamlComparison.Equal,
                    dr["AdhocSlidekitId"] == DBNull.Value ? null : (int?)dr["AdhocSlidekitId"]);
            }

            return seg;
        }

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            DataTable dt = Repository.GetAdhocSlidekitIdsBySpeakerCounter(company, year, speakerCounter);

            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.AdHocSlideKitId,
                    CamlComparison.Equal,
                    dr["AdhocSlidekitID"] == DBNull.Value ? null : (int?)dr["AdhocSlidekitID"]);
            }

            return seg;
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            //TODO: Make PIF ID validator
            //TODO: Make AdHoc Slide Kit ID Validator

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

            if (objects[0] != null)
            {
                AdHocSlideKitId = Convert.ToInt32(objects[0]);
            }

            UploadFileName = objects[1].ToString();

            if (objects[2] != null)
            {
                UploadDate = Convert.ToDateTime(objects[2]);
            }

            if (objects[3] != null)
            {
                PifId = Convert.ToInt32(objects[3]);
            }

            Contents = (byte[])objects[4];
            FileExtension = objects[5].ToString();
            Company = (Company)objects[6];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.UploadFileName].InternalName))
            {
                UploadFileName = (string)values[SPFields[SPFieldNames.UploadFileName].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.UploadDate].InternalName))
            {
                UploadDate = Convert.ToDateTime(values[SPFields[SPFieldNames.UploadDate].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.PifId].InternalName))
            {
                PifId = (int)values[SPFields[SPFieldNames.PifId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.AdHocSlideKitId].InternalName))
            {
                AdHocSlideKitId = (int)values[SPFields[SPFieldNames.AdHocSlideKitId].InternalName];
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.AdHocSlideKitId].InternalName, AdHocSlideKitId.ToString() },
                       { SPFields[SPFieldNames.UploadFileName].InternalName, UploadFileName },
                       { SPFields[SPFieldNames.UploadDate].InternalName, UploadDate.ToString() },
                       { SPFields[SPFieldNames.PifId].InternalName, PifId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempAdhocSlideKitId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.AdHocSlideKitId, "Integer");
            }

            AdHocSlideKitId = tempAdhocSlideKitId;

            UploadFileName = fileNameParts[2];

            if (!DateTime.TryParseExact(fileNameParts[3],
                "yyyy-MM-dd_HH-mm-ss",
                new CultureInfo("en-US"),
                DateTimeStyles.None,
                out DateTime tempUploadDate))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.UploadDate, "DateTime");
            }

            UploadDate = tempUploadDate;

            if (!string.IsNullOrEmpty(fileNameParts[4]))
            {
                if (!int.TryParse(fileNameParts[4], out int tempPifId))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.PifId, "Integer");
                }

                PifId = tempPifId;
            }

            return fileNameParts;
        }
    }
}
