using System;
using System.Threading.Tasks;

using MEI.Core.Validation;

namespace MEI.Core.Commands.Decorators
{
    public class ValidationCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _handler;
        private readonly IValidator _validator;

        public ValidationCommandHandlerDecorator(IValidator validator, ICommandHandler<TCommand, TResult> handler)
        {
            _validator = validator;
            _handler = handler;
        }

        public Task<TResult> HandleAsync(TCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _validator.ValidateObject(command);

            return _handler.HandleAsync(command);
        }
    }
}