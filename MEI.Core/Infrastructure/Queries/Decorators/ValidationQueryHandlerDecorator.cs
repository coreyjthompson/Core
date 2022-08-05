using System;
using System.Threading.Tasks;

using MEI.Core.Validation;

namespace MEI.Core.Infrastructure.Queries.Decorators
{
    public class ValidationQueryHandlerDecorator<TQuery, TResult>
        : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _handler;
        private readonly IValidator _validator;

        public ValidationQueryHandlerDecorator(IValidator validator, IQueryHandler<TQuery, TResult> handler)
        {
            _validator = validator;
            _handler = handler;
        }

        public Task<TResult> HandleAsync(TQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            _validator.ValidateObject(query);

            return _handler.HandleAsync(query);
        }
    }
}