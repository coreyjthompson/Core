using MEI.Core.Infrastructure.Services;

using Microsoft.AspNetCore.Http;

namespace MEI.Web
{
    public class UserResolverService
        : IUserResolverService
    {
        private readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUserName()
        {
            return _context?.HttpContext?.User?.Identity?.Name;
        }
    }
}
