using System.ComponentModel.DataAnnotations.Schema;

namespace MEI.Core.DomainModels.Travel
{
    public class InvoiceLineItem
    {
        public int Id { get; set; }

        public long Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int InvoiceId { get; set; }

        public int AgencyServiceId { get; set; }

        public virtual AgencyService AgencyService { get; set; } 
    }
}