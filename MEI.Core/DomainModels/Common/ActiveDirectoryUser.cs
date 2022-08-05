using System;
using System.Collections.Generic;

namespace MEI.Core.DomainModels.Common
{
    public class ActiveDirectoryUser
    {
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        public string AvatarSource { get; set; }

        public string PrincipleType { get; set; }

        public DateTime? AccountExpirationDate { get; set; }

        public DateTime? AccountLockoutTime { get; set; }

        public int BadLogonCount { get; set; }

        public string Description { get; set; }

        public string DistinguishedName { get; set; }

        public string Title { get; set; }

        public string EmployeeId { get; set; }

        public bool? Enabled { get; set; }

        public DateTime? LastBadPasswordAttempt { get; set; }

        public DateTime? LastLogon { get; set; }

        public string TelephoneNumber { get; set; }

        public string TelephoneNumberExtension { get; set; }

        public string Department { get; set; }

        public IList<string> Groups { get; set; } = new List<string>();

    }
}