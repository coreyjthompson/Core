using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MEI.Core.Commands
{
    public interface ICommandProcessor
    {
        Task<TResult> Execute<TResult>(ICommand<TResult> command);
    }

    public class DynamicCommandProcessor
        : ICommandProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicCommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerStepThrough]
        public async Task<TResult> Execute<TResult>(ICommand<TResult> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);

            return await handler.HandleAsync((dynamic) command);
        }
    }
}