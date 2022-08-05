using System;
using System.Collections.Generic;
using System.Linq;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Clients.Queries;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly IQueryProcessor _queries;

        public IList<ClientTableItem> Clients { get; set; } = new List<ClientTableItem>();

        public NotificationViewModel Notification { get; set; }

        public IList<ClientCard> Cards { get; set; } = new List<ClientCard>();

        public IndexModel(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public void OnGet(NotificationViewModel notification)
        {
            var query = new GetAllClientsQuery();

            var data = _queries.Execute(query).Result;
            Cards = data.Select(i => new ClientCard()
            {
                Id = i.Id,
                Name = i.Name,
                Status = GetTableItemStatus(i),
                StatusCssClass = GetTableItemStatusCssClass(i),
                StreetAddressLine1 = i.StreetAddressLine1,
                StreetAddressLine2 = i.StreetAddressLine2,
                CityTown = i.CityTown,
                StateProvince = i.StateProvince,
                Country = i.Country,
                PostalCode = i.PostalCode,
                TollFreePhoneNumber = i.TollFreePhoneNumber,
                GoToUrl = GetGotoUrl(i),
                Initials = GetClientInitials(i.Name),
                CssClass = GetCardCssClass()
            }).ToList();

            Notification = notification;
        }

        private string GetClientInitials(string clientName)
        {
            switch (clientName.ToLower())
            {
                case "abbvie":
                    return "AB";
                case "exact sciences":
                    return "ES";
                case "mei":
                    return "MEI";
                default:
                    return GetFirstTwoInitials(clientName);
            }
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

        private string GetGotoUrl(Core.DomainModels.Common.Client item)
        {
            return string.Format("Client/Edit/{0}", item.Id);
        }
        
        private string GetTableItemStatus(Core.DomainModels.Common.Client item)
        {
            switch (item.WhenDeactivated == null)
            {
                case true:
                    return "Deactivated";
                case false:
                    return "Active";
            }
        }

        private string GetTableItemStatusCssClass(Core.DomainModels.Common.Client item)
        {
            switch (item.WhenDeactivated == null)
            {
                case true:
                    return "badge-danger";
                case false:
                    return "badge-success";
            }
        }

        private string GetCardCssClass()
        {
            // Set up the list of bg theme css classes
            var list = new List<string> { "card-initials-primary", "card-initials-warning", "card-initials-secondary", "card-initials-dark", "card-initials-danger", "card-initials-info", "card-initials-success" };

            // Get a random item from the list
            var random = new Random();
            var index = random.Next(list.Count);

            return list[index];
        }

        public class ClientTableItem
        {
            public int Id { get; set; }

            public string Name { get; set; }
            
            public string Status { get; set; }

            public string StatusCssClass { get; set; }

            public string Initials { get; set; }

            public string GoToUrl { get; set; }

            public string StreetAddressLine1 { get; set; }

            public string StreetAddressLine2 { get; set; }

            public string CityTown { get; set; }

            public string StateProvince { get; set; }

            public string PostalCode { get; set; }

            public string Country { get; set; }

            public string WebsiteUrl { get; set; }

            public string TollFreePhoneNumber { get; set; }

            public string CardCssClass { get; set; } = string.Empty;


            public DateTimeOffset WhenCreated { get; set; }

            public DateTimeOffset? WhenDeactivated { get; set; }

        }

        public class ClientCard
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Status { get; set; }

            public string StatusCssClass { get; set; }

            public string Initials { get; set; }

            public string GoToUrl { get; set; }

            public string StreetAddressLine1 { get; set; }

            public string StreetAddressLine2 { get; set; }

            public string CityTown { get; set; }

            public string StateProvince { get; set; }

            public string PostalCode { get; set; }

            public string Country { get; set; }

            public string WebsiteUrl { get; set; }

            public string TollFreePhoneNumber { get; set; }

            public string CssClass { get; set; } = string.Empty;

            public DateTimeOffset WhenCreated { get; set; }

            public DateTimeOffset? WhenDeactivated { get; set; }

        }
    }
}
