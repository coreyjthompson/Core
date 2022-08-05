using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace MEI.Web.Areas.Admin.Pages.Travel
{
    public class IndexModel : PageModel
    {
        private readonly IQueryProcessor _queries;
        private readonly InfrastructureOptions _options;

        [BindProperty]
        public string SelectedGroup { get; set; }

        public IList<MEI.Core.DomainModels.Travel.AgencyService> AgencyServices { get; set; } = new List<MEI.Core.DomainModels.Travel.AgencyService>();

        public IList<MemberTableItem> Members { get; set; } = new List<MemberTableItem>();

        public IList<SelectListItem> Groups { get; set; } = new List<SelectListItem>();

        public string AgentStatusAlertMessage { get; set; } = string.Empty;

        public string AgentStatusAlertCss { get; set; } = "alert-info";

        public NotificationViewModel Notification { get; set; }

        public IndexModel(IQueryProcessor queries, IOptions<InfrastructureOptions> options)
        {
            _queries = queries;
            _options = options.Value;
        }

        public async void OnGetAsync()
        {
        }

        private IList<SelectListItem> GetGroupList(IList<string> groupNames)
        {
            var list = new List<SelectListItem>();
            // Add one empty line item
            list.Add(new SelectListItem { Value = "", Text = "" });

            // loop the list and make selectlistitems form them
            foreach (var name in groupNames)
            {
                list.Add(new SelectListItem { Value = name, Text = name });
            }

            return list;
        }

        private string GetGotoUrl(Core.DomainModels.Travel.Invoice item)
        {
            // Check the status and find out where we should redirect
            var current = GetCurrentStatus(item.WorkflowSteps);

            switch (current.WorkflowStepId)
            {
                case (int)WorkflowStepEnum.InvoiceDraftSaved:
                    return string.Format("Invoice/Edit/{0}", item.Id);
                case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                case (int)WorkflowStepEnum.InvoicePaid:
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
            var current = steps.FirstOrDefault(s => s.WorkflowStepId == (int)WorkflowStepEnum.InvoicePaid);
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

            var query = new FindByIdentityQuery { Username = current.CreatedBy };
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
                case (int)WorkflowStepEnum.InvoiceDraftSaved:
                    return "Draft";
                case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "Submitted";
                case (int)WorkflowStepEnum.InvoicePaid:
                    return "Paid";
                case (int)WorkflowStepEnum.None:
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
                case (int)WorkflowStepEnum.InvoiceDraftSaved:
                    return "badge-warning";
                case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "badge-primary";
                case (int)WorkflowStepEnum.InvoicePaid:
                    return "badge-success";
                case (int)WorkflowStepEnum.None:
                    return "badge-light";
                default:
                    return "badge-primary";

            }
        }

        public class MemberTableItem
        {
            public MemberTableItem()
            {
            }

            public MemberTableItem(ActiveDirectoryUser ad, string defaultAvatarPath)
            {
                DisplayName = ad.DisplayName;
                EmailAddress = ad.EmailAddress;
                FirstName = ad.FirstName;
                LastName = ad.LastName;
                Avatar = GetAvatar(ad, defaultAvatarPath);
                AccountExpirationDate = ad.AccountExpirationDate;
                AccountLockoutTime = ad.AccountLockoutTime;
                BadLogonCount = ad.BadLogonCount;
                Description = ad.Description;
                DistinguishedName = ad.DistinguishedName;
                EmployeeId = ad.EmployeeId;
                Enabled = ad.Enabled;
                LastBadPasswordAttempt = ad.LastBadPasswordAttempt;
                LastLogon = ad.LastLogon;
                TelephoneNumber = ad.TelephoneNumber;
                PrincipleType = ad.PrincipleType;
                Department = ad.Department;
                Title = ad.Title;
            }

            public string DisplayName { get; set; }

            public string Department { get; set; } 

            public string EmailAddress { get; set; }

            public string Title { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Avatar { get; set; }

            public string PrincipleType { get; set; }

            public DateTime? AccountExpirationDate { get; set; }

            public DateTime? AccountLockoutTime { get; set; }

            public int BadLogonCount { get; set; }

            public string Description { get; set; }

            public string DistinguishedName { get; set; }
            public string EmployeeId { get; set; }

            public bool? Enabled { get; set; }

            public DateTime? LastBadPasswordAttempt { get; set; }

            public DateTime? LastLogon { get; set; }

            public string TelephoneNumber { get; set; }

            private string GetFirstTwoInitials(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return string.Empty;
                }

                if (name.ToLower() == "pj smith")
                {
                    return "PJ";
                }

                var intials = string.Empty;
                var arr = name.Trim().Split(' ');

                if (arr.Length > 1)
                {
                    // We want a max of 3 initials
                    for (int i = 0; i < (Math.Min(3, arr.Length)); i++)
                    {
                        if (string.IsNullOrEmpty(arr[i]))
                        {
                            return name;
                        }

                        intials += arr[i].Trim().Substring(0, 1).ToUpper();

                    }
                }

                if (arr.Length == 1)
                {
                    if (string.IsNullOrEmpty(arr[0]))
                    {
                        return name;
                    }

                    if (arr[0].Length > 1)
                    {
                        intials += arr[0].Trim().Substring(0, 2).ToUpper();
                    }
                    else
                    {
                        intials += arr[0].Trim().Substring(0, 1).ToUpper();
                    }
                }

                return intials;
            }

            private string GetAvatar(ActiveDirectoryUser ad, string pathToReplace)
            {
                var html = string.Empty;

                if (string.IsNullOrEmpty(ad.AvatarSource) || ad.AvatarSource == pathToReplace)
                {
                    var fullName = string.Format("{0} {1}", ad.FirstName?.Trim(), ad.LastName?.Trim()).Trim();

                    if (string.IsNullOrEmpty(fullName))
                    {
                        fullName = ad.DisplayName;
                    }

                    var initials = GetFirstTwoInitials(fullName);
                    html = string.Format("<div class=\"user-avatar avatar-initials {1} p-1\"><div class=\"initials\" ><b>{0}</b></div></div>", initials, GetRandomIntialsCssClass());
                }
                else
                {
                    html = string.Format("<div class=\"user-avatar p-1\"><img src='{0}' alt=\"Avatar\"></div>", ad.AvatarSource);
                    //html = string.Format("<img src=\"{0}\" alt=\"Avatar\">", ad.AvatarSource);
                }

                return html;
            }

            private string GetRandomIntialsCssClass()
            {
                // Set up the list of bg theme css classes
                var list = new List<string> { "avatar-initials-primary", "avatar-initials-warning", "avatar-initials-secondary", "avatar-initials-dark", "avatar-initials-danger", "avatar-initials-info", "avatar-initials-success" };

                // Get a random item from the list
                var random = new Random();
                var index = random.Next(list.Count);

                return list[index];
            }

        }


    }
}
