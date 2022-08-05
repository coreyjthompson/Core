using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Travel.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Areas.Travel.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        private readonly IQueryProcessor _queries;

        public IndexModel(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public IList<InvoiceTableItem> Invoices { get; set; } = new List<InvoiceTableItem>();

        public string InvoiceStatusAlertMessage { get; set; } = string.Empty;

        public string InvoiceStatusAlertCss { get; set; } = "alert-info";

        public IList<object> TableInitialSortOrder
        {
            get
            {
                return new List<object>
                {
                    new {field = "Status", direction = "Ascending"}, 
                    new {field = "WhenSubmitted", direction = "Descending"}
                };
            }
        }

        public NotificationViewModel Notification { get; set; }

        public async Task OnGetAsync(NotificationViewModel notification)
        {
            var query = new GetAllInvoicesQuery();

            var data = await _queries.Execute(query);

            Invoices = data.Select(
                i => new InvoiceTableItem
                {
                    InvoiceId = i.Id,
                    SabreId = i.SabreInvoiceId,
                    EventName = i.EventName,
                    ConsultantName = i.ConsultantName,
                    LastModifiedBy = GetTableItemStatusCreator(i.WorkflowSteps),
                    WhenSubmitted = GetTableItemStatusDateTime(i.WorkflowSteps),
                    Client = i.Client.Name,
                    Status = GetTableItemStatus(i.WorkflowSteps),
                    Total = i.LineItems.Sum(l => l.Amount),
                    GotoUrl = GetGotoUrl(i),
                    StatusCssClass = GetTableItemStatusCssClass(i.WorkflowSteps)
                }
            ).ToList();

            Notification = notification;
        }

        private string GetGotoUrl(Invoice item)
        {
            if(item != null)
            {
                // Check the status and find out where we should redirect
                var current = GetCurrentStatus(item.WorkflowSteps);
                if(current != null)
                {
                    switch (current.WorkflowStepId)
                    {
                        case (int) WorkflowStepEnum.InvoiceDraftSaved:
                        case (int) WorkflowStepEnum.InvoiceCreated:
                            return  Url.Content(string.Format("~/Travel/Invoices/Edit/{0}", item.Id));
                        case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                        case (int) WorkflowStepEnum.InvoicePaid:
                            return  Url.Content(string.Format("~/Travel/Invoices/View/{0}", item.Id));
                        default:
                            return  Url.Content(string.Format("~/Travel/Invoices/View/{0}", item.Id));
                    }
                }

                return  Url.Content(string.Format("~/Travel/Invoices/View/{0}", item.Id));
            }

            return  Url.Content("~/Travel/Invoices");

        }

        private DateTime? GetTableItemStatusDateTime(IList<InvoiceWorkflowStatus> steps)
        {
            if (steps == null || !steps.Any())
            {
                return null;
            }

            var current = GetCurrentStatus(steps);

            return current?.WhenCreated.DateTime;
        }

        private InvoiceWorkflowStatus GetCurrentStatus(IList<InvoiceWorkflowStatus> steps)
        {
            if (steps == null || !steps.Any())
            {
                return null;
            }

            // Precedence 1 - if invoice was ever paid show it as such
            var current = steps.FirstOrDefault(s => s.WorkflowStepId == (int) WorkflowStepEnum.InvoicePaid);

            if (current != null)
            {
                return current;
            }

            // Precedence 2 - if invoice was ever submitted show it as such
            current = steps.FirstOrDefault(s => s.WorkflowStepId == (int) WorkflowStepEnum.InvoiceSubmittedForPayment);

            if (current != null)
            {
                return current;
            }

            return steps.OrderByDescending(s => s.Id).FirstOrDefault();
        }

        private string GetTableItemStatusCreator(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(current.CreatedBy))
            {
                var query = new FindByIdentityQuery { Username = current.CreatedBy };
                var user = _queries.Execute(query).Result ?? new ActiveDirectoryUser();

                return user.DisplayName;
            }

            return "Unknown user";
        }

        private string GetTableItemStatus(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            switch (current.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                case (int) WorkflowStepEnum.InvoiceCreated:
                    return "Draft";
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "Submitted";
                case (int) WorkflowStepEnum.InvoicePaid:
                    return "Paid";
                case (int) WorkflowStepEnum.None:
                    return "Undefined";
                default:
                    return "Submitted";
            }
        }

        private string GetTableItemStatusCssClass(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            switch (current.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                case (int) WorkflowStepEnum.InvoiceCreated:
                    return "badge-warning";
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "badge-info";
                case (int) WorkflowStepEnum.InvoicePaid:
                    return "badge-success";
                case (int) WorkflowStepEnum.None:
                    return "badge-light";
                default:
                    return "badge-primary";
            }
        }

        public class InvoiceTableItem
        {
            public int InvoiceId { get; set; }

            public string Description { get; set; }

            public string SabreId { get; set; }

            public string EventName { get; set; }

            public string ConsultantName { get; set; }

            public string LastModifiedBy { get; set; }

            public DateTime? WhenSubmitted { get; set; }

            public string LastModifiedDate 
            { 
                get 
                {
                    if (WhenSubmitted != null)
                    {
                        return WhenSubmitted.Value.ToString("MM/dd/yyyy");
                    }

                    return "Unknown";
                } 
            }

            public string Client { get; set; }

            public string Status { get; set; }

            public decimal Total { get; set; }

            public string GotoUrl { get; set; }

            public string StatusCssClass { get; set; }
        }
    }
}