using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models.Shared;

using Microsoft.AspNetCore.Mvc;

namespace MEI.Web.ViewComponents.Shared
{
    public class StatusAlertBannerViewComponent : ViewComponent
    {
        private readonly Core.Queries.IQueryProcessor _queries;

        public StatusAlertBannerViewComponent(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public StatusAlertBannerViewModel StatusAlertBanner { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(IList<InvoiceWorkflowStatus> workflowSteps)
        {
            var currentUser = User.Identity.Name;
            var tuples = new List<Tuple<ActiveDirectoryUser, InvoiceWorkflowStatus>>();
            workflowSteps = workflowSteps.OrderByDescending(s => s.WhenCreated).ToList();

            foreach(var step in workflowSteps)
            {
                var query = new FindByIdentityQuery
                {
                    Username = step.CreatedBy
                };

                var owner = await _queries.Execute(query);
                tuples.Add(Tuple.Create(owner, step));
            }

            StatusAlertBanner = new StatusAlertBannerViewModel(tuples, currentUser);

            return View(StatusAlertBanner);
        }
    }
}