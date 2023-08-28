using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq.AutoMock;
using Moq;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Application.UseCases.Payment.Commands;
using Plooto.Payments.Application.UnitTests.Extensions;
using Plooto.Payments.Domain.Interfaces.Events;

namespace Plooto.Payments.Application.UnitTests.UseCases.Payment.Commands
{
    public class CreatePaymentCommandTests
    {
        private readonly AutoMocker _mocker;
        private readonly CreatePaymentCommandHandler _handler;
        private readonly CreatePaymentCommandValidator _validator;

        public CreatePaymentCommandTests()
        {
            _mocker = new AutoMocker();

            _handler = _mocker.CreateInstance<CreatePaymentCommandHandler>();
            _validator = _mocker.CreateInstance<CreatePaymentCommandValidator>();
        }

        [Fact]
        public async Task Validator_ValidCommand_ShouldPassValidation()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.EmailTransfer
            };

            var repository = _mocker.GetMock<IBillRepository>();

            repository
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Domain.Entities.Bill { Id = 1 });

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_InvalidCommand_ShouldNotPassValidation()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 0,
                BillId = 0,
                DebitDate = DateTime.MinValue,
                PaymentMethod = (Domain.Enums.PaymentMethod)99
            };

            var repository = _mocker.GetMock<IBillRepository>();

            repository
                .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(Domain.Entities.Bill));

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(4);
            result.Errors.Select(x => x.ErrorMessage).Should().Contain(CreatePaymentCommandValidator.BILL_DOES_NOT_EXISTS);
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("'Amount' must be greater than '0'.");
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("'Payment Method' has a range of values which does not include '99'.");
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("The specified condition was not met for 'Debit Date'.");
        }

        [Fact]
        public async Task Handler_ValidCommand_ShouldCreate()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.EmailTransfer
            };

            var paymentId = 0;

            var repository = _mocker.GetMock<IPaymentRepository>();
            var billRepository = _mocker.GetMock<IBillRepository>();
            var unitOfWork = _mocker.GetMock<IUnitOfWork>();
            var domainEventDispatcher = _mocker.GetMock<IDomainEventDispatcher>();

            repository
                .Setup(s => s.AddAsync(It.IsAny<Domain.Entities.Payment>(), It.IsAny<CancellationToken>()))
                .Callback(() => { paymentId = 1; });

            billRepository
               .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Domain.Entities.Bill { Id = 1 });

            domainEventDispatcher
                .Setup(s => s.DispatchAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
                .Verifiable();

            unitOfWork.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            paymentId.Should().Be(1);
            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            domainEventDispatcher.Verify(x => x.DispatchAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Handler_RepositoryThrowsException_ShouldLogTheExceptionAndThrow()
        {
            var command = new CreatePaymentCommand
            {
                Amount = 1,
                BillId = 1,
                DebitDate = DateTime.UtcNow,
                PaymentMethod = Domain.Enums.PaymentMethod.EmailTransfer
            };

            var exceptionMessage = "Error to connect to database";

            var repository = _mocker.GetMock<IPaymentRepository>();
            var billRepository = _mocker.GetMock<IBillRepository>();
            var logger = _mocker.GetMock<ILogger<CreatePaymentCommandHandler>>();

            billRepository
               .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Domain.Entities.Bill { Id = 1 });

            repository
                 .Setup(s => s.AddAsync(It.IsAny<Domain.Entities.Payment>(), It.IsAny<CancellationToken>()))
                 .Throws(new Exception(exceptionMessage));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            logger.VerifyLog(Times.Once, LogLevel.Error);
            repository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Payment>(), It.IsAny<CancellationToken>()), exceptionMessage);
        }
    }
}
