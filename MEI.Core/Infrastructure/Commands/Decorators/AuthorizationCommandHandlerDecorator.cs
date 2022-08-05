using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MEI.Core.Commands.Decorators
{
    public class AuthorizationCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly IPrincipal _currentUser;
        private readonly ICommandHandler<TCommand, TResult> _handler;
        private readonly ILogger _logger;

        public AuthorizationCommandHandlerDecorator(ICommandHandler<TCommand, TResult> handler, IPrincipal currentUser, ILogger logger)
        {
            _handler = handler;
            _currentUser = currentUser;
            _logger = logger;
        }

        public Task<TResult> HandleAsync(TCommand command)
        {
            Authorize();

            return _handler.HandleAsync(command);
        }

        private void Authorize()
        {
            var ns = typeof(TCommand).Namespace;

            if (ns?.Contains("Admin") == true && !_currentUser.IsInRole("Admin"))
            {
                throw new SecurityException();
            }

            _logger.LogInformation("User " + _currentUser.Identity.Name + " has been authorized to execute " + typeof(TCommand).Name);
        }
    }
}