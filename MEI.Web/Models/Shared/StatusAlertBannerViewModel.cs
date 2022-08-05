using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MEI.Web.Models.Shared
{
    public class StatusAlertBannerViewModel
    {
        private readonly Tuple<ActiveDirectoryUser, InvoiceWorkflowStatus> _currentStatus;
        private readonly string _currentUser;

        public StatusAlertBannerViewModel(IList<Tuple<ActiveDirectoryUser, InvoiceWorkflowStatus>> tuples, string currentUser)
        {
            _currentStatus = GetCurrentStatus(tuples);
            _currentUser = currentUser;
            for (int i = 0; i < tuples.Count(); i++)
            {
                HistoryItems.Add(new HistoryItem(tuples[i].Item1, tuples[i].Item2, _currentUser, i));
            }
        }

        public IList<HistoryItem> HistoryItems { get; set; } = new List<HistoryItem>();

        public string BannerHeading => GetStatusHeading();

        public string BannerCssClass => GetStatusCssClass();

        public string BannerMessage => GetStatusMessage();

        private string GetStatusCssClass()
        {
            if (_currentStatus == null)
            {
                return string.Empty;
            }

            switch (_currentStatus.Item2.WorkflowStepId)
            {
                case (int)WorkflowStepEnum.InvoiceCreated:
                case (int)WorkflowStepEnum.InvoiceDraftSaved:
                    return "alert-dark";
                case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return "alert-primary";
                case (int)WorkflowStepEnum.InvoicePaid:
                    return "alert-success";
                case (int)WorkflowStepEnum.None:
                    return "alert-light";
                default:
                    return "alert-light";

            }
        }

        private Tuple<ActiveDirectoryUser, InvoiceWorkflowStatus> GetCurrentStatus(IList<Tuple<ActiveDirectoryUser, InvoiceWorkflowStatus>> steps)
        {
            if (steps == null || !steps.Any())
            {
                return null;
            }

            var workflowSteps = steps.Select(t => t.Item2).ToList();
            if (workflowSteps == null || !steps.Any())
            {
                return null;
            }

            // Precedence 1 - if invoice was ever paid show it as such
            var current = workflowSteps.FirstOrDefault(s => s.WorkflowStepId == (int)WorkflowStepEnum.InvoicePaid);
            if (current == null)
            {
                // Precedence 2 - if invoice was ever submitted show it as such
                current = workflowSteps.FirstOrDefault(s => s.WorkflowStepId == (int)WorkflowStepEnum.InvoiceSubmittedForPayment);
                if (current == null)
                {
                    current = workflowSteps.OrderByDescending(s => s.Id).FirstOrDefault();
                }
            }
            
            foreach (var item in steps)
            {
                if(item.Item2 == current)
                {
                    return item;
                }
            }

            return null;
        }

        private string GetStatusHeading()
        {
            if (_currentStatus == null)
            {
                return string.Empty;
            }

            switch (_currentStatus.Item2.WorkflowStepId)
            {
                case (int)WorkflowStepEnum.InvoiceCreated:
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

        private string GetStatusMessage()
        {
            if (_currentStatus == null)
            {
                return string.Empty;
            }

            var creator = "";
            if (_currentUser.ToLower() == _currentStatus.Item2.CreatedBy.ToLower())
            {
                creator = "You";
            }

            switch (_currentStatus.Item2.WorkflowStepId)
            {
                case (int)WorkflowStepEnum.InvoiceCreated:
                    return string.Format("{1} created this invoice on {0}", _currentStatus.Item2.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                case (int)WorkflowStepEnum.InvoiceDraftSaved:
                    return string.Format("{1} saved this invoice on {0}", _currentStatus.Item2.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                    return string.Format("{1} submitted this invoice on {0}", _currentStatus.Item2.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                case (int)WorkflowStepEnum.InvoicePaid:
                    return string.Format("This invoice was submitted for payment on {0}");
                case (int)WorkflowStepEnum.None:
                    return string.Format("This invoice was submitted for payment on {0}");
                default:
                    return string.Format("This invoice was submitted for payment on {0}");

            }
        }


        public class HistoryItem
        {
            private readonly ActiveDirectoryUser _statusOwner;
            private readonly InvoiceWorkflowStatus _status;
            private readonly string _currentUser;
            private readonly int _statusIndex;

            public HistoryItem(ActiveDirectoryUser statusOwner, InvoiceWorkflowStatus status, string currentUser, int statusIndex)
            {
                _statusOwner = statusOwner;
                _status = status;
                _currentUser = currentUser;
                _statusIndex = statusIndex;
            }

            public int StatusId => _status.Id;

            public string OwnerAvatar => GetAvatar(_statusOwner);

            public string ActionTaken => GetActionTaken();

            public string GetActionTaken()
            {
                var creator = _statusOwner.DisplayName;

                if (_currentUser.ToLower() == _status.CreatedBy.ToLower())
                {
                    creator = "You";
                }

                switch (_status.WorkflowStepId)
                {
                    case (int)WorkflowStepEnum.InvoiceCreated:
                        return string.Format("{1} created this invoice on {0}", _status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                    case (int)WorkflowStepEnum.InvoiceDraftSaved:
                        return string.Format("{1} saved this invoice on {0}", _status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                    case (int)WorkflowStepEnum.InvoiceSubmittedForPayment:
                        return string.Format("{1} submitted this invoice for payment on {0}", _status.WhenCreated.DateTime.ToString("MM/dd/yyyy"), creator);
                    default:
                        return string.Format("There was an error in this item");
                }
            }

            private string GetAvatar(ActiveDirectoryUser ad)
            {
                var html = string.Empty;

                if (string.IsNullOrEmpty(ad.AvatarSource))
                {
                    var fullName = string.Format("{0} {1}", ad.FirstName?.Trim(), ad.LastName?.Trim()).Trim();

                    if (string.IsNullOrEmpty(fullName))
                    {
                        fullName = ad.DisplayName;
                    }

                    var initials = GetFirstTwoInitials(fullName);
                    html = string.Format("<div class=\"user-avatar avatar-initials {1} p-1\"><div class=\"initials\" ><b>{0}</b></div></div>", initials, "avatar-initials-dark");
                }
                else
                {
                    html = string.Format("<div class=\"user-avatar p-1\"><img src='{0}' alt=\"Avatar\"></div>", ad.AvatarSource);
                }

                return html;
            }

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

        }
    }
}
