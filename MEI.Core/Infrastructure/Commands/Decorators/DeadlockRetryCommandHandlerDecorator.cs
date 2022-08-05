using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MEI.Core.Commands.Decorators
{
    public class DeadlockRetryCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _decorated;

        public DeadlockRetryCommandHandlerDecorator(ICommandHandler<TCommand, TResult> decorated)
        {
            _decorated = decorated;
        }

        public async Task<TResult> HandleAsync(TCommand command)
        {
            return await HandleWithRetry(command, 5);
        }

        private Task<TResult> HandleWithRetry(TCommand command, int retries)
        {
            try
            {
                return _decorated.HandleAsync(command);
            }
            catch (Exception ex)
            {
                if (retries <= 0 || !IsDeadlockException(ex))
                {
                    throw;
                }

                Thread.Sleep(300);

                return HandleWithRetry(command, retries - 1);
            }
        }

        private static bool IsDeadlockException(Exception ex)
        {
            return ex is DbException && ex.Message.Contains("deadlock") || ex.InnerException != null
                   && IsDeadlockException(ex.InnerException);
        }
    }
}