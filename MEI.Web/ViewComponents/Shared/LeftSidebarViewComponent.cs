using System.Threading.Tasks;

using MEI.Web.Models.Shared;

using Microsoft.AspNetCore.Mvc;

namespace MEI.Web.ViewComponents.Shared
{
    public class LeftSidebarViewComponent : ViewComponent
    {
        public LeftSidebarViewModel LeftSidebarViewModel { get; set; } = new LeftSidebarViewModel();

        public async Task<IViewComponentResult> InvokeAsync(string currentlySelectedLinkText)
        {
            LeftSidebarViewModel.CurrentlySelectedLinkTitle = currentlySelectedLinkText;

            return View(LeftSidebarViewModel);
        }
    }
}