using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace MEI.Web.Areas.Travel.Models
{
    public class InvoiceFormViewModel
    {
        public int? InvoiceId { get; set; }

        public string InvoiceNumber => InvoiceId?.ToString().PadLeft(7, '0');

        public string Description { get; set; } = "Travel Booking Fees";

        public string SabreInvoiceId { get; set; }

        public int? EventId { get; set; }

        public string EventName { get; set; }

        public int? ConsultantId { get; set; }

        public string ConsultantName { get; set; }

        public string ClientName { get; set; }

        public string SubmittingUser { get; set; }

        public string FormAction { get; set; }

        public string PageAction { get; set; }

        public string CurrentStatusHeading => GetStatusHeading(WorkflowSteps);

        public string CurrentStatusCssClass => GetStatusCssClass(WorkflowSteps);

        public string CurrentStatusMessage => GetStatusMessage(WorkflowSteps);

        public string SubmissionDate => GetSubmissionDate(WorkflowSteps);

        public string TotalAmount { get; set; }

        public IList<string> HistorySteps { get; set; } = new List<string>();

        public IList<InvoiceWorkflowStatus> WorkflowSteps { get; set; } = new List<InvoiceWorkflowStatus>();


        public virtual IList<LineItem> LineItems { get; set; } = new List<LineItem>();

        public IList<SelectListItem> Events { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> Consultants { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> Clients { get; set; } = new List<SelectListItem>();

        public IList<SelectListItem> TravelServices { get; set; } = new List<SelectListItem>();

        private string GetStatusCssClass(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            switch (current.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                    return "alert-dark";
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "alert-primary";
                case (int) WorkflowStepEnum.InvoicePaid:
                    return "alert-success";
                case (int) WorkflowStepEnum.None:
                    return "alert-light";
                default:
                    return "alert-light";
            }
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

        private string GetStatusHeading(IList<InvoiceWorkflowStatus> steps)
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

        private string GetStatusMessage(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            switch (current.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                    return string.Format("This invoice was last saved on {0}", current.WhenCreated.DateTime.ToString("MM/dd/yyyy"));
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return string.Format("This invoice was submitted for payment on {0}", current.WhenCreated.DateTime.ToString("MM/dd/yyyy"));
                case (int) WorkflowStepEnum.InvoicePaid:
                    return string.Format("This invoice was submitted for payment on {0}");
                case (int) WorkflowStepEnum.None:
                    return string.Format("This invoice was submitted for payment on {0}");
                default:
                    return string.Format("This invoice was submitted for payment on {0}");
            }
        }

        public NotificationViewModel GetNotification()
        {
            var notification = new NotificationViewModel();

            if (FormAction != "cancel")
            {
                if (FormAction == "save")
                {
                    notification.Content = $"Your changes were saved to Invoice #{InvoiceId.ToString()}.";
                }
                else if (FormAction == "submit")
                {
                    notification.Content = $"Invoice #{InvoiceId.ToString()} was submitted.";
                }

                notification.Title = "Success!";
                notification.Css = "e-toast-success";
                notification.Icon = "d-none";
            }
            else
            {
                notification.Title = "Cancelled";
                notification.Content = $"No changes were made to Invoice #{InvoiceId.ToString()}";
                notification.Css = "e-toast-info";
                notification.Icon = "d-none";
            }

            ;

            return notification;
        }

        private string GetSubmissionDate(IList<InvoiceWorkflowStatus> steps)
        {
            var current = GetCurrentStatus(steps);

            if (current == null)
            {
                return string.Empty;
            }

            return current.WhenCreated.DateTime.ToString("MM/dd/yyyy");
        }

        public IList<string> GetHistorySteps(IList<InvoiceWorkflowStatus> steps, string currentUser)
        {
            var list = new List<string>();
            var index = 0;

            foreach (var step in steps)
            {
                list.Add(GetHistoryDescription(step, currentUser, index));
                index++;
            }

            return list;
        }

        private string GetHistoryDescription(InvoiceWorkflowStatus status, string currentUser, int index)
        {
            var creator = GetCleanUsername(status.CreatedBy);

            if (creator.ToLower() == GetCleanUsername(currentUser.ToLower()))
            {
                creator = "You";
            }

            if (index == 0)
            {
                return string.Format("{1} created this invoice on {0}", status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
            }

            switch (status.WorkflowStepId)
            {
                case (int) WorkflowStepEnum.InvoiceDraftSaved:
                    return string.Format("{1} saved this invoice on {0}", status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                case (int) WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return string.Format("{1} submitted this invoice for payment on {0}", status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                default:
                    return "There was an error";
            }
        }

        private string GetCleanUsername(string username)
        {
            return username.Replace("MEIDOMAIN1\\", "");
        }

        public class LineItem
        {
            public int Id { get; set; }

            public long Quantity { get; set; } = 1;

            public decimal Amount { get; set; }

            public int InvoiceId { get; set; }

            [Required] 
            public int TravelServiceId { get; set; }

            public AgencyService TravelService { get; set; }

            [Required] 
            public string AmountAsString { get; set; }

            public string TravelServiceName { get; set; } = string.Empty;

            public string ServiceCssClass => LineItemError.IsTravelServiceInvalid ? "show" : "hide";

            public string AmountCssClass => LineItemError.IsAmountInvalid ? "show" : "hide";

            public LineItemValidationError LineItemError { get; set; } = new LineItemValidationError();
        }

        public class LineItemValidationError
        {
            public int? LineIndex { get; set; }

            public bool IsAmountInvalid { get; set; }

            public string AmountValidationMessage { get; set; }

            public bool IsTravelServiceInvalid { get; set; }

            public string TravelServiceValidationMessage { get; set; }
        }

        public class HistoryItem
        {
            public string DateTime { get; set; }

            //public string Description { get; set; }

            public string User { get; set; }
        }
    }
}