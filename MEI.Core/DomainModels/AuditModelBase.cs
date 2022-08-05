namespace MEI.Core.DomainModels
{
    public class AuditModelBase
        : ICorrelationId
    {
        public string CorrelationId { get; set; }
    }
}
