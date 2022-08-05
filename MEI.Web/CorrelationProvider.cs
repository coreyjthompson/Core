using MEI.Logging;

using Microsoft.AspNetCore.Http;

namespace MEI.Web
{
    public class CorrelationProvider
        : ICorrelationProvider
    {
        private readonly IHttpContextAccessor _context;

        public CorrelationProvider(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetCorrelationId()
        {
            return _context?.HttpContext?.TraceIdentifier;
        }
    }
}
