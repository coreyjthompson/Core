using System.Collections.Generic;

using MEI.Core.DomainModels.Common;

namespace MEI.Web.Models.AdminLog
{
    public class ListViewModel
    {
        public IList<Log> Logs { get; set; }
    }
}
