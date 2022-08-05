using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace MEI.Web.Areas.Travel.Models
{
    public class AgencyServiceViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FeeAmount { get; set; }

        public int FeeCurrencyId { get; set; }

        public virtual Currency FeeCurrency { get; set; }

        public int SortOrder { get; set; }

    }
}