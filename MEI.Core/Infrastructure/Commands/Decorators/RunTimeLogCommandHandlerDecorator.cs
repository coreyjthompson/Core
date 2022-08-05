using System.Threading.Tasks;

using MEI.Core.Infrastructure;

using Microsoft.Extensions.Logging;

namespace MEI.Core.Commands.Decorators
{
    public class RunTimeLogCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _decorated;
        private readonly ILogger<RunTimeLogCommandHandlerDecorator<TCommand, TResult>> _logger;
        private readonly IStopwatch _stopwatch;

        public RunTimeLogCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated, ILogger<RunTimeLogCommandHandlerDecorator<TCommand, TResult>> logger, IStopwatch stopwatch)
        {
            _decorated = decorated;
            _logger = logger;
            _stopwatch = stopwatch;
        }

        public async Task<TResult> HandleAsync(TCommand command)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            var result = await _decorated.HandleAsync(command);

            _stopwatch.Stop();

            _logger.LogTrace(string.Format("{0}:{1}:{2}", command.GetType().Name, command, _stopwatch.ElapsedDuration()));

            return result;
        }
    }
}