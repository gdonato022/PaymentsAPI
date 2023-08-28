using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Moq;
using Plooto.Payments.Application.UseCases.Payment.Queries;
using FluentAssertions;
using Plooto.Payments.Application.UnitTests.Extensions;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UnitTests.UseCases.Payment.Queries.GetPaymentsByBill
{
    public class GetPaymentsByBillQueryTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetPaymentsByBillQueryHandler _handler;

        public GetPaymentsByBillQueryTests()
        {
            _mocker = new AutoMocker();

            _handler = _mocker.CreateInstance<GetPaymentsByBillQueryHandler>();
        }

        [Fact]
        public async Task GetPaymentsByBillQueryHandler_PaymentsExists_ShouldReturnPaginatedListOfPayments()
        {
            // Arrange
            var query = new GetPaymentsByBillQuery
            {
                BillId = 1
            };

            var payments = new Domain.Entities.Payment[] { new Domain.Entities.Payment { Id = 1 } };
            var repository = _mocker.GetMock<IPaymentRepository>();

            repository
                .Setup(x => x.GetPaymentsByBillAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments);           

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(payments.Count());
            repository.Verify(x => x.GetPaymentsByBillAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task GetPaymentsByBillQueryHandler_RepositoryThrowsException_ShouldLogTheExceptionAndThrow()
        {
            // Arrange
            var query = new GetPaymentsByBillQuery();
            var exceptionMessage = "Error to connect to database";

            var logger = _mocker.GetMock<ILogger<GetPaymentsByBillQueryHandler>>();
            var repository = _mocker.GetMock<IPaymentRepository>();

            repository
                .Setup(x => x.GetPaymentsByBillAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception(exceptionMessage));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));

            logger.VerifyLog(Times.Once, LogLevel.Error);
            repository.Verify(x => x.GetPaymentsByBillAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), exceptionMessage);
        }
    }
}
