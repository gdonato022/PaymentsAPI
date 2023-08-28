using Microsoft.Extensions.Logging;
using Moq;

namespace Plooto.Payments.Application.UnitTests.Extensions
{
    public static class MockExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, Func<Times> times, LogLevel logLevel)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
        }
    }
}
