using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Logem
{
    public class LogemHub
    {
        public ReadOnlyCollection<ILogger> Loggers => loggerConfigurations.Select(x => x.Logger).ToList().AsReadOnly();

        public ReadOnlyCollection<LoggerConfiguration> LoggerConfigurations => loggerConfigurations.ToList().AsReadOnly();

        private List<LoggerConfiguration> loggerConfigurations = new List<LoggerConfiguration>();

        public void AddLogger(ILogger logger)
        {
            AddLogger(logger, null);
        }

        public void AddLogger(ILogger logger, params string[] categories)
        {
            loggerConfigurations.Add(new LoggerConfiguration(logger, categories));
        }

        public async Task LogAsync(string message = null, object data = null, string category = null)
        {
            IEnumerable<ILogger> loggers;

            loggers = loggerConfigurations
                .Where(x => string.IsNullOrWhiteSpace(category) || x.Categories.Contains(category))
                .Select(x => x.Logger);

            foreach (var logger in loggers)
            {
                await logger.LogAsync(message, data);
            }
        }
    }
}
