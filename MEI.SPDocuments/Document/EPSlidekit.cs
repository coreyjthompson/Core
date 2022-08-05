using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EPSlideKit, "EPSlidekit", "EPSK")]
    public class EPSlidekit
        : SPDocumentBase
    {
        internal EPSlidekit(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public EPSlidekit WithValues(int? slideKitId, EPassStatus statusTypeCode)
        {
            SlideKitId = slideKitId;
            Status = statusTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.SlideKitId, "SlidekitId", SPFieldType.Text, 0)]
        public int? SlideKitId { get; private set; }

        [SPFieldInfo(SPFieldNames.StatusCode, "StatusCode", SPFieldType.Text, 1)]
        public EPassStatus Status { get; private set; }

        public override string FileName => MakeFileName(SlideKitId, Status.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SlideKitId.HasValue)
                {
                    return false;
                }

                if (Status == EPassStatus.Undefined)
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

        public override string UniqueIdentifiers => "SlideKitId;Status";

        public override string UniqueValues => string.Format("{0};{1}", SlideKitId, Status.ToDisplayNameLong());

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (!SlideKitId.HasValue)
            {
                return false;
            }

            if (Status == EPassStatus.Undefined)
            {
                return false;
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

            SlideKitId = Convert.ToInt32(objects[0]);
            Status =objects[1].ToString().ToEPassStatus();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SlideKitId].InternalName))
            {
                SlideKitId = Convert.ToInt32(values[SPFields[SPFieldNames.SlideKitId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.StatusCode].InternalName))
            {
                Status = ((string)values[SPFields[SPFieldNames.StatusCode].InternalName]).ToEPassStatus();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SlideKitId].InternalName, SlideKitId.ToString() },
                       { SPFields[SPFieldNames.StatusCode].InternalName, Status.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempSlideKitId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SlideKitId, "Integer");
            }

            SlideKitId = tempSlideKitId;

            Status = fileNameParts[2].ToEPassStatus();

            if (Status == EPassStatus.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StatusCode, "EPassStatusCode");
            }

            return fileNameParts;
        }
    }
}
