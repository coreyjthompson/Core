using System;
using System.ComponentModel.DataAnnotations;

namespace MEI.Core.DomainModels.Common
{
    public class Log
        : ICorrelationId
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string MachineName { get; set; }

        public DateTimeOffset WhenLogged { get; set; }

        [MaxLength(5)]
        public string Level { get; set; }

        public string Message { get; set; }

        [MaxLength(300)]
        public string Logger { get; set; }

        public string Properties { get; set; }

        [MaxLength(300)]
        public string Callsite { get; set; }

        public string Exception { get; set; }

        [MaxLength(200)]
        public string Environment { get; set; }

        [MaxLength(500)]
        public string AppName { get; set; }

        public string CorrelationId { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}, Level={1}, WhenLogged={2}, AppName{3}, Environment={4}", Id, Level, WhenLogged, AppName, Environment);
        }
    }
}