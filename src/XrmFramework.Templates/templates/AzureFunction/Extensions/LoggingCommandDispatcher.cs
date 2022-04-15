using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace $safeprojectname$.Extensions
{
    public class LoggingCommandDispatcher : ICommandDispatcher
    {
        private readonly IFrameworkCommandDispatcher _underlyingDispatcher;
        private readonly ILogger<LoggingCommandDispatcher> _logger;

        public LoggingCommandDispatcher(
            IFrameworkCommandDispatcher underlyingDispatcher,
            ILogger<LoggingCommandDispatcher> logger)
        {
            _underlyingDispatcher = underlyingDispatcher;
            _logger = logger;
        }

        public ICommandExecuter AssociatedExecuter => null;

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Received payload { command.GetType().Name } : { JsonConvert.SerializeObject(command) }");

            var result = await _underlyingDispatcher.DispatchAsync(command, cancellationToken);

            _logger.LogInformation($"Result sent : { JsonConvert.SerializeObject(result.Result) }");

            return result;
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Received payload { command.GetType().Name } : { JsonConvert.SerializeObject(command) }");

            return await _underlyingDispatcher.DispatchAsync(command, cancellationToken);
        }
    }
}
