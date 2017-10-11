using System.Threading.Tasks;
using Xunit;

namespace Logem.Tests
{
    public class HubTests
    {
        private class FakeLogger : ILogger
        {
            public bool HasBeenCalled { get; private set; }

            public (string Message, object Data) CalledWith { get; private set; }

            public Task LogAsync(string log, object data)
            {
                HasBeenCalled = true;
                CalledWith = (log, data);

                return Task.FromResult(0);
            }
        }

        static string testMessage = "Hello world!";

        static object testObject = new
        {
            Data = "Test data!"
        };

        const string CategoryA = "categoryA";
        const string CategoryB = "categoryB";
        const string CategoryC = "categoryC";
        const string CategoryD = "categoryD";

        [Fact]
        public async Task ExecuteLog_CallsLoggerWithParameters()
        {
            // Arrange
            var logem = new LogemHub();
            var logger = new FakeLogger();

            logem.AddLogger(logger);

            Assert.False(logger.HasBeenCalled);

            // Act
            await logem.LogAsync(testMessage);

            // Assert
            Assert.True(logger.HasBeenCalled);
            Assert.Equal(testMessage, logger.CalledWith.Message);
            Assert.Null(logger.CalledWith.Data);
        }

        [Fact]
        public async Task ExecuteLog_WithoutMessage_CallsLoggerWithNoParameters()
        {
            // Arrange
            var logem = new LogemHub();
            var logger = new FakeLogger();

            logem.AddLogger(logger);

            Assert.False(logger.HasBeenCalled);

            // Act
            await logem.LogAsync();

            // Assert
            Assert.True(logger.HasBeenCalled);
            Assert.Null(logger.CalledWith.Message);
            Assert.Null(logger.CalledWith.Data);
        }

        [Fact]
        public async Task ExecuteLog_WithData_CallsLoggerWithParameters()
        {
            // Arrange
            var logem = new LogemHub();
            var logger = new FakeLogger();

            logem.AddLogger(logger);

            Assert.False(logger.HasBeenCalled);

            // Act
            await logem.LogAsync(testMessage, testObject);

            // Assert
            Assert.True(logger.HasBeenCalled);
            Assert.Equal(testMessage, logger.CalledWith.Message);
            Assert.Equal(testObject, logger.CalledWith.Data);
        }

        [Fact]
        public async Task ExecuteLog_MultipleLoggers_CallsAllLoggers()
        {
            // Arrange
            var logem = new LogemHub();

            var loggerA = new FakeLogger();
            var loggerB = new FakeLogger();
            var loggerC = new FakeLogger();

            logem.AddLogger(loggerA);
            logem.AddLogger(loggerB);
            logem.AddLogger(loggerC);

            Assert.False(loggerA.HasBeenCalled);
            Assert.False(loggerB.HasBeenCalled);
            Assert.False(loggerC.HasBeenCalled);

            // Act
            await logem.LogAsync(testMessage, testObject);

            // Assert
            Assert.True(loggerA.HasBeenCalled);
            Assert.True(loggerB.HasBeenCalled);
            Assert.True(loggerC.HasBeenCalled);
        }

        [Theory]
        [InlineData(null, true, true)]
        [InlineData(CategoryA, true, false)]
        [InlineData(CategoryB, false, true)]
        [InlineData(CategoryC, true, true)]
        [InlineData(CategoryD, false, false)]
        public async Task ExecuteLog_WithCategory_CallsAllLoggersWithGivenCategory(
            string category, bool loggerACalled, bool loggerBCalled)
        {
            // Arrange
            var logem = new LogemHub();

            var loggerA = new FakeLogger();
            var loggerB = new FakeLogger();

            Assert.False(loggerA.HasBeenCalled);
            Assert.False(loggerB.HasBeenCalled);

            logem.AddLogger(loggerA, CategoryA, CategoryC);
            logem.AddLogger(loggerB, CategoryB, CategoryC);

            // Act
            await logem.LogAsync(testMessage, null, category);

            // Assert
            Assert.Equal(loggerACalled, loggerA.HasBeenCalled);
            Assert.Equal(loggerBCalled, loggerB.HasBeenCalled);
        }
    }
}
