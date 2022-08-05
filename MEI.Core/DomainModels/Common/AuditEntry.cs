using System;

namespace MEI.Core.DomainModels.Common
{
    public class AuditEntry
        : ICorrelationId
    {
        public int Id { get; set; }

        public DateTimeOffset WhenExecuted { get; set; }

        public string UserName { get; set; }

        public string Operation { get; set; }

        public string Data { get; set; }

        public string AppName { get; set; }

        public string MachineName { get; set; }

        public string Environment { get; set; }

        public string CorrelationId { get; set; }
    }
}
