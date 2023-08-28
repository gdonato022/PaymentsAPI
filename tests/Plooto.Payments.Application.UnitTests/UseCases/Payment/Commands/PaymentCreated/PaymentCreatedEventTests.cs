using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Moq;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Application.UseCases.Payment.Commands;
using Plooto.Payments.Domain.Entities;
using Plooto.Payments.Application.UnitTests.Extensions;

namespace Plooto.Payments.Application.UnitTests.UseCases.Payment.Commands.PaymentCreated
{
    public class PaymentCreatedEventTests
    {
        private readonly AutoMocker _mocker;
        private readonly PaymentCreatedEventHandler _handler;
        private readonly PaymentCreatedEventValidator _validator;

        public PaymentCreatedEventTests()
        {
            _mocker = new AutoMocker();

            _handler = _mocker.CreateInstance<PaymentCreatedEventHandler>();
            _validator = _mocker.CreateInstance<PaymentCreatedEventValidator>();
        }

        [Fact]
        public async Task Validator_ValidCommand_ShouldPassValidation()
        {
            var payment = new Domain.Entities.Payment
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.BankTransfer
            };

            var command = new PaymentCreatedEvent(payment);

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_InvalidCommand_ShouldNotPassValidation()
        {
            var command = new PaymentCreatedEvent(null);

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("'Payment' must not be empty.");
        }

        [Fact]
        public async Task Handler_ValidCommand_ShouldCreate()
        {
            var payment = new Domain.Entities.Payment
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.BankTransfer
            };

            var bill = new Domain.Entities.Bill
            {
                Id = 1,
                Amount = 1,
            };

            var command = new PaymentCreatedEvent(payment);

            var billRepository = _mocker.GetMock<IBillRepository>();
            var paymentRepository = _mocker.GetMock<IPaymentRepository>();
            var unitOfWork = _mocker.GetMock<IUnitOfWork>();

            billRepository
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bill);

            paymentRepository
                .Setup(s => s.GetPaymentsByBillAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { payment });

            unitOfWork.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            bill.IsPaid.Should().BeTrue();
            billRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Bill>()));
            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Handler_RepositoryThrowsException_ShouldLogTheExceptionAndThrow()
        {
            var payment = new Domain.Entities.Payment
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.BankTransfer
            };

            var command = new PaymentCreatedEvent(payment);

            var exceptionMessage = "Error to connect to database";

            var repository = _mocker.GetMock<IBillRepository>();
            var logger = _mocker.GetMock<ILogger<PaymentCreatedEventHandler>>();

            repository
                 .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .Throws(new Exception(exceptionMessage));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            logger.VerifyLog(Times.Once, LogLevel.Error);
            repository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), exceptionMessage);
        }
    }
}
