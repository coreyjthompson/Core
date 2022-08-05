using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentEnabled(false)]
    [DocumentInfo(SPDocumentType.DebarmentCheck, "DebarmentCheck", "DC", "DC", "Debarment Check")]
    public class DebarmentCheck
        : SPDocumentBase, ISearchSpeaker
    {
        internal DebarmentCheck(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public DebarmentCheck WithValues(string site,
                               bool? isFound,
                               int? speakerCounter,
                               string siteLocation,
                               DateTime? searchDateTime,
                               string searchSpeakerFirstName,
                               string searchSpeakerLastName,
                               string searchValue)
        {
            Found = isFound;
            Site = site;
            SiteLocation = siteLocation;
            SpeakerCounter = speakerCounter;
            SearchDateTime = searchDateTime;
            SearchValue = searchValue;
            SpeakerFirstName = searchSpeakerFirstName;
            SpeakerLastName = searchSpeakerLastName;

            return this;
        }

        public override DocumentYear DocumentYear
        {
            get
            {
                if (!SearchDateTime.HasValue)
                {
                    return DocumentYear.Undefined;
                }

                return SearchDateTime.Value.Year.ToString().ToDocumentYear();
            }
        }

        [SPFieldInfo(SPFieldNames.SearchDateTime, "SearchDateTime", SPFieldType.Text, 4)]
        public DateTime? SearchDateTime { get; private set; }

        [SPFieldInfo(SPFieldNames.SearchValue, "SearchValue", SPFieldType.Text, 7)]
        public string SearchValue { get; private set; }

        [SPFieldInfo(SPFieldNames.Site, "Site", SPFieldType.Text, 0)]
        public string Site { get; private set; }

        [SPFieldInfo(SPFieldNames.SiteLocation, "SiteLocation", SPFieldType.Text, 1)]
        public string SiteLocation { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 3)]
        public int? SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerFirstName, "SpeakerFirstName", SPFieldType.Text, 5)]
        public string SpeakerFirstName { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerLastName, "SpeakerLastName", SPFieldType.Text, 6)]
        public string SpeakerLastName { get; private set; }

        [SPFieldInfo(SPFieldNames.Found, "Found", SPFieldType.Text, 2)]
        public bool? Found { get; private set; }

        public override string FileName =>
            MakeFileName(Site,
                SiteLocation,
                Found,
                SpeakerCounter,
                SearchDateTime == null ? "" : SearchDateTime.Value.ToString("yyyyMMddHHmmss"));

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(Site))
                {
                    return false;
                }

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(SiteLocation))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "Site;SiteLocation;Found;SpeakerCounter;SearchDateTime";

        public override string UniqueValues =>
            string.Format("{0};{1};{2};{3};{4}", Site, SiteLocation, Found, SpeakerCounter, SearchDateTime);

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter);
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
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

            Site = objects[0].ToString();
            SiteLocation = objects[1].ToString();
            Found = Convert.ToBoolean(objects[2]);
            SpeakerCounter = Convert.ToInt32(objects[3]);
            SearchDateTime = Convert.ToDateTime(objects[4]);
            Contents = (byte[])objects[5];
            FileExtension = objects[6].ToString();
            Company = (Company)objects[7];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = (int)values[SPFields[SPFieldNames.SpeakerCounter].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SiteLocation].InternalName))
            {
                SiteLocation = (string)values[SPFields[SPFieldNames.SiteLocation].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Site].InternalName))
            {
                Site = (string)values[SPFields[SPFieldNames.Site].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SearchDateTime].InternalName))
            {
                SearchDateTime = (DateTime)values[SPFields[SPFieldNames.SearchDateTime].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Found].InternalName))
            {
                Found = (bool)values[SPFields[SPFieldNames.Found].InternalName];
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            var fields = new Dictionary<string, string>
                         {
                             { SPFields[SPFieldNames.Site].InternalName, Site },
                             { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                             { SPFields[SPFieldNames.SiteLocation].InternalName, SiteLocation },
                             { SPFields[SPFieldNames.SearchDateTime].InternalName, SearchDateTime.ToString() },
                             { SPFields[SPFieldNames.SpeakerFirstName].InternalName, SpeakerFirstName },
                             { SPFields[SPFieldNames.SpeakerLastName].InternalName, SpeakerLastName },
                             { SPFields[SPFieldNames.SearchValue].InternalName, SearchValue },
                             { SPFields[SPFieldNames.Found].InternalName, Found.HasValue ? "1" : "0" }
                         };

            return fields;
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            Site = fileNameParts[1];
            SiteLocation = fileNameParts[2];

            if (!bool.TryParse(fileNameParts[3], out bool tempFound))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.Found, "Boolean");
            }

            Found = tempFound;

            if (!int.TryParse(fileNameParts[4], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            if (!DateTime.TryParse(string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    fileNameParts[5].Substring(0, 4),
                    fileNameParts[5].Substring(4, 2),
                    fileNameParts[5].Substring(6, 2),
                    fileNameParts[5].Substring(8, 2),
                    fileNameParts[5].Substring(10, 2),
                    fileNameParts[5].Substring(12, 2)),
                out DateTime tempSearchDateTime))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SearchDateTime, "DateTime");
            }

            SearchDateTime = tempSearchDateTime;

            return fileNameParts;
        }
    }
}
