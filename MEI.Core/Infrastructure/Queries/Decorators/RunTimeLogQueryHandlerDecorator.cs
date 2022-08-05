using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MEI.Core.Infrastructure.Queries.Decorators
{
    public class RunTimeLogQueryHandlerDecorator<TQuery, TResult>
        : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _decorated;
        private readonly ILogger<RunTimeLogQueryHandlerDecorator<TQuery, TResult>> _logger;
        private readonly IStopwatch _stopwatch;

        public RunTimeLogQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decorated, ILogger<RunTimeLogQueryHandlerDecorator<TQuery, TResult>> logger, IStopwatch stopwatch)
        {
            _decorated = decorated;
            _logger = logger;
            _stopwatch = stopwatch;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            var result = await _decorated.HandleAsync(query);

            _stopwatch.Stop();

            _logger.LogTrace(string.Format("{0}:{1}:{2}", query.GetType().Name, query, _stopwatch.ElapsedDuration()));

            return result;
        }
    }
}