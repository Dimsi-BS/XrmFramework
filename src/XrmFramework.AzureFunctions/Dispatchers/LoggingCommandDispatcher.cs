using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace XrmFramework.AzureFunctions
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
            var sw = Stopwatch.StartNew();

            _logger.LogInformation($"Received payload { command.GetType().Name } : { JsonConvert.SerializeObject(command) }");

            var result = await _underlyingDispatcher.DispatchAsync(command, cancellationToken);
            
            sw.Stop();
            _logger.LogInformation($"Result sent in ({sw.Elapsed}) : { JsonConvert.SerializeObject(result.Result) }");

            return result;
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            _logger.LogInformation($"Received payload { command.GetType().Name } : { JsonConvert.SerializeObject(command) }");

            var result = await _underlyingDispatcher.DispatchAsync(command, cancellationToken);

            sw.Stop();
            _logger.LogInformation($"Result sent in ({sw.Elapsed})");

            return result;
        }
    }
}
