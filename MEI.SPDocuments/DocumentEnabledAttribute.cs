using System;

namespace MEI.SPDocuments
{
    internal class DocumentEnabledAttribute
        : Attribute
    {
        public DocumentEnabledAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }
    }
}
