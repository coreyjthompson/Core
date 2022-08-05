using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MEI.Core.Infrastructure.Queries.Decorators
{
    public class AuthorizationQueryHandlerDecorator<TQuery, TResult>
        : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IPrincipal _currentUser;
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly ILogger<AuthorizationQueryHandlerDecorator<TQuery, TResult>> _logger;

        public AuthorizationQueryHandlerDecorator(IQueryHandler<TQuery, TResult> handler, IPrincipal currentUser, ILogger<AuthorizationQueryHandlerDecorator<TQuery, TResult>> logger)
        {
            _handler = handler;
            _currentUser = currentUser;
            _logger = logger;
        }

        public Task<TResult> HandleAsync(TQuery query)
        {
            Authorize();

            return _handler.HandleAsync(query);
        }

        private void Authorize()
        {
            var ns = typeof(TQuery).Namespace;

            if (ns?.Contains("Admin") == true && !_currentUser.IsInRole("Admin"))
            {
                throw new SecurityException();
            }

            _logger.LogError("User " + _currentUser.Identity.Name + " has been authorized to execute " + typeof(TQuery).Name);
        }
    }
}