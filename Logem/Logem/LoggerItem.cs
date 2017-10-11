namespace Logem
{
    public class LoggerConfiguration
    {
        public ILogger Logger { get; private set; }

        public string[] Categories { get; private set; }

        public LoggerConfiguration(ILogger logger, string[] categories)
        {
            this.Logger = logger;
            this.Categories = categories;
        }
    }
}
