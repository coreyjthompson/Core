using System.Threading.Tasks;

namespace MEI.Core.Commands.Decorators
{
    public sealed class PostCommitCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _decorated;
        private readonly IPostCommitRegistrator _registrator;

        public PostCommitCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated, IPostCommitRegistrator registrator)
        {
            _decorated = decorated;
            _registrator = registrator;
        }

        public async Task<TResult> HandleAsync(TCommand command)
        {
            try
            {
                var result = await _decorated.HandleAsync(command);

                _registrator.ExecuteActions();

                return result;
            }
            finally
            {
                _registrator.Reset();
            }
        }
    }
}