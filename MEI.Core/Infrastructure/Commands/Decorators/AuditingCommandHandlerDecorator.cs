using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Services;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using NodaTime;
using NodaTime.Extensions;

namespace MEI.Core.Infrastructure.Commands.Decorators
{
    public class AuditingCommandHandlerDecorator<TCommand, TResult>
        : ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        private readonly IClock _clock;
        private readonly IConfiguration _config;
        private readonly CoreContext _db;
        private readonly ICommandHandler<TCommand, TResult> _handler;
        private readonly IUserResolverService _userResolverService;

        public AuditingCommandHandlerDecorator(ICommandHandler<TCommand, TResult> handler, IUserResolverService userResolverService, CoreContext db, IClock clock, IConfiguration config)
        {
            _handler = handler;
            _userResolverService = userResolverService;
            _db = db;
            _clock = clock;
            _config = config;
        }

        public async Task<TResult> HandleAsync(TCommand command)
        {
            var result = await _handler.HandleAsync(command);
            await AppendToAuditTrail(command);

            return result;
        }

        private async Task<int> AppendToAuditTrail(TCommand command)
        {
            string environment = null;

            if (_config["ApplicationOptions:Environment"] != null)
            {
                environment = _config["ApplicationOptions:Environment"];
            }

            if (environment == null && _config["Environment"] != null)
            {
                environment = _config["Environment"];
            }

            string appName = null;

            if (_config["ApplicationOptions:AppName"] != null)
            {
                appName = _config["ApplicationOptions:AppName"];
            }

            if (appName == null && _config["AppName"] != null)
            {
                appName = _config["AppName"];
            }

            var entry = new AuditEntry
            {
                WhenExecuted = _clock.InTzdbSystemDefaultZone().GetCurrentZonedDateTime().ToDateTimeOffset(),
                UserName = _userResolverService.GetUserName(),
                Operation = command.GetType().Name,
                Data = JsonConvert.SerializeObject(command),
                AppName = appName,
                MachineName = Environment.MachineName,
                Environment = environment
            };

            await _db.AuditEntries.AddAsync(entry);

            return await _db.SaveChangesAsync();
        }
    }
}