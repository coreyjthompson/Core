using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MEI.Core.DomainModels.Common;

namespace MEI.Core.DomainModels.Travel
{
    public class AgencyService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FeeAmount { get; set; }

        public int FeeCurrencyId { get; set; }

        public virtual Currency FeeCurrency { get; set; }

        public int SortOrder { get; set; }

        public DateTimeOffset? WhenInactivated { get;set; }
    }
}