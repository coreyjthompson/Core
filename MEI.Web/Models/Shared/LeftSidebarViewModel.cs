using System;

namespace MEI.Web.Models.Shared
{
    public class LeftSidebarViewModel
    {
        public string CurrentlySelectedLinkTitle { get; set; } = string.Empty;

        public string GetCss(string currentLinkText)
        {
            return string.Equals(CurrentlySelectedLinkTitle, currentLinkText, StringComparison.CurrentCultureIgnoreCase) ? "active" : string.Empty;
        }
    }
}
