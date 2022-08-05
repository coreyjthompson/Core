using System.Collections.Generic;

using MEI.Core.DomainModels.Common;

namespace MEI.Core.Infrastructure.Services
{
    public interface ILdapService
    {
        ActiveDirectoryUser FindByIdentity(string username);

        IList<string> GetAllOrganizationalUnits();

        List<string> FindByOrganizationalUnit(string organizationalUnit);
    }
}