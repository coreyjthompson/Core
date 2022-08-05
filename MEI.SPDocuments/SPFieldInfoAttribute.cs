using System;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class SPFieldInfoAttribute
        : Attribute
    {
        public SPFieldInfoAttribute(SPFieldNames enumValue, string internalName, SPFieldType fieldType)
        {
            EnumValue = enumValue;
            InternalName = internalName;
            FieldType = fieldType;
            DisplayName = InternalName;
        }

        public SPFieldInfoAttribute(SPFieldNames enumValue, string internalName, SPFieldType fieldType, int getFileIndex)
        {
            EnumValue = enumValue;
            InternalName = internalName;
            FieldType = fieldType;
            DisplayName = InternalName;
            GetFileIndex = getFileIndex;
        }

        public SPFieldInfoAttribute(SPFieldNames enumValue, string internalName, SPFieldType fieldType, string displayName)
        {
            EnumValue = enumValue;
            InternalName = internalName;
            DisplayName = displayName;
            FieldType = fieldType;
        }

        public SPFieldInfoAttribute(SPFieldNames enumValue, string internalName, SPFieldType fieldType, string displayName, int getFileIndex)
        {
            EnumValue = enumValue;
            InternalName = internalName;
            DisplayName = displayName;
            FieldType = fieldType;
            GetFileIndex = getFileIndex;
        }

        public SPFieldNames EnumValue { get; }

        public string InternalName { get; }

        public string DisplayName { get; }

        public SPFieldType FieldType { get; }

        public int? GetFileIndex { get; }
    }
}
