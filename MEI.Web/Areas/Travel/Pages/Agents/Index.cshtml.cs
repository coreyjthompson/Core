using System;
using System.Collections.Generic;
using System.Linq;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Areas.Travel.Pages.Agents
{
    public class IndexModel : PageModel
    {
        private readonly IQueryProcessor _queries;

        public IndexModel(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public IList<ActiveDirectoryUser> Agents { get; set; } = new List<ActiveDirectoryUser>();

        public string AgentStatusAlertMessage { get; set; } = string.Empty;

        public string AgentStatusAlertCss { get; set; } = "alert-info";

        public NotificationViewModel Notification { get; set; }

        public void OnGet(NotificationViewModel notification)
        {
            //var query = new FindByOrganizationalUnitQuery()
            //{
            //    GroupName = "Travel"
            //};

            //var users = _queries.Execute(query).Result;

            //var query = new GetOrganizationalUnitsQuery();
            //var groups = _queries.Execute(query).Result;

            var query = new FindByGroupQuery {GroupName = "Travel"};

            Agents = _queries.Execute(query).Result;

            //Invoices = data.Select(i => new InvoiceTableItem
            //{
            //    InvoiceId = i.Id,
            //    Description = i.Description,
            //    SabreId = i.SabreId,
            //    Program = i.ProgramName,
            //    Speaker = i.SpeakerName,
            //    SubmittingUser = GetTableItemStatusCreator(i.WorkflowSteps),
            //    WhenSubmitted = GetTableItemStatusDateTime(i.WorkflowSteps),
            //    Client = i.Client.Name,
            //    Status = GetTableItemStatus(i.WorkflowSteps),
            //    Total = i.LineItems.Sum(l => l.Amount),
            //    GotoUrl = GetGotoUrl(i),
            //    StatusCssClass = GetTableItemStatusCssClass(i.WorkflowSteps)
            //}).ToList();

            Notification = notification;
        }

        private string GetGotoUrl(Invoice item)
        {
            // Check the status and find out where we should redirect
            var current = GetCurrentStatus(item.WorkflowSteps);

            switch (current.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                    return string.Format("Invoice/Edit/{0}", item.Id);
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                case (int) WorkflowStepEnum.InvoicePaid:
                    return string.Format("Invoice/View/{0}", item.Id);
                default:
                    return string.Format("Invoice/View/{0}", item.Id);
            }
        }

        private void SetStatusMessage(string action, int id)
        {
            var number = id == 0 ? "" : string.Format(" #{0}", id);

            if (action.ToLower() == "save")
            {
                AgentStatusAlertMessage = string.Format("Invoice{0} was successfully saved.", number);
                AgentStatusAlertCss = "alert-success";
            }
            else if (action.ToLower() == "submit")
            {
                AgentStatusAlertMessage = string.Format("Invoice{0} was successfully submitted.", number);
                AgentStatusAlertCss = "alert-success";
            }
            else if (action.ToLower() == "cancel")
            {
                AgentStatusAlertMessage = string.Format("Changes were cancelled for Invoice{0}.", number);
                AgentStatusAlertCss = "alert-info";
            }
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

            var query = new FindByIdentityQuery {Username = current.CreatedBy};
            var user = _queries.Execute(query).Result ?? new ActiveDirectoryUser();

            return user.DisplayName;
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
                    return "badge-warning";
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "badge-primary";
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

            public string Program { get; set; }

            public string Speaker { get; set; }

            public string SubmittingUser { get; set; }

            public DateTime? WhenSubmitted { get; set; }

            public string Client { get; set; }

            public string Status { get; set; }

            public decimal Total { get; set; }

            public string GotoUrl { get; set; }

            public string StatusCssClass { get; set; }
        }
    }
}