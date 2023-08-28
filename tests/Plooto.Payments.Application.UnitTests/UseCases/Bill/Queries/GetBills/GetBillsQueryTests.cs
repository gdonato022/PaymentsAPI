using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Moq;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Application.UseCases.Bill.Queries;
using Plooto.Payments.Application.UnitTests.Extensions;
using FluentAssertions;

namespace Plooto.Payments.Application.UnitTests.UseCases.Bill.Queries.GetBills
{
    public class GetBillsQueryTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetBillsQueryHandler _handler;

        public GetBillsQueryTests()
        {
            _mocker = new AutoMocker();

            _handler = _mocker.CreateInstance<GetBillsQueryHandler>();
        }

        [Fact]
        public async Task GetBillsQueryHandler_PaymentsExists_ShouldReturnPaginatedListOfPayments()
        {
            // Arrange
            var query = new GetBillsQuery();

            var bills = new Domain.Entities.Bill[] { new Domain.Entities.Bill { Id = 1 } };
            var repository = _mocker.GetMock<IBillRepository>();

            repository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bills);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(bills.Count());
            repository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetBillsQueryHandler_RepositoryThrowsException_ShouldLogTheExceptionAndThrow()
        {
            // Arrange
            var query = new GetBillsQuery();
            var exceptionMessage = "Error to connect to database";

            var logger = _mocker.GetMock<ILogger<GetBillsQueryHandler>>();
            var repository = _mocker.GetMock<IBillRepository>();

            repository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));

            logger.VerifyLog(Times.Once, LogLevel.Error);
            repository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), exceptionMessage);
        }
    }
}
