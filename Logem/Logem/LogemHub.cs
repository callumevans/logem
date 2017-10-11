using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logem
{
    public class LogemHub
    {
        public List<LoggerConfiguration> LoggerConfigurations => this.loggerConfigurations;

        private List<LoggerConfiguration> loggerConfigurations = new List<LoggerConfiguration>();

        public void AddLogger(ILogger logger)
        {
            AddLogger(logger, null);
        }

        public void AddLogger(ILogger logger, params string[] categories)
        {
            loggerConfigurations.Add(new LoggerConfiguration(logger, categories));
        }

        public async Task LogAsync(string message, object data = null, string category = null)
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
