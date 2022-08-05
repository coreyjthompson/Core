using System;
using System.Xml;

namespace MEI.SPDocuments.SPActionResult
{
    public class SearchFieldVersionsResult
    {
        public SearchFieldVersionsResult(string fieldName, string fieldValue, DateTime modified)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
            Modified = modified;
        }

        public SearchFieldVersionsResult(XmlNode node, string fieldName)
        {
            Preconditions.CheckNotNull("node", node);

            FieldName = fieldName;
            ParseNode(node);
        }

        public string FieldName { get; }

        public string FieldValue { get; private set; }

        public DateTime Modified { get; private set; }

        private void ParseNode(XmlNode node)
        {
            Preconditions.CheckNotNull("node", node);

            FieldValue = node.Attributes?[FieldName].Value;

            if (DateTime.TryParse(node.Attributes?["Modified"].Value, out DateTime tempDateTime))
            {
                Modified = tempDateTime;
            }
        }

        public override string ToString()
        {
            return string.Format("FieldName={0}, FieldValue={1}, Modified={2}]", FieldName, FieldValue, Modified);
        }
    }
}
