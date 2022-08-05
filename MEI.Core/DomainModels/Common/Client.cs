using System;

namespace MEI.Core.DomainModels.Common
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StreetAddressLine1 { get; set; }

        public string StreetAddressLine2 { get; set; }

        public string CityTown { get; set; }

        public string StateProvince { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string WebsiteUrl { get; set; }

        public string TollFreePhoneNumber { get; set; }

        public string Initials { get; set; }

        public string SharePointCompanyCode { get; set; }

        public DateTimeOffset WhenCreated { get; set; }

        public DateTimeOffset? WhenDeactivated { get; set; }
    }
}