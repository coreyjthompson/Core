using System;
using System.Diagnostics;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;

namespace MEI.Core.Queries
{
    public interface IQueryProcessor
    {
        Task<TResult> Execute<TResult>(IQuery<TResult> query);
    }

    public class DynamicQueryProcessor
        : IQueryProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicQueryProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerStepThrough]
        public async Task<TResult> Execute<TResult>(IQuery<TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);

            return await handler.HandleAsync((dynamic) query);
        }
    }
}